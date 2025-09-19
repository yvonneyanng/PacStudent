using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PacStudentMovement : MonoBehaviour
{
    [Header("Path")]
    public Transform waypointsRoot;           
    public float speed = 3.5f;                
    public float snapEpsilon = 0.02f;         

    [Header("Animator Param Names")]
    public string paramMoveX = "MoveX";
    public string paramMoveY = "MoveY";
    public string paramDir   = "Dir";         
    public string paramIsDead= "IsDead";

    [Header("SFX")]
    public AudioClip moveClip;                
    public float stepInterval = 0.22f;       

    Animator anim;
    AudioSource sfx;
    readonly List<Vector3> points = new List<Vector3>();
    int index = 0;                           
    float stepTimer = 0f;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sfx  = GetComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.loop = false;

        points.Clear();
        if (waypointsRoot != null)
        {
            foreach (Transform child in waypointsRoot)
                points.Add(SnapToCellCenter(child.position));
        }

        if (points.Count >= 2)
        {
            transform.position = points[0];   
            index = 1;                        
        }

        if (!string.IsNullOrEmpty(paramIsDead))
            anim.SetBool(paramIsDead, false); 
    }

    void Update()
    {
        if (points.Count < 2) return;

        Vector3 pos = transform.position;
        Vector3 target = points[index];
        Vector3 to = (target - pos);

        if (to.sqrMagnitude <= snapEpsilon * snapEpsilon)
        {
            pos = target;
            index = (index + 1) % points.Count;
            target = points[index];
            to = target - pos;
        }
        else
        {
            float step = speed * Time.deltaTime;
            pos = Vector3.MoveTowards(pos, target, step);
        }

        transform.position = pos;

        Vector3 dir = to.normalized;
        SetAnimParams(dir);

        stepTimer -= Time.deltaTime;
        if (moveClip && stepTimer <= 0f && dir.sqrMagnitude > 0.0001f)
        {
            sfx.PlayOneShot(moveClip, 0.9f);
            stepTimer = stepInterval;
        }
    }

    void SetAnimParams(Vector3 dir)
    {
        float mx = 0f, my = 0f;
        int d = 0; 

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
        float rx = Mathf.Round(p.x - 0.5f) + 0.5f;
        float ry = Mathf.Round(p.y - 0.5f) + 0.5f;
        return new Vector3(rx, ry, 0f);
    }


#if UNITY_EDITOR
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
        if (prev.HasValue && waypointsRoot.childCount > 0)
        {
            Vector3 first = SnapToCellCenter(waypointsRoot.GetChild(0).position);
            Gizmos.DrawLine(prev.Value, first);
        }
    }
#endif
}
