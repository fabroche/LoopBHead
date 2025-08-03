using System.Diagnostics;
using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f; // Set player's movement speed.
    public float rotationSpeed = 120.0f; // Set player's rotation speed.

    private Rigidbody rb; // Reference to player's Rigidbody.
    private bool isJumping = false; // Flag to check if player is jumping.

    public float jumpForce = 5f; // Force applied when jumping.

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // Access player's Rigidbody.
    }

    // Update is called once per frame
    void Update()
    {
        UnityEngine.Debug.Log($"isJumping: {isJumping}"); // Log to check if Update is running.
        if (Input.GetButtonDown("Jump"))
        {
            Jump(); // Call Jump method when jump button is pressed.
        }
    }


    // Handle physics-based movement and rotation.
    private void FixedUpdate()
    {
      HandleMovement();
    }

    void HandleMovement()
    {
        // Move player based on vertical input.
        float moveVertical = Input.GetAxis("Vertical");
 
        //UnityEngine.Debug.Log("Vertical Input: " + moveVertical); // Log vertical input for debugging.
        Vector3 movement = transform.forward * moveVertical * speed * Time.fixedDeltaTime;
        //UnityEngine.Debug.Log("Transform Foward: " + transform.forward); // Log vertical input for debugging.
        rb.MovePosition(rb.position + movement);

        // Rotate player based on horizontal input.
        float turn = Input.GetAxis("Horizontal") * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void Jump()
    {
        // Check if the player is grounded before allowing a jump.
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange); // Apply an upward force for jumping.
            //UnityEngine.Debug.Log("Jumped!"); // Log jump action for debugging.
        }
    }

    bool IsGrounded()
    {
        // Check if the player is grounded using a raycast.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            UnityEngine.Debug.Log("Player is grounded."); // Log grounded status for debugging.
            return true; // Player is grounded.
        }
        UnityEngine.Debug.Log("Player is not grounded."); // Log not grounded status for debugging.
        return false; // Player is not grounded.
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the player collides with an object tagged as "Floor".
        if (collision.gameObject.CompareTag("SceneFloor"))
        {
            isJumping = false; // Reset jumping flag when colliding with the floor.
            UnityEngine.Debug.Log("Player has collided with the floor!"); // Log collision with floor.
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Check if the player collides with an object tagged as "Floor".
        if (collision.gameObject.CompareTag("SceneFloor"))
        {
            isJumping = true; // Set jumping flag when leaving the floor.
            UnityEngine.Debug.Log("Player left the floor!"); // Log collision with floor.
        }
    }
}