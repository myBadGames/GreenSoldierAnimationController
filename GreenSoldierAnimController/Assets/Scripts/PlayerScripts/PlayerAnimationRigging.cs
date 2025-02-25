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
    [SerializeField] private float reloadTime;
    [SerializeField] private float reloadDuration = 0.25f;
    [SerializeField] private float idleWeightClone;
    [SerializeField] private float aimWeightClone;

    [SerializeField] private float aimType;
    public float aimTypeSur;


    [SerializeField] private TwoBoneIKConstraint rightAimN;
    [SerializeField] private TwoBoneIKConstraint rightAimW;    
    
    [SerializeField] private TwoBoneIKConstraint leftAimN;
    [SerializeField] private TwoBoneIKConstraint leftAimW;

    [SerializeField] private float wickSpeed = 4000.0f;


    [SerializeField] private float aimTypeGoal;
    [SerializeField] private float wickChance = 55.0f;
    private float wickVelocity = 0;
    [SerializeField] private float wickSmooth = 0.05f;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        firstAim = false;
        idleWeightTarget = 1.01f;
        aimType = 1;
        aimTypeSur = 1000;
    }

    void Update()
    {
        if (firstAim)
        {
            idle.weight = idleWeight;
            aim.weight = aimWeight;

            if (!playerController.reload)
            {
                if (!playerController.aiming)
                {
                    idleWeightTarget = 1.01f;
                    aimWeightTarget = -0.01f;
                }
                else if (playerController.aiming)
                {
                    idleWeightTarget = -0.01f;
                    aimWeightTarget = 1.01f;
                }

                idleWeight = Mathf.Lerp(idleWeight, idleWeightTarget, Time.deltaTime * lerpSpeed);
                aimWeight = Mathf.Lerp(aimWeight, aimWeightTarget, Time.deltaTime * lerpSpeed);
                reloadTime = 0.0f;
                idleWeightClone = idleWeight;
                aimWeightClone = aimWeight;
            }
            else
            {
                if (reloadTime < reloadDuration)
                {
                    aimWeight = Mathf.Lerp(aimWeightClone, 0, reloadTime / reloadDuration);
                    idleWeight = Mathf.Lerp(idleWeightClone, 0, reloadTime / reloadDuration);
                    reloadTime += Time.deltaTime;
                }
                else
                {
                    aimWeight = 0;
                    idleWeight = 0;
                }
            }

            aimTypeSur = Mathf.Clamp(aimTypeSur, 0, 1000);

            if (playerController.wickChance <= wickChance)
            {
                aimTypeSur -= playerController.horizontalMouse * wickSpeed * Time.deltaTime;
            }
            else
            { aimTypeSur = 1000; }
            aimTypeGoal = Mathf.Clamp01(aimTypeGoal);
            aimType = Mathf.Clamp01(aimType);
            aimTypeGoal = aimTypeSur / 1000;
            aimType = Mathf.SmoothDamp(aimType, aimTypeGoal, ref wickVelocity, wickSmooth);

            rightAimN.weight = aimType;
            leftAimN.weight = rightAimN.weight;

            rightAimW.weight = -rightAimN.weight + 1;
            leftAimW.weight = -leftAimN.weight + 1;
        }
    }
}
