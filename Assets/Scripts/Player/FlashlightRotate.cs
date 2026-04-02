using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FlashlightRotate : MonoBehaviour
{
    public bool IsFocus = false;
    // Start is called before the first frame update
    Quaternion originalRotation;

    void Awake()
    {
        originalRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Find the main camera
            RotateTowardsCamera();
    }


    public void RotateTowardsParentForward()
    {
        if (transform.parent == null) return;

        Vector3 targetDirection = transform.parent.forward;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        float rotationSpeed = 10f; // Adjust for smoothness
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    void RotateTowardsCamera()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        // Get the direction the camera is facing
        Vector3 targetDirection = cam.transform.forward;

        // Calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);

        // Smoothly rotate towards the target rotation
        float rotationSpeed = 10f; // Adjust for smoothness
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

