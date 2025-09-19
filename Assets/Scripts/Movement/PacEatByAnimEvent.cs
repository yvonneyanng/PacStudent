using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacEatByAnimEvent : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private Tilemap pelletTilemap;
    [SerializeField] private AudioSource chompSfx; // optional

    // Prevent re-eating the same cell if the event fires multiple times while Pac lingers
    private readonly HashSet<Vector3Int> eatenCells = new HashSet<Vector3Int>();
    private Vector3Int lastCell; // light debounce

    // Called by Animation Events on the closed-mouth frame in each walk clip
    public void OnMouthClosedFrame()
    {
        if (!pelletTilemap) return;

        Vector3Int cell = pelletTilemap.WorldToCell(transform.position);

        // If we’re still in the exact same cell as last time and already handled it, bail early
        if (cell == lastCell && eatenCells.Contains(cell)) return;

        if (pelletTilemap.HasTile(cell) && !eatenCells.Contains(cell))
        {
            pelletTilemap.SetTile(cell, null);   // remove pellet tile
            eatenCells.Add(cell);

            if (chompSfx) chompSfx.Play();      // optional sfx
        }

        lastCell = cell;
    }
}