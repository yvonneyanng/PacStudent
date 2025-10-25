using UnityEngine;

public class CherryPickup : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (GameManager.Instance) GameManager.Instance.AddCherry();
        Destroy(gameObject);
    }
}