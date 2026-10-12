using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    [SerializeField] private float conveyorSpeed = 0.5f;
    
    private void OnCollisionStay(Collision other)
    {
        Rigidbody rb = other.rigidbody;

        if (rb == null)
            return;

        Vector3 velocity = transform.forward * conveyorSpeed;

        rb.linearVelocity = new Vector3(
            velocity.x,
            rb.linearVelocity.y,
            velocity.z
        );
    }
}