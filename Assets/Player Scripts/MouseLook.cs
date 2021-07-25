using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Player")]
    public Transform Player;
    public Vector3 CamOffset;

    [Header("Camera")]
    public float Sensitivity;
    public float ClampAngle;

    float rotY; // rotation around the up/y axis
    float rotX; // rotation around the right/x axis

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Look();
        RotatePlayer();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = -Input.GetAxisRaw("Mouse Y");

        rotY += mouseX * Sensitivity;
        rotX += mouseY * Sensitivity;

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);

        transform.rotation = Quaternion.Euler(rotX, rotY, 0.0f);
    }

    void RotatePlayer()
    {
        Player.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        transform.position = Player.transform.position + CamOffset;
    }
}
