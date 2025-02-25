using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private PlayerAnimatorControl animatorControl;
    private PlayerAnimationRigging animationRigging;
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
    public float horizontalMouse;
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

    private LayerMask obstacleLayer = 1 << 6;
    [SerializeField] private bool vaulting;
    [SerializeField] private GameObject obstacle;
    private BoxCollider obstacleCollider;
    [SerializeField] private float distanceToObstacle;
    [SerializeField] private float distanceToObstacleLim;
    [SerializeField] private float distanceToVault;
    private float distanceToVaultMultiplier = 1.3f;
    [SerializeField] private bool vaultReady;
    [SerializeField] private float modelZ;
    private float modelZStart = -40.0f;
    [SerializeField] private float dot;
    [SerializeField] private float dot2;
    [SerializeField] private float vaultingY;
    [SerializeField] private bool obstacleInFront;
    [SerializeField] private CheckFront checkFront;
    [SerializeField] private Vector3 start;
    [SerializeField] private Vector3 destination;
    private Vector3 obstacleChildOffset = new Vector3(0, 0.6f, 0);

    public bool reload;
    [SerializeField] private float reloadTimer = 1.15f;
    public int bulletCount;
    public bool shootB;
    [SerializeField] private float recoil = 3.0f;

    [SerializeField] private Transform clips;
    [SerializeField] private float clipTime = 0.35f;

   public float wickChance;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animatorControl = GetComponent<PlayerAnimatorControl>();
        animationRigging = GetComponent<PlayerAnimationRigging>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
        compass = transform.GetChild(2);
        impulseSource = GetComponent<CinemachineImpulseSource>();
        colliderLerpTime = colliderLerpDuration;
        modelZ = 0;
        bulletCount = 8;
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
        Reload();
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
        if (Input.GetKey(KeyCode.LeftShift) && walking && !aiming && !vaulting)
        {
            running = true;
            crouch = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) || !walking)
        {
            running = false;
        }

        if (Input.GetKeyDown(KeyCode.C) && !running && !vaulting)
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
            StartCoroutine(VaultRoutineUp());
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

        if (!vaulting)
        {
            controller.Move(forwardDirection * verticalInput * movingSpeed * Time.deltaTime);
            controller.Move(strafeDirection * horizontalInput * movingSpeed * Time.deltaTime);
        }

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
        if (Input.GetButton("Fire2") && !running && !vaulting && !reload) 
        {
            aiming = true;
        } 
        if (Input.GetButtonDown("Fire2") && !running && !vaulting && !reload) 
        {
            wickChance = Random.Range(0, 100f);

            if (wickChance <= 60)
            {
                animationRigging.aimTypeSur = 1000;
            }

            if (!animationRigging.firstAim)
            {
                animationRigging.firstAim = true;
            }
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
            if (Input.GetButtonDown("Fire1") && Time.time > fireTime && bulletCount > 0 && !reload)
            {
                fireTime = Time.time + 1 / fireRate;
                shootB = true;
                targetX -= recoil;
                bulletCount--;

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

        if (shootB)
        {
            if (Time.time > fireTime)
            {
                shootB = false;
            }
        }
    }

    private void Reload()
    {
        if (Input.GetKeyDown(KeyCode.R) && bulletCount < 8 && !shootB && !reload) 
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    IEnumerator ReloadRoutine()
    {
        reload = true;
        yield return new WaitForSeconds(clipTime);
        for (int i = 0; i < clips.childCount - 1; i++)
        {
            if (!clips.GetChild(i).gameObject.activeInHierarchy)
            {
                clips.GetChild(i).GetComponent<Clip>().pushDirection = soldierModel.transform.right;
                clips.GetChild(i).gameObject.SetActive(true);
                break;
            }
        }
        yield return new WaitForSeconds(reloadTimer - clipTime);
        reload = false;
        bulletCount = 8;
    }

    private void ModelRotation()
    {
        if (!aiming)
        {
            if (!vaulting)
            {
                nonAimModelDirection = forwardDirection * verticalInput + strafeDirection * horizontalInput;

                if (nonAimModelDirection != Vector3.zero)
                {
                    modelAngles = Quaternion.LookRotation(nonAimModelDirection, Vector3.up);
                }
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

    IEnumerator VaultRoutineUp()
    {
        vault = true;
        vaulting = true;
        controller.excludeLayers = obsLayerSet.value;
        Vector3 trueStart = transform.position;
        Transform obstacleChild;

        if (dot > 0)
        {
            start = obstacle.transform.GetChild(obstacle.transform.childCount - 4).transform.position;
            obstacleChild = obstacle.transform.GetChild(obstacle.transform.childCount - 3).transform;

        }
        else
        {
            start = obstacle.transform.GetChild(obstacle.transform.childCount - 2).transform.position;
            obstacleChild = obstacle.transform.GetChild(obstacle.transform.childCount -1).transform;
        }

        obstacleChild.localPosition -= obstacleChildOffset;
        destination = obstacleChild.transform.position;

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
        StartCoroutine(VaultRoutineDown(obstacleChild));
    }

    private LayerMask obsLayerSet
    {
        get { return obstacleLayer; }
        set
        {
            obstacleLayer = value;
        }
    }

    IEnumerator VaultRoutineDown(Transform obstacleChild)
    {
        float timeE = 0;
        float duration = 1.11f - .3f;

        while (timeE < duration)
        {
            transform.position = Vector3.Lerp(start, destination, timeE / duration);
            transform.position = transform.position + new Vector3(0, vaultingY, 0);
            vaultingY = -5.2917f * (timeE * timeE) + 4.2483f * timeE - 0.006f; //y = -5.3333x2 + 4.3567x - 0.036 \\\\ y = -5.2917x2 + 4.2483x - 0.006
            modelZ = Mathf.Lerp(modelZStart, 0, timeE / duration);
            timeE += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        modelZ = 0;
        controller.excludeLayers = 0;
        vaulting = false;

        obstacleChild.localPosition += obstacleChildOffset;
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
            if (obstacle == null)
            {
                obstacle = hit.gameObject;
            }
        }
    }
}
