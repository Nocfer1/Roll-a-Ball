using UnityEngine;

public class PushBox : MonoBehaviour
{
    [SerializeField] private float pushForce = 12f;

    private void OnCollisionStay(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
        if (playerRb == null) return;

        Vector3 pushDirection = collision.transform.position - transform.position;
        pushDirection.y = 0f;

        if (pushDirection.sqrMagnitude < 0.001f) return;

        pushDirection.Normalize();

        playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    }
}