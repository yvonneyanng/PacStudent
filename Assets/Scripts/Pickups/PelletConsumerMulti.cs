using UnityEngine;
using UnityEngine.Tilemaps;

public class PelletConsumerMulti : MonoBehaviour
{
    [Header("Pellet Tilemaps (assign all 4)")]
    public Tilemap[] pelletMaps;           // TL, TR, BL, BR

    [Header("SFX (optional)")]
    public AudioClip pelletSfx;
    public AudioClip powerPelletSfx;

    AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (pelletMaps == null || pelletMaps.Length == 0) return;

        // Eat one normal pellet per frame from whichever tilemap has one at Pac's cell
        Vector3 pacWorld = transform.position;
        for (int i = 0; i < pelletMaps.Length; i++)
        {
            var map = pelletMaps[i];
            if (!map) continue;

            Vector3Int cell = map.WorldToCell(pacWorld);
            if (map.GetTile(cell) != null)
            {
                map.SetTile(cell, null);
                if (pelletSfx && sfx) sfx.PlayOneShot(pelletSfx, 0.8f);
                break;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // PowerPellets are prefabs with IsTrigger = true
        if (other.CompareTag("PowerPellet"))
        {
            if (powerPelletSfx && sfx) sfx.PlayOneShot(powerPelletSfx, 0.9f);
            Destroy(other.gameObject);
            // TODO (later): trigger ghosts IsScared/IsRecovering timers
        }
    }
}