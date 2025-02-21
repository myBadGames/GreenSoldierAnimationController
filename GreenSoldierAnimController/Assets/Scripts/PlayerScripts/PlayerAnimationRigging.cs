using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAnimationRigging : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Rig idle;
    [SerializeField] private float idleWeight;
    [SerializeField] private float idleWeightTarget;
    [SerializeField] private float lerpSpeed = 10.0f;
    [SerializeField] private Rig aim;
    [SerializeField] private float aimWeight;
    [SerializeField] private float aimWeightTarget;
    public bool firstAim;


    void Start()
    {
        playerController = GetComponent<PlayerController>();
        firstAim = false;
        idleWeightTarget = 1.01f;
    }

    void Update()
    {
        if (firstAim)
        {
            idle.weight = idleWeight;
            aim.weight = aimWeight;
            aimWeight = -idleWeight + 1;

            if (!playerController.aiming)
            {
                idleWeightTarget = 1.01f;
                aimWeightTarget = -0.01f;
            }
            else if (playerController.aiming)
            {
                aimWeightTarget = 1.01f;
                idleWeightTarget = -0.01f;
            }

            if (!playerController.reload)
            {
                idleWeight = Mathf.Lerp(idleWeight, idleWeightTarget, Time.deltaTime * lerpSpeed);
                aimWeight = Mathf.Lerp(aimWeight, aimWeightTarget, Time.deltaTime * lerpSpeed);
            }
            else
            {
                idleWeight = 0;
                aimWeight = 0;
            }
        }
    }
}
