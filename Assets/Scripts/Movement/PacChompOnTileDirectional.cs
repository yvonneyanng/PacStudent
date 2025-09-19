using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PacChompOnTileDirectional : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private Tilemap pelletTilemap;
    [SerializeField] private Animator animator;

    [Header("Timing")]
    [SerializeField, Range(0.05f, 0.25f)] private float closedDuration = 0.1f;

    [Header("Animator State Names (exact)")]
    [SerializeField] private string walkUp = "Pac_Move_Up";
    [SerializeField] private string walkDown = "Pac_Move_Down";
    [SerializeField] private string walkLeft = "Pac_Move_Left";
    [SerializeField] private string walkRight = "Pac_Move_Right";
    [SerializeField] private string closedUp = "Pac_Close_Up";
    [SerializeField] private string closedDown = "Pac_Close_Down";
    [SerializeField] private string closedLeft = "Pac_Close_Left";
    [SerializeField] private string closedRight = "Pac_Close_Right";

    private HashSet<Vector3Int> eatenCells = new HashSet<Vector3Int>();
    private Vector3 lastPos;
    private bool chompBusy;

    private enum Dir { Up, Down, Left, Right }
    private Dir lastDir = Dir.Right;

    void Start() { lastPos = transform.position; }

    void Update()
    {
        if (!pelletTilemap) return;

        Vector3Int cell = pelletTilemap.WorldToCell(transform.position);

        if (pelletTilemap.HasTile(cell) && !eatenCells.Contains(cell))
        {
            eatenCells.Add(cell);
            pelletTilemap.SetTile(cell, null);

            if (!chompBusy)
            {
                var d = GetDirection();
                StartCoroutine(PlayClosedThenResume(d));
            }
        }

        lastDir = GetDirection();
        lastPos = transform.position;
    }

    private Dir GetDirection()
    {
        Vector3 delta = transform.position - lastPos;
        if (delta.sqrMagnitude < 0.0001f) return lastDir; 
        if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y)) return (delta.x >= 0f) ? Dir.Right : Dir.Left;
        return (delta.y >= 0f) ? Dir.Up : Dir.Down;
    }

    private IEnumerator PlayClosedThenResume(Dir d)
    {
        chompBusy = true;

        string closed = closedRight, walk = walkRight;
        switch (d)
        {
            case Dir.Up:    closed = closedUp;    walk = walkUp;    break;
            case Dir.Down:  closed = closedDown;  walk = walkDown;  break;
            case Dir.Left:  closed = closedLeft;  walk = walkLeft;  break;
            case Dir.Right: closed = closedRight; walk = walkRight; break;
        }

        animator.Play(closed, 0, 0f);
        yield return new WaitForSeconds(closedDuration);
        animator.Play(walk, 0, 0f);

        chompBusy = false;
    }
}
