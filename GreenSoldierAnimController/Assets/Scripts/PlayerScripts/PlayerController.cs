using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Transform soldierModel;
    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;
    [SerializeField] private Vector3 forwardDirection;
    [SerializeField] private Vector3 strafeDirection;
    [SerializeField] private float movingSpeed = 2.5f;

    public bool aiming;
    public bool walking;
    public bool running;

    [SerializeField] private float horizontalMouse;

    private Transform focalPoint;
    [SerializeField] private float rotationSpeed = 15.0f;
    [SerializeField] private float targetY;
    [SerializeField] private float focalY;
    private float v = 0;
    [SerializeField] private float smoothTime = 0.15f;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
    }

    private void Update()
    {
        RotateCamera();
        Movement();
        ArmControl();
    }

    private void Movement()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        forwardDirection = focalPoint.forward.normalized;
        strafeDirection = focalPoint.right.normalized;

        controller.Move(forwardDirection * verticalInput * movingSpeed * Time.deltaTime);
        controller.Move(strafeDirection * horizontalInput * movingSpeed * Time.deltaTime);
    }

    private void RotateCamera()
    {
        horizontalMouse = Input.GetAxisRaw("Mouse X");
        targetY += horizontalMouse * rotationSpeed * Time.deltaTime;
        focalY = Mathf.SmoothDampAngle(focalY, targetY, ref v, smoothTime);
        focalPoint.localRotation = Quaternion.Euler(0, focalY, 0);
    }

    private void ArmControl()
    {
        if (Input.GetButton("Fire2"))
        {
            aiming = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            aiming = false;
        }
    }
}
