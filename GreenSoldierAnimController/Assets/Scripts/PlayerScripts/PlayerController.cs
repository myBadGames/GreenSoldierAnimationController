using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Transform soldierModel;
    public bool walking;
    public bool running;
    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;
    [SerializeField] private Vector3 forwardDirection;
    [SerializeField] private Vector3 strafeDirection;
    [SerializeField] private float movingSpeed = 2.5f;
    [SerializeField] private float walkingpeed = 2.5f;
    [SerializeField] private float runningSpeed = 5.0f;

    public bool aiming;

    [SerializeField] private float horizontalMouse;

    private Transform focalPoint;
    [SerializeField] private float rotationSpeed = 30.0f;
    [SerializeField] private float targetY;
    [SerializeField] private float focalY;
    private float v = 0;
    [SerializeField] private float smoothTime = 0.15f;

    [SerializeField] private Vector3 nonAimModelDirection;
    [SerializeField] private Quaternion lookDirection;
    [SerializeField] private float modelRotationSpeed = 2.5f;
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject aimCamera;
    public CinemachineImpulseSource impulseSource;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        RotateCamera();
        MovementInput();
        Movement();
        ArmControl();
        ModelRotation();
        CameraControl();
    }

    private void MovementInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        if (verticalInput != 0 || horizontalInput != 0)
        {
            walking = true;
        }
        else if (verticalInput != 0 && horizontalInput != 0)
        {
            walking = true;
        }
        else if (verticalInput == 0 && horizontalInput == 0)
        {
            walking = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && walking)
        {
            running = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || !walking)
        {
            running = false;
        }
    }

    private void Movement()
    {
        forwardDirection = focalPoint.forward.normalized;
        strafeDirection = focalPoint.right.normalized;

        controller.Move(forwardDirection * verticalInput * movingSpeed * Time.deltaTime);
        controller.Move(strafeDirection * horizontalInput * movingSpeed * Time.deltaTime);

        if (walking && !running)
        {
            movingSpeed = walkingpeed;
        }
        else if (walking && running)
        {
            movingSpeed = runningSpeed;
        }
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
        if (Input.GetButton("Fire2") && !running)
        {
            aiming = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            if (aiming)
            {
                aiming = false;
            }
        }

        if (aiming)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                impulseSource.GenerateImpulse(aimCamera.transform.forward);
            }
        }
    }

    private void ModelRotation()
    {
        if (!aiming)
        {
            nonAimModelDirection = forwardDirection * verticalInput + strafeDirection * horizontalInput;

            if (nonAimModelDirection != Vector3.zero)
            {
                lookDirection = Quaternion.LookRotation(nonAimModelDirection);
            }
        }
        else
        {
            lookDirection = Quaternion.LookRotation(forwardDirection);
        }

        soldierModel.localRotation = Quaternion.Slerp(soldierModel.localRotation, lookDirection, Time.deltaTime * modelRotationSpeed);
    }

    private void CameraControl()
    {
        if (!aiming)
        {
            mainCamera.SetActive(true);
            aimCamera.SetActive(false);
        }
        else
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(true);
        }
    }
}
