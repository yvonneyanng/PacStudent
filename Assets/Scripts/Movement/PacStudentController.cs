using UnityEngine;

public class PacStudentController : MonoBehaviour
{
    public float gridSize = 1f;
    public float gridOffset = 0.5f;
    public float tilesPerSecond = 8f;
    public LayerMask wallMask;
    public Animator animator;

    Vector2Int lastInput = Vector2Int.zero;
    Vector2Int currentInput = Vector2Int.zero;
    Vector3 targetPos;
    bool isLerping = false;
    float t = 0f;

    void Start()
    {
        var p = transform.position;
        transform.position = new Vector3(
            Mathf.Round((p.x - gridOffset) / gridSize) * gridSize + gridOffset,
            Mathf.Round((p.y - gridOffset) / gridSize) * gridSize + gridOffset,
            p.z
        );
        targetPos = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) lastInput = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.S)) lastInput = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.A)) lastInput = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.D)) lastInput = Vector2Int.right;

        if (!isLerping)
        {
            if (lastInput != Vector2Int.zero && CanMove(lastInput))
            {
                currentInput = lastInput;
                StartStep(currentInput);
            }
            else if (currentInput != Vector2Int.zero && CanMove(currentInput))
            {
                StartStep(currentInput);
            }
            else
            {
                if (animator) { animator.SetFloat("MoveX", 0); animator.SetFloat("MoveY", 0); }
            }
        }
        else
        {
            t += (tilesPerSecond * Time.deltaTime) / Mathf.Max(0.0001f, gridSize);
            float u = Mathf.Clamp01(t);
            transform.position = Vector3.Lerp(transform.position, targetPos, u);
            if (u >= 1f - 1e-6f)
            {
                transform.position = targetPos;
                isLerping = false;
                t = 0f;
            }
        }
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, 1.5f, 26.5f),
            Mathf.Clamp(transform.position.y, -28.5f, -1.5f),
            transform.position.z
        );

    }

    void StartStep(Vector2Int dir)
    {
        var start = transform.position;
        targetPos = start + new Vector3(dir.x * gridSize, dir.y * gridSize, 0f);
        isLerping = true;
        t = 0f;
        if (animator) { animator.SetFloat("MoveX", dir.x); animator.SetFloat("MoveY", dir.y); }
    }

    bool CanMove(Vector2Int dir)
    {
        Vector3 probe = transform.position + new Vector3(dir.x * gridSize, dir.y * gridSize, 0f);
        return Physics2D.OverlapPoint(probe, wallMask) == null;
    }

}
