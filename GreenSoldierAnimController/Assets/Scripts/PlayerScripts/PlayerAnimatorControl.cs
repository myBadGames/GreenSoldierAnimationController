using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorControl : MonoBehaviour
{
    private PlayerController playerController;
    private Animator animator;
    [SerializeField] private float speedF;
    [SerializeField] private int weaponType = 1;
    public float speedTarget;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
        weaponType = 1;
    }

    // Update is called once per frame
    void Update()
    {
        AnimatorParameters();
    }

    private void AnimatorParameters()
    {
        animator.SetFloat("Speed_f", speedF);
        animator.SetInteger("WeaponType_int", weaponType);

        if (playerController != null)
        {
            if (playerController.walking)
            {
                speedF = 0.5f;
            }
            else
            {
                speedF = 0;
            }

            if (playerController.aiming)
            {
                weaponType = 2;

                if (Input.GetButtonDown("Fire1"))
                {
                    animator.SetTrigger("ShootTrig");
                }
            }
            else
            {
                weaponType = 1;
            }
        }
    }
}
