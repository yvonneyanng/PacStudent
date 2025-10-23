using UnityEngine;
using System.Collections;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public Transform levelCenter;
    public Renderer boundsFrom;
    public float initialDelay = 5f;
    public float respawnDelay = 5f;
    public float speedUnitsPerSec = 6f;
    public float outsidePadding = 1.5f;

    Bounds levelBounds;

    void Awake()
    {
        if (boundsFrom) levelBounds = boundsFrom.bounds;
    }

    void OnEnable()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        yield return new WaitForSeconds(initialDelay);
        while (true)
        {
            yield return SpawnRunDestroyOnce();
            yield return new WaitForSeconds(respawnDelay);
        }
    }

    IEnumerator SpawnRunDestroyOnce()
    {
        if (!cherryPrefab) yield break;
        if (levelBounds.size == Vector3.zero && boundsFrom) levelBounds = boundsFrom.bounds;

        Vector3 center = levelCenter ? levelCenter.position : levelBounds.center;
        float xMin = levelBounds.min.x, xMax = levelBounds.max.x;
        float yMin = levelBounds.min.y, yMax = levelBounds.max.y;

        int side = Random.Range(0, 4);
        Vector3 start = center, end = center;

        switch (side)
        {
            case 0:
                start = new Vector3(xMin - outsidePadding, center.y, 0);
                end   = new Vector3(xMax + outsidePadding, center.y, 0);
                break;
            
            case 1:
                start = new Vector3(xMax + outsidePadding, center.y, 0);
                end   = new Vector3(xMin - outsidePadding, center.y, 0);
                break;
            
            case 2:
                start = new Vector3(center.x, yMin - outsidePadding, 0);
                end   = new Vector3(center.x, yMax + outsidePadding, 0);
                break;
            
            default:
                start = new Vector3(center.x, yMax + outsidePadding, 0);
                end   = new Vector3(center.x, yMin - outsidePadding, 0);
                break;
        }

        var cherry = Instantiate(cherryPrefab, start, Quaternion.identity);
        var sr = cherry.GetComponent<SpriteRenderer>();
        if (sr) sr.sortingOrder = 500;

        yield return MoveLinear(cherry.transform, start, end);
        Destroy(cherry);
    }


    IEnumerator MoveLinear(Transform t, Vector3 a, Vector3 b)
    {
        float dist = Vector3.Distance(a, b);
        float dur = dist / Mathf.Max(0.01f, speedUnitsPerSec);
        float elapsed = 0f;
        while (elapsed < dur)
        {
            elapsed += Time.deltaTime;
            float u = Mathf.Clamp01(elapsed / dur);
            t.position = Vector3.Lerp(a, b, u);
            yield return null;
        }
        t.position = b;
    }
}
