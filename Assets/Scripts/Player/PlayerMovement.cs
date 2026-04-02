using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float Walkspeed;
    [SerializeField] private float Gravity;
    private CharacterController characterController;
    [SerializeField] private float RotationSpeed = 1f;
    [SerializeField] float RunMultipler;
    [SerializeField] float ExhaustionRate = 100f;
    [SerializeField] private float ExhaustionSpeed = 1f;
    [SerializeField] private float ExhaustionRechargeSpeed = 1f;
    [SerializeField] private GameObject SoundCollision;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float RunPitch = 1.5f;
    private float originalPitch;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        if (SoundCollision != null)
        {
            SoundCollision.SetActive(false);
        }
        originalPitch = audioSource.pitch;
        RunPitch *= originalPitch;

    }

    void Update()
    {
        PlayerMove();
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        characterController.Move(Vector3.down * Gravity * Time.deltaTime);
    }

    private void PlayerMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, 0, moveZ);
        if (move != Vector3.zero)
        {
            SoundCollision.SetActive(true);
            audioSource.enabled = true;
        }
        else
        {
            SoundCollision.SetActive(false);
            audioSource.enabled = false;
        }
        // Get the camera's forward and right directions, ignoring the y component
        Camera cam = Camera.main;
        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;



        camForward.Normalize();
        camRight.Normalize();

        // Move relative to camera
        Vector3 moveDir = camForward * moveZ + camRight * moveX;
        moveDir.Normalize();

        // Rotate player to face camera direction when moving
        FaceCameraDirection(camForward, moveDir);
        characterController.Move(moveDir * RunInput() * Time.deltaTime);
        ExhaustionRate = Mathf.Clamp(ExhaustionRate, 0f, 100f);
    }

    private float RunInput()
    {
        float MoveSpeed = Walkspeed;
        if (Input.GetButton("Run"))
        {
            if (ExhaustionRate > 1)
            {
                audioSource.pitch = RunPitch;
                ExhaustionRate -= ExhaustionSpeed * Time.deltaTime;
                MoveSpeed *= RunMultipler;
            }
            else
            { 
            //Get Vignett on PLayer's Cinemachine post process vignate camera
            }


        }
        else
        {
            audioSource.pitch = originalPitch;
            ExhaustionRate += ExhaustionRechargeSpeed * Time.deltaTime;
        }
        return MoveSpeed;
    }

    private void FaceCameraDirection(Vector3 camForward, Vector3 moveDir)
    {
        Quaternion targetRotation = transform.rotation;
        if (Input.GetButton("Focus"))
        {
            
            targetRotation = Quaternion.LookRotation(camForward);
        }
        else {
            if (moveDir.sqrMagnitude > 0.01f)
            {
                targetRotation = Quaternion.LookRotation(moveDir);
            }
        }
        
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);
    }

}
