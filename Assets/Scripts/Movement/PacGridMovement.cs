using UnityEngine;
using UnityEngine.Tilemaps;

public class PacGridMovement : MonoBehaviour
{
    [Header("Grid")]
    public Vector2 gridOrigin = Vector2.zero;
    public float cellSize = 1f;

    [Header("Movement")]
    public float speed = 5f;
    [Range(0.01f, 0.2f)] public float turnEpsilon = 0.1f; // how close to center we allow a buffered turn

    [Header("Collision")]
    public Tilemap wallsTilemap;

    [Header("Animation (optional)")]
    public Animator animator;
    public string walkUp = "WalkUp", walkDown = "WalkDown", walkLeft = "WalkLeft", walkRight = "WalkRight";

    private Vector2Int curDir = Vector2Int.right; // active direction
    private Vector2Int nextDir = Vector2Int.right; // buffered desired direction
    private Vector3 targetCenter; // world center of current cell

    void Start()
    {
        // snap to nearest cell center on start
        var cell = WorldToCell(transform.position);
        targetCenter = CellCenterWorld(cell);
        transform.position = targetCenter;
    }

    void Update()
    {
        // 1) read input -> set nextDir (buffer)
        ReadInput();

        // 2) compute current cell & its center
        Vector2Int cell = WorldToCell(transform.position);
        targetCenter = CellCenterWorld(cell);

        // 3) are we close enough to center to turn?
        float distToCenter = Vector2.Distance(transform.position, targetCenter);
        bool atCenter = distToCenter <= turnEpsilon;

        // Try to commit the buffered turn at (or very near) center
        if (atCenter && CanMove(cell, nextDir))
        {
            curDir = nextDir;
            // snap exactly to center before moving into the new tile
            transform.position = targetCenter;
            Face(curDir); // rotate / animate immediately BEFORE moving
        }
        else
        {
            // if blocked ahead and at center, stop (don’t drift past)
            if (atCenter && !CanMove(cell, curDir))
            {
                Face(curDir);
                return;
            }
        }

        // 4) move along current direction, but never skip the turn point
        Vector3 delta = new Vector3(curDir.x, curDir.y, 0f) * speed * Time.deltaTime;
        Vector3 newPos = transform.position + delta;

        // If we are crossing the center, clamp to center first so turn can happen next frame
        if (!atCenter)
        {
            Vector3 toCenter = targetCenter - transform.position;
            // are we about to pass the center along current axis?
            if (Vector3.Dot(toCenter, delta) < 0f) // we overshot
            {
                newPos = targetCenter;
            }
            else
            {
                // if we’d move beyond center this frame, clamp to center
                if (delta.sqrMagnitude > toCenter.sqrMagnitude)
                    newPos = targetCenter;
            }
        }

        transform.position = newPos;
    }

    private void ReadInput()
    {
        int x = 0, y = 0;
        // WASD / arrows both work
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) x = -1;
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) x = 1;

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) y = 1;
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) y = -1;

        Vector2Int wanted = Vector2Int.zero;
        if (x != 0 && y == 0) wanted = new Vector2Int(x, 0);
        else if (y != 0 && x == 0) wanted = new Vector2Int(0, y);

        if (wanted != Vector2Int.zero)
            nextDir = wanted; // buffer it; we’ll apply at center if possible
    }

    private bool CanMove(Vector2Int cell, Vector2Int dir)
    {
        if (dir == Vector2Int.zero) return false;
        Vector3Int nextCell = new Vector3Int(cell.x + dir.x, cell.y + dir.y, 0);
        // treat any tile present on wallsTilemap as blocked
        return wallsTilemap ? !wallsTilemap.HasTile(nextCell) : true;
    }

    private Vector2Int WorldToCell(Vector3 world)
    {
        Vector2 p = (Vector2)world - gridOrigin;
        int cx = Mathf.RoundToInt(p.x / cellSize);
        int cy = Mathf.RoundToInt(p.y / cellSize);
        return new Vector2Int(cx, cy);
    }

    private Vector3 CellCenterWorld(Vector2Int cell)
    {
        return new Vector3(gridOrigin.x + cell.x * cellSize, gridOrigin.y + cell.y * cellSize, 0f);
    }

    private void Face(Vector2Int dir)
    {
        // either rotate transform, or use animator states
        if (animator)
        {
            if (dir == Vector2Int.up) animator.Play(walkUp, 0, 0f);
            else if (dir == Vector2Int.down) animator.Play(walkDown, 0, 0f);
            else if (dir == Vector2Int.left) animator.Play(walkLeft, 0, 0f);
            else if (dir == Vector2Int.right) animator.Play(walkRight, 0, 0f);
        }
        else
        {
            // rotate sprite to face movement (if you don't use 4-direction clips)
            float angle = (dir == Vector2Int.up) ? 90f :
                          (dir == Vector2Int.down) ? -90f :
                          (dir == Vector2Int.left) ? 180f : 0f;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}
