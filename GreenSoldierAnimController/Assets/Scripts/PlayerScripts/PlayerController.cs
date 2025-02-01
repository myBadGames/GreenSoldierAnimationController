using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Transform soldierModel;
    private Transform focalPoint;
    [SerializeField] private float verticalInput;
    [SerializeField] private float horizontalInput;
    [SerializeField] private float horizontalMouse;
    [SerializeField] private Vector3 forwardDirection;
    [SerializeField] private Vector3 strafeDirection;
    [SerializeField] private float movingSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float rotationSlerpSpeed;
    public bool aiming;
    public bool walking;
    public bool running;

    public float target;
    public float focalY;
    private float v = 0;
    private float smoothTime = 0.15f;
    private void Start()
    {
        controller = GetComponent<CharacterController>();
        soldierModel = transform.GetChild(0);
        focalPoint = transform.GetChild(1);
    }

    private void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        horizontalMouse = Input.GetAxisRaw("Mouse X");

        target += horizontalMouse * rotationSpeed * Time.deltaTime;
        focalY = Mathf.SmoothDampAngle(focalY, target,ref v, smoothTime);


        focalPoint.localRotation = Quaternion.Euler(0, focalY, 0);
    }
}
