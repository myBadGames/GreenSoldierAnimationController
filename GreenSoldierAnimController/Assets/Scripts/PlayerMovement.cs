using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
   // private Rigidbody rb;
    [SerializeField] private Vector3 move;
    [SerializeField] private float speed = 10.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        Movement();
    }


    private void PlayerInput()
    {
        move = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }
    
    private void Movement()
    {
        if (move != Vector3.zero)
        {
            controller.Move(move * Time.deltaTime * speed);
        }
    }
}
