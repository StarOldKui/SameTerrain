using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightSimulator : MonoBehaviour
{
    // Camera move speed
    public float MovingSpeed = 10;
    // Rotate camera with mouse
    public float speedH = 2.0f;
    public float speedV = 2.0f;
    public float pitch = 35.0f;
    public float yaw = -130.0f;
    // Camera collision detection
    private Rigidbody rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        // Assign the collision detector
        rigidBody = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate camera with mouse
        rotateCamera();
        // Move the camera from four directions
        moveCamera();
    }

    void rotateCamera()
    {
        yaw += speedH * Input.GetAxis("Mouse X");
        pitch -= speedV * Input.GetAxis("Mouse Y");
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    void moveCamera()
    {
        // If put down the "S", the camera should move back
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.Translate(0.0f, 0.0f,- Time.deltaTime * MovingSpeed);
        }

        // If put down the "W", the camera should move front
        if (Input.GetKey(KeyCode.W))
        {

            this.transform.Translate(0.0f, 0.0f, Time.deltaTime * MovingSpeed);
        }
        // If put down the "A", the camera should move left
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Translate(-Time.deltaTime * MovingSpeed, 0.0f, 0.0f);

        }
        // If put down the "D", the camera should move right
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Translate(Time.deltaTime * MovingSpeed, 0.0f, 0.0f);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Terrain") { 
            rigidBody.isKinematic = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "Terrain")
        {
            rigidBody.isKinematic = false;
        }
    }
}
