using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    [SerializeField] private float speedF;
    public float speedTarget;
    [SerializeField] private bool aiming;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        AnimatorParameters();
    }

    private void AnimatorParameters()
    {
        animator.SetFloat("Speed_f", speedF);
        animator.SetBool("Aiming", aiming);

        if (playerController != null)
        {
            if (playerController.walking && !playerController.running)
            {
                speedF = 0.5f;
            }
            else if (playerController.walking && playerController.running)
            {
                speedF = 1.0f;
            }

            else if (!playerController.walking && !playerController.running)
            {
                speedF = 0.0f;
            }

            if (playerController.aiming)
            {
                if (!aiming)
                {
                    aiming = true;
                }

                if (Input.GetButtonDown("Fire1"))
                {
                    animator.SetTrigger("ShootTrig");
                }
            }
            else if (!playerController.aiming)
            { aiming = false; }
        }
    }
}
