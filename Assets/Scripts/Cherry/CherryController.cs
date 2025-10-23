using UnityEngine;
using System.Collections;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public Transform levelCenter;
    public float initialDelay = 5f;
    public float respawnDelay = 5f;
    public float speedUnitsPerSec = 6f;
    public float outsidePadding = 1.5f;

    public bool useManualBounds = true;
    public float leftX;
    public float rightX;
    public float bottomY;
    public float topY;

    void OnEnable(){ StartCoroutine(Loop()); }

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

        Vector3 center = levelCenter ? levelCenter.position : Vector3.zero;
        float xMin = useManualBounds ? leftX  : center.x - 10f;
        float xMax = useManualBounds ? rightX : center.x + 10f;
        float yMin = useManualBounds ? bottomY: center.y - 10f;
        float yMax = useManualBounds ? topY   : center.y + 10f;

        int side = Random.Range(0, 4);
        Vector3 start = center, end = center;

        switch (side)
        {
            case 0: start = new Vector3(xMin - outsidePadding, center.y, 0); end = new Vector3(xMax + outsidePadding, center.y, 0); break;
            case 1: start = new Vector3(xMax + outsidePadding, center.y, 0); end = new Vector3(xMin - outsidePadding, center.y, 0); break;
            case 2: start = new Vector3(center.x, yMin - outsidePadding, 0); end = new Vector3(center.x, yMax + outsidePadding, 0); break;
            default:start = new Vector3(center.x, yMax + outsidePadding, 0); end = new Vector3(center.x, yMin - outsidePadding, 0); break;
        }

        var cherry = Instantiate(cherryPrefab, start, Quaternion.identity);
        var sr = cherry.GetComponent<SpriteRenderer>();
        if (sr) sr.sortingOrder = 2000;

        float dist = Vector3.Distance(start, end);
        float dur = dist / Mathf.Max(0.01f, speedUnitsPerSec);
        float t = 0f;
        while (t < dur)
        {
            t += Time.deltaTime;
            float u = Mathf.Clamp01(t / dur);
            cherry.transform.position = Vector3.Lerp(start, end, u);
            yield return null;
        }

        Destroy(cherry);
    }
}
