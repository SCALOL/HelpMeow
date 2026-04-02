using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate : MonoBehaviour
{
    [SerializeField] private Transform followTarget;

    [SerializeField] private float rotationalSpeed = 30f;
    [SerializeField] private float TopClamp = 70f;
    [SerializeField] private float BottomClamp = -40f;

    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;

    private void LateUpdate()
    {
        CameraLogic();
    }
    private void CameraLogic() { 
        float mouseX = GetMouseInput("Mouse X");
        float mouseY = GetMouseInput("Mouse Y");
        cinemachineTargetPitch = UpdateRotation(cinemachineTargetPitch, mouseY, BottomClamp, TopClamp, true);
        cinemachineTargetYaw = UpdateRotation(cinemachineTargetYaw, mouseX, float.MinValue, float.MaxValue, false);

        ApplyRotation(cinemachineTargetPitch, cinemachineTargetYaw);
    }

    private void ApplyRotation(float pitch, float yaw)
    {
        followTarget.rotation = Quaternion.Euler(pitch, yaw, followTarget.eulerAngles.z);
    }
    private float UpdateRotation(float currentRotation, float input, float min, float max, bool isXAxis)
    {
        currentRotation += isXAxis ? -input : input;
        currentRotation = Mathf.Clamp(currentRotation, min, max);
        return currentRotation;
    }
    private float GetMouseInput(string axis)
    {
        return Input.GetAxis(axis) * rotationalSpeed;
    }
}
