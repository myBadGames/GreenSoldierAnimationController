using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    public Transform soldierTransform;
    [SerializeField] private Vector3 move;
    [SerializeField] private float speed = 1.0f;
    public bool aiming;
    public bool walking;
    public bool running;

    [SerializeField] private Quaternion targetRotation;
    private Quaternion fwRotation = Quaternion.Euler(0, 0, 0);
    private Quaternion bkRotation = Quaternion.Euler(0, 180, 0);
    private Quaternion lRotation = Quaternion.Euler(0, -90, 0);
    private Quaternion rRotation = Quaternion.Euler(0, 90, 0);   
    private Quaternion fwLRotation = Quaternion.Euler(0, -45, 0);
    private Quaternion fwRRotation = Quaternion.Euler(0, 45, 0);
    private Quaternion bkLRotation = Quaternion.Euler(0, -135, 0);
    private Quaternion bkRRotation = Quaternion.Euler(0, 135, 0);
    [SerializeField] private float rotationSpeedSoldier = 2.5f;

    [SerializeField] private Quaternion lookDirection;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        soldierTransform = transform.GetChild(0);
    }

    void Update()
    {
        PlayerInput();
        Movement();
        Conditions();
        SoldierRotation();
    }

    private void PlayerInput()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Input.GetButton("Fire2"))
        {
            aiming = true;
        }
        if (Input.GetButtonUp("Fire2"))
        {
            aiming = false;
        }
    }

    private void Movement()
    {
        if (move != Vector3.zero)
        {
            if (!aiming)
            {
                controller.Move(move * Time.deltaTime * speed);
            }
        }
    }

    private void Conditions()
    {
        if (move != Vector3.zero)
        {
            walking = true;
        }
        else
        {
            walking = false;
        }
    }

    private void SoldierRotation()
    {
        if (move != Vector3.zero)
        {
            lookDirection = Quaternion.LookRotation(move);

            soldierTransform.localRotation = Quaternion.Slerp(soldierTransform.localRotation, lookDirection, Time.deltaTime * rotationSpeedSoldier);
        }
    }
}
