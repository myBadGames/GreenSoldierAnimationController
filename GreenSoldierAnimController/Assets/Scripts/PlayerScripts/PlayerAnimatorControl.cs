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

    [SerializeField] private float bodyVertical;
    [SerializeField] private float bodyVerticalTarget;
    private float bodyVerticalVelocity;
    private float smoothTime = 0.05f;

    [SerializeField] private float bodyHorizontal;
    [SerializeField] private float bodyHorizontalTarget;
    private float bodyHorizontalVelocity;

    [SerializeField] private float headHorizontal;
    [SerializeField] private float headHorizontalTarget;
    private float headHorizontalVelocity;

    [SerializeField] private bool crouchB;
    [SerializeField] private bool vault;


    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        bodyVertical = 0.0f;
        bodyVerticalTarget = 0.0f;
        bodyHorizontal = 0.0f;
        bodyHorizontalTarget = 0.0f;
        headHorizontal = 0.0f;
        headHorizontalTarget = 0.0f;
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
        animator.SetFloat("Body_Horizontal_f", bodyHorizontal);
        animator.SetFloat("Head_Horizontal_f", headHorizontal);

        shootB = playerController.shootB;

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
            bodyHorizontalTarget = 0.101f;
            headHorizontalTarget = 0.075f;
        }
        else if (!playerController.aiming)
        {
            aiming = false;
            bodyVerticalTarget = 0;
            bodyHorizontalTarget = 0;
            headHorizontalTarget = 0;
        }

        bodyVertical = Mathf.SmoothDamp(bodyVertical, bodyVerticalTarget, ref bodyVerticalVelocity, smoothTime);
        bodyHorizontal = Mathf.SmoothDamp(bodyHorizontal, bodyHorizontalTarget, ref bodyHorizontalVelocity, smoothTime);
        headHorizontal = Mathf.SmoothDamp(headHorizontal, headHorizontalTarget, ref headHorizontalVelocity, smoothTime);

        animator.SetBool("Crouch_b", crouchB);

        crouchB = playerController.crouch;

        animator.SetBool("Vault", vault);

        vault = playerController.vault;

    }
}
