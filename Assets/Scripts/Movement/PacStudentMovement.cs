using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PacStudentMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform waypointsRoot;           // Assign the Waypoints parent
    public float speed = 3.5f;                // Units/second (linear)
    public float snapEpsilon = 0.02f;         // Distance to consider "arrived"

    [Header("Animator Param Names")]
    public string paramMoveX = "MoveX";
    public string paramMoveY = "MoveY";
    public string paramDir   = "Dir";         // 0=Up,1=Down,2=Left,3=Right
    public string paramIsDead= "IsDead";

    [Header("SFX")]
    public AudioClip moveClip;                // Soft step/swish
    public float stepInterval = 0.22f;        // Seconds between steps

    Animator anim;
    AudioSource sfx;
    readonly List<Vector3> points = new List<Vector3>();
    int index = 0;                            // Next target index
    float stepTimer = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sfx  = GetComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.loop = false;

        // Collect waypoint positions in order
        points.Clear();
        if (waypointsRoot != null)
        {
            foreach (Transform child in waypointsRoot)
                points.Add(SnapToCellCenter(child.position));
        }

        if (points.Count >= 2)
        {
            transform.position = points[0];   // Start exactly at P0
            index = 1;                        // Head to P1
        }

        if (!string.IsNullOrEmpty(paramIsDead))
            anim.SetBool(paramIsDead, false); // A3: Pac is alive
    }

    void Update()
    {
        if (points.Count < 2) return;

        Vector3 pos = transform.position;
        Vector3 target = points[index];
        Vector3 to = (target - pos);

        // Arrived?
        if (to.sqrMagnitude <= snapEpsilon * snapEpsilon)
        {
            pos = target;
            index = (index + 1) % points.Count;
            target = points[index];
            to = target - pos;
        }
        else
        {
            // Linear, framerate-independent motion
            float step = speed * Time.deltaTime;
            pos = Vector3.MoveTowards(pos, target, step);
        }

        transform.position = pos;

        // Animator params from direction toward the next target
        Vector3 dir = to.normalized;
        SetAnimParams(dir);

        // Throttled step SFX while moving
        stepTimer -= Time.deltaTime;
        if (moveClip && stepTimer <= 0f && dir.sqrMagnitude > 0.0001f)
        {
            sfx.PlayOneShot(moveClip, 0.9f);
            stepTimer = stepInterval;
        }
    }

    void SetAnimParams(Vector3 dir)
    {
        // Choose dominant axis for clean 4-dir animation
        float mx = 0f, my = 0f;
        int d = 0; // 0=Up,1=Down,2=Left,3=Right

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            mx = Mathf.Sign(dir.x);
            d  = (mx < 0f) ? 2 : 3;
            my = 0f;
        }
        else
        {
            my = Mathf.Sign(dir.y);
            d  = (my < 0f) ? 1 : 0;
            mx = 0f;
        }

        if (!string.IsNullOrEmpty(paramMoveX)) anim.SetFloat(paramMoveX, mx);
        if (!string.IsNullOrEmpty(paramMoveY)) anim.SetFloat(paramMoveY, my);
        if (!string.IsNullOrEmpty(paramDir))   anim.SetInteger(paramDir, d);
    }

    static Vector3 SnapToCellCenter(Vector3 p)
    {
        // Use .5-centered grid (e.g., 6.5, 12.5)
        float rx = Mathf.Round(p.x - 0.5f) + 0.5f;
        float ry = Mathf.Round(p.y - 0.5f) + 0.5f;
        return new Vector3(rx, ry, 0f);
    }


#if UNITY_EDITOR
    // Visualize path in Scene view when selected
    void OnDrawGizmosSelected()
    {
        if (waypointsRoot == null) return;
        Gizmos.color = Color.yellow;
        Vector3? prev = null;
        foreach (Transform t in waypointsRoot)
        {
            Vector3 c = SnapToCellCenter(t.position);
            Gizmos.DrawWireSphere(c, 0.12f);
            if (prev.HasValue) Gizmos.DrawLine(prev.Value, c);
            prev = c;
        }
        // Loop line to first
        if (prev.HasValue && waypointsRoot.childCount > 0)
        {
            Vector3 first = SnapToCellCenter(waypointsRoot.GetChild(0).position);
            Gizmos.DrawLine(prev.Value, first);
        }
    }
#endif
}
