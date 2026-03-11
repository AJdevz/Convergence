using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements; // Unused, can be removed

public class PlayerController : MonoBehaviour
{
    public float moveSpeed; // Speed at which the player moves
    private Rigidbody myRigidBody; // Reference to the Rigidbody component
    private Vector3 moveInput; // Stores player input direction
    private Vector3 moveVelocity; // Stores calculated movement velocity

    private Camera mainCamera; // Reference to the main camera

    public GunController theGun; // Reference to the GunController script

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>(); // Get the Rigidbody component
        mainCamera = Object.FindFirstObjectByType<Camera>(); // Find the main camera in the scene
    }

    void Update()
    {
        // Get movement input from the player
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput * moveSpeed; // Multiply input by moveSpeed to determine velocity

        // Create a ray from the camera to the mouse position
        Ray cameraRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane Floor = new Plane(Vector3.up, Vector3.zero); 
        float rayLength;

        // Check if the ray intersects the plane
        if (Floor.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength); // Get the intersection point
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.blue); // Draw a debug line for visualization

            // Rotate the player to face the point where the ray intersects the plane
            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }

        // Handle shooting with left mouse button
        if (Input.GetMouseButtonDown(0))
            theGun.isFiring = true; // Start firing

        if (Input.GetMouseButtonUp(0))
            theGun.isFiring = false; // Stop firing
    }

    private void FixedUpdate()
    {
        myRigidBody.linearVelocity = moveVelocity; // Apply movement velocity to Rigidbody
    }
}
