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
    [SerializeField] private Quaternion modelAngles;
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

    public bool vault;
    [SerializeField] private float vaultTime;
    [SerializeField] private float vaultRate = 1.2f;

    private Vector3 modelPosVelocity = Vector3.zero;
    [SerializeField] private float modelPosSmoothTime = 0.15f;

    private Vector3 standingCenter = new Vector3(0, 0.87f, 0);
    private float standingHeight = 1.75f;
    private Vector3 crouchingCenter = new Vector3(0, 0.62f, 0);
    private float crouchingHeight = 1.25f;
    private float colliderLerpDuration = 0.1123f;
    [SerializeField] private float colliderLerpTime;



    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animatorControl = GetComponent<PlayerAnimatorControl>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
        compass = transform.GetChild(2);
        impulseSource = GetComponent<CinemachineImpulseSource>();
        colliderLerpTime = colliderLerpDuration;
        modelZ = 0;
    }

    private void Update()
    {
        MovementInput();
        Movement();
        RotateCamera();
        CameraControl();
        ColliderControl();
        AimControl();
        AttackControl();
        ModelRotation();
        ModelPosition();
        ObstacleCollision();
    }

    private void MovementInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        float verticalRaw = Input.GetAxisRaw("Vertical");
        float horizontalRaw = Input.GetAxisRaw("Horizontal");

        if (verticalRaw != 0 || horizontalRaw != 0)
        {
            walking = true;
        }
        else if (verticalRaw != 0 && horizontalRaw != 0)
        {
            walking = true;
        }
        else if (verticalRaw == 0 && horizontalRaw == 0)
        {
            walking = false;
        }
        if (Input.GetKey(KeyCode.LeftShift) && walking && !aiming)
        {
            running = true;
            crouch = false;
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
                colliderLerpTime = 0;
            }
            else
            {
                crouch = true;
                colliderLerpTime = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > vaultTime && !aiming && obstacle != null && vaultReady && obstacleInFront)
        {
            vaultTime = Time.time + 1 / vaultRate;
            StartCoroutine(VaultRoutineUp(walking, running));
        }

        if (vault)
        {
            if (Time.time > vaultTime)
            {
                vault = false;
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

        if (walking && !running && !crouch && !vaulting)
        {
            movingSpeed = walkingpeed;
        }
        else if (walking && running && !crouch && !vaulting)
        {
            movingSpeed = runningSpeed;
        }
        else if (crouch && !running && !vaulting)
        {
            movingSpeed = crouchSpeed;
        }
        else if (vaulting)
        {
            movingSpeed = 0;
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
                modelAngles = Quaternion.LookRotation(nonAimModelDirection, Vector3.up);
            }
        }
        else
        {
            modelAngles = Quaternion.LookRotation(forwardDirection, Vector3.up);
        }

        modelEulerAngles = modelAngles.eulerAngles;
        modelY = Mathf.LerpAngle(modelY, modelEulerAngles.y, Time.deltaTime * modelRotationSpeed);
        soldierModel.localRotation = Quaternion.Euler(0, modelY, modelZ);
    }

    private void ModelPosition()
    {
        if (!vault && !vaulting)
        {
            if (soldierModel.localPosition != Vector3.zero)
            {
                soldierModel.localPosition = Vector3.SmoothDamp(soldierModel.localPosition, Vector3.zero, ref modelPosVelocity, modelPosSmoothTime);
            }
        }
    }

    private void ColliderControl()
    {
        if (!crouch)
        {
            if (colliderLerpTime < colliderLerpDuration)
            {
                controller.center = Vector3.Lerp(crouchingCenter, standingCenter, colliderLerpTime / colliderLerpDuration);
                controller.height = Mathf.Lerp(crouchingHeight, standingHeight, colliderLerpTime / colliderLerpDuration);
                colliderLerpTime += Time.deltaTime;
            }
            else
            {
                controller.center = standingCenter;
                controller.height = standingHeight;
            }
        }
        else
        {
            if (colliderLerpTime < colliderLerpDuration)
            {
                controller.center = Vector3.Lerp(standingCenter, crouchingCenter, colliderLerpTime / colliderLerpDuration);
                controller.height = Mathf.Lerp(standingHeight, crouchingHeight, colliderLerpTime / colliderLerpDuration);
                colliderLerpTime += Time.deltaTime;
            }
            else
            {
                controller.center = crouchingCenter;
                controller.height = crouchingHeight;
            }
        }
    }

    IEnumerator VaultRoutineUp(bool walking, bool running)
    {
        vault = true;
        vaulting = true;
        controller.excludeLayers = obsLayerSet.value;
        Vector3 trueStart = transform.position;

        if (dot > 0)
        {
            directionMultiplier = 1;
        }
        else
        {
            directionMultiplier = -1;
        }

        Vector3 start = new Vector3(obstacle.transform.position.x + .58f * - directionMultiplier, transform.position.y, obstacle.transform.position.z - 1.1f * directionMultiplier);
        Vector3 destination = new Vector3(obstacle.transform.position.x - .58f , transform.position.y, obstacle.transform.position.z + 1.1f * directionMultiplier);

        Debug.Log(start);
        Debug.Log(destination);

        float timeE = 0;
        float duration = 0.3f;
        while (timeE < duration)
        {
            transform.position = Vector3.Lerp(trueStart, start, timeE / duration);
            modelZ = Mathf.Lerp(0, modelZStart, timeE / duration);
            timeE += Time.deltaTime;
            yield return null;
        }

        transform.position = start;
        modelZ = modelZStart;
        StartCoroutine(VaultRoutineDown(walking, running, start, destination));
    }

    private LayerMask obsLayerSet
    {
        get { return obstacleLayer; }
        set
        {
            obstacleLayer = value;
        }
    }

    IEnumerator VaultRoutineDown(bool walking, bool running, Vector3 start, Vector3 destination)
    {
        float timeE = 0;
        float duration;
        if (walking && !running)
        {
            duration = 1.15f - 0.3f;
        }
        else if (running && walking)
        {
            duration = 1.08f - 0.3f;
        }
        else
        {
            duration = 1.15f - 0.3f;
        }

        while (timeE < duration)
        {
            transform.position = Vector3.Lerp(start, destination, timeE / duration);
            transform.position = transform.position + new Vector3(0, vaultingY, 0);
            float x = transform.position.z - obstacle.transform.position.z;
            vaultingY = -0.7936f * Mathf.Pow(x, 2) - 0.0482f * (x) + 0.8886f;
            modelZ = Mathf.Lerp(modelZStart, 0, timeE / duration);
            timeE += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        modelZ = 0;
        controller.excludeLayers = 0;
        vaulting = false;
    }

    private void ObstacleCollision()
    {
        if (obstacle != null)
        {
            obstacleCollider = obstacle.GetComponent<BoxCollider>();

            obstacleInFront = checkFront.occupied;

            if (obstacleCollider.size.x > obstacleCollider.size.z)
            {
                distanceToObstacleLim = obstacleCollider.size.x * 0.77f;
            }
            else
            {
                distanceToObstacleLim = obstacleCollider.size.z * 0.77f;
            }

            distanceToVault = obstacleCollider.size.z * distanceToVaultMultiplier;

            dot = Vector3.Dot(obstacle.transform.forward, soldierModel.transform.forward);
            dot2 = Vector3.Dot(-obstacle.transform.forward, soldierModel.transform.forward);

            if (distanceToObstacle <= distanceToVault && dot == -dot2)
            {
                vaultReady = true;
            }
            else
            {
                vaultReady = false;
            }

            distanceToObstacle = Vector3.Distance(transform.position - new Vector3(0, transform.position.y, 0), obstacle.transform.position - new Vector3(0, obstacle.transform.position.y, 0));

            if (distanceToObstacle >= distanceToObstacleLim && !vaulting)
            {
                obstacle = null;
            }
        }
        else
        {
            distanceToObstacle = 0;
            obstacleCollider = null;
            distanceToObstacleLim = 0;
            vaultReady = false;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Obstacles"))
        {
            obstacle = hit.gameObject;
        }
    }

    private LayerMask obstacleLayer = 1 << 6;
    [SerializeField] private bool vaulting;
    [SerializeField] private GameObject obstacle;
    [SerializeField] private BoxCollider obstacleCollider;
    [SerializeField] private float distanceToObstacle;
    [SerializeField] private float distanceToObstacleLim;
    [SerializeField] private float distanceToVault;
    [SerializeField] private float distanceToVaultMultiplier = 1.3f;
    [SerializeField] private bool vaultReady;
    [SerializeField] private float modelZ;
    [SerializeField] private float modelZStart = -40.0f;
    [SerializeField] private float dot;
    [SerializeField] private float dot2;
    [SerializeField] private float vaultingY;
    [SerializeField] private float randomMultiplier;
    [SerializeField] private float directionMultiplier;
    [SerializeField] private bool obstacleInFront;
    [SerializeField] private CheckFront checkFront;
}
