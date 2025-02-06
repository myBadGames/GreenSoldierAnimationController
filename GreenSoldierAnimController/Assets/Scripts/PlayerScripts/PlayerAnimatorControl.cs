using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    [SerializeField] private float speedF;
    public bool shootB;
    [SerializeField] private bool aiming;
    private string pistolFireStr = "Weapons.PistolFire";
    [SerializeField] private float bodyVertical;
    [SerializeField] private float bodyVerticalTarget;
    private float bodyVerticalVelocity;
    [SerializeField] private float bodyVerticalSmoothTime = 0.05f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        bodyVertical = 0.0f;
    }

    void Update()
    {
        AnimatorParameters();
    }

    private void AnimatorParameters()
    {
        animator.SetFloat("Speed_f", speedF);
        animator.SetBool("Shoot_b", shootB);
        animator.SetBool("Aiming", aiming);
        animator.SetFloat("Body_Vertical_f", bodyVertical);


        if (playerController.walking && !playerController.running)
        {
            speedF = 0.49f;
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

            bodyVerticalTarget = Mathf.Clamp(bodyVerticalTarget, -1.0f, 1.0f);
            bodyVerticalTarget = -0.02222f * playerController.targetX;
        }
        else if (!playerController.aiming)
        {
            aiming = false;
            bodyVerticalTarget = 0;
        }

        bodyVertical = Mathf.SmoothDamp(bodyVertical, bodyVerticalTarget, ref bodyVerticalVelocity, bodyVerticalSmoothTime);
        CrouchingTiger();
    }

    public void ShootTrig()
    {
        if (!shootB)
        {
            shootB = true;
        }
        else
        {
            animator.Play(pistolFireStr, 5, 0);
        }
    }

    [SerializeField] private bool crouchB;

    void CrouchingTiger()
    {
        animator.SetBool("Crouch_b", crouchB);

        crouchB = playerController.crouch;
    }
}
