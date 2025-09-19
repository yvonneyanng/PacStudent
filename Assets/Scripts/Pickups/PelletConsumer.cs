using UnityEngine;
using UnityEngine.Tilemaps;

public class PelletConsumer : MonoBehaviour
{
    public Tilemap pelletsMap;        
    public AudioClip pelletSfx;       
    public AudioClip powerPelletSfx;  

    AudioSource sfx;

    void Awake()
    {
        sfx = GetComponent<AudioSource>();
    }

    void LateUpdate()
    {
        if (!pelletsMap) return;
        Vector3Int cell = pelletsMap.WorldToCell(transform.position);
        var tile = pelletsMap.GetTile(cell);
        if (tile != null)
        {
            pelletsMap.SetTile(cell, null);
            if (pelletSfx && sfx) sfx.PlayOneShot(pelletSfx, 0.8f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PowerPellet"))
        {
            if (powerPelletSfx && sfx) sfx.PlayOneShot(powerPelletSfx, 0.9f);
            Destroy(other.gameObject);
        }
    }
}