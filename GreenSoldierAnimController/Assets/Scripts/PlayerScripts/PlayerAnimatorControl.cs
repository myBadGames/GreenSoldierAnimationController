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
    private string pistolFireStr  = "Weapons.PistolFire";

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
        animator.SetBool("Shoot_b", shootB);
        animator.SetBool("Aiming", aiming);

        if (playerController != null)
        {
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
            }
            else if (!playerController.aiming)
            { aiming = false; }
        }
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
}
