using UnityEngine;
using UnityEngine.Tilemaps;

public class PelletConsumer : MonoBehaviour
{
    public Tilemap pelletsMap;        // assign Tilemap_Pellets
    public AudioClip pelletSfx;       // optional one-shot
    public AudioClip powerPelletSfx;  // optional one-shot

    AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (!pelletsMap) return;
        // Convert Pac's position to the pellet tile cell
        Vector3Int cell = pelletsMap.WorldToCell(transform.position);
        var tile = pelletsMap.GetTile(cell);
        if (tile != null)
        {
            pelletsMap.SetTile(cell, null);
            if (pelletSfx && sfx) sfx.PlayOneShot(pelletSfx, 0.8f);
        }
    }

    // Handle the 4 PowerPellet prefabs we placed (with trigger colliders)
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerPellet"))
        {
            if (powerPelletSfx && sfx) sfx.PlayOneShot(powerPelletSfx, 0.9f);
            Destroy(other.gameObject);
            // (Optional) trigger ghost scared/recovering timers later
        }
    }
}