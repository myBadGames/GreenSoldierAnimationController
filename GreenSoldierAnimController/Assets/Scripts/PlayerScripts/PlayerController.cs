using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private PlayerAnimatorControl animatorControl;
    private Transform soldierModel;
    public bool walking;
    public bool running;
    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;
    private Transform compass;
    [SerializeField] private Vector3 focalEulerAngles;
    [SerializeField] private Vector3 forwardDirection;
    [SerializeField] private Vector3 strafeDirection;
    [SerializeField] private float movingSpeed = 2.5f;
    [SerializeField] private float walkingpeed = 2.5f;
    [SerializeField] private float runningSpeed = 5.0f;
    [SerializeField] private float crouchSpeed = 1.5f;

    public bool aiming;

    [SerializeField] private Quaternion newRotation;
    [SerializeField] private float horizontalMouse;
    [SerializeField] private float verticalMouse;

    private Transform focalPoint;
    [SerializeField] private float rotationSpeed = 30.0f;
    [SerializeField] private float targetY;
    public float targetX;
    [SerializeField] private float slerpSpeed = 5.0f;
    [SerializeField] private float xRotationLimit = 45.0f;

    [SerializeField] private Vector3 nonAimModelDirection;
    [SerializeField] private Quaternion modelDirection;
    [SerializeField] private float modelY;
    [SerializeField] private Vector3 modelEulerAngles;
    [SerializeField] private float modelRotationSpeed = 5.0f;

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private GameObject crouchCam;
    [SerializeField] private GameObject aimCamCrouch;
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private Vector3 impulseDirection;

    public float fireTime;
    [SerializeField] private float fireRate = 4.0f;

    public bool crouch;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animatorControl = GetComponent<PlayerAnimatorControl>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
        compass = transform.GetChild(2);
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        MovementInput();
        Movement();
        RotateCamera();
        CameraControl();
        AimControl();
        AttackControl();
        ModelRotation();
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

        if (Input.GetKey(KeyCode.LeftShift) && walking && !aiming)
        {
            running = true;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || !walking)
        {
            running = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && !running)
        {
            if (crouch)
            {
                crouch = false;
            }
            else
            {
                crouch = true;
            }
        }
    }

    private void Movement()
    {
        focalEulerAngles = focalPoint.localRotation.eulerAngles;
        compass.localRotation = Quaternion.Euler(0, focalEulerAngles.y, 0);

        forwardDirection = compass.forward.normalized;
        strafeDirection = compass.right.normalized;

        controller.Move(forwardDirection * verticalInput * movingSpeed * Time.deltaTime);
        controller.Move(strafeDirection * horizontalInput * movingSpeed * Time.deltaTime);

        if (walking && !running && !crouch)
        {
            movingSpeed = walkingpeed;
        }
        else if (walking && running && !crouch)
        {
            movingSpeed = runningSpeed;
        }
        else if (crouch)
        {
            movingSpeed = crouchSpeed;
        }
    }

    private void RotateCamera()
    {
        horizontalMouse = Input.GetAxisRaw("Mouse X");
        targetY += horizontalMouse * rotationSpeed * Time.deltaTime;

        verticalMouse = Input.GetAxis("Mouse Y");
        targetX = Mathf.Clamp(targetX, -xRotationLimit, xRotationLimit);
        targetX += -verticalMouse * rotationSpeed / 2 * Time.deltaTime;

        newRotation = Quaternion.Euler(targetX, targetY, 0);

        focalPoint.localRotation = Quaternion.Slerp(focalPoint.localRotation, newRotation, Time.deltaTime * slerpSpeed);
    }

    private void CameraControl()
    {
        if (!crouch)
        {
            crouchCam.SetActive(false);
            aimCamCrouch.SetActive(false);

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
        else
        {
            mainCamera.SetActive(false);
            aimCamera.SetActive(false);

            if (!aiming)
            {
                crouchCam.SetActive(true);
                aimCamCrouch.SetActive(false);
            }
            else
            {
                crouchCam.SetActive(false);
                aimCamCrouch.SetActive(true);
            }
        }
    }

    private void AimControl()
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
    }    
    
    private void AttackControl()
    {
        if (aiming)
        {
            if (Input.GetButtonDown("Fire1") && Time.time > fireTime)
            {
                fireTime = Time.time + 1 / fireRate;
                animatorControl.ShootTrig();

                if (forwardDirection.z >= 0.95f || forwardDirection.z <= -0.95) 
                {
                    impulseDirection = forwardDirection;
                }
                else if (strafeDirection.z >= 0.95f || strafeDirection.z <= -0.95f)
                {
                    impulseDirection = strafeDirection;
                }

                impulseSource.GenerateImpulse(impulseDirection);
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
                modelDirection = Quaternion.LookRotation(nonAimModelDirection, Vector3.up);
            }
        }
        else
        {
            modelDirection = Quaternion.LookRotation(forwardDirection, Vector3.up);
        }

        modelEulerAngles = modelDirection.eulerAngles;
        modelY = Mathf.LerpAngle(modelY, modelEulerAngles.y, Time.deltaTime * modelRotationSpeed);
        soldierModel.localRotation = Quaternion.Euler(0, modelY, 0);
    }
}
