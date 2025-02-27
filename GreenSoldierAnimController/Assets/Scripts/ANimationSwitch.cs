using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANimationSwitch : MonoBehaviour
{
    public GameObject legacyModel;
    public GameObject animatorModel;
    public float blendDuration = 0.5f; // Duration for crossfade

    // Assuming both models have the same bone hierarchy:
    private Dictionary<Transform, Transform> boneMap;

    void Start()
    {
        // Ensure legacyModel is active and animatorModel is initially inactive.
        legacyModel.SetActive(true);
        animatorModel.SetActive(false);

        // Setup boneMap if both models share similar bone names or structure.
        boneMap = CreateBoneMap(legacyModel.transform, animatorModel.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            SwitchToAnimatorModel();
        }
    }

    // Trigger this function when you want to switch models (e.g., on death).
    public void SwitchToAnimatorModel()
    {
        // Enable animator model.

        // Copy the current pose from legacyModel to animatorModel.
        CopyPose(boneMap);
        legacyModel.SetActive(false);
        animatorModel.SetActive(true);        // Start crossfade blending.
        //  StartCoroutine(CrossFadeModels());

        // Trigger the animator animation.
        // Animator anim = animatorModel.GetComponent<Animator>();
        //  if (anim != null)
        //     {
        //         anim.Play(animatorStateName, 0, 0f); // Optionally adjust normalized time.
        //     }
    }

    // Creates a mapping between corresponding bones in both models.
    Dictionary<Transform, Transform> CreateBoneMap(Transform legacyRoot, Transform animatorRoot)
    {
        Dictionary<Transform, Transform> map = new Dictionary<Transform, Transform>();
        // Assuming bones share the same names:
        foreach (Transform legacyBone in legacyRoot.GetComponentsInChildren<Transform>())
        {
            Transform matchingBone = FindChildByName(animatorRoot, legacyBone.name);
            if (matchingBone != null)
                map.Add(legacyBone, matchingBone);
        }
        return map;
    }

    // Helper: Finds a child transform by name.
    Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.name == name)
                return child;
        }
        return null;
    }

    // Copies transform data from the legacy model to the animator model.
    void CopyPose(Dictionary<Transform, Transform> boneMap)
    {
        foreach (var pair in boneMap)
        {
            Transform legacyBone = pair.Key;
            Transform animatorBone = pair.Value;
            animatorBone.position = legacyBone.position;
            animatorBone.rotation = legacyBone.rotation;
          //  animatorBone.localScale = legacyBone.localScale;
        }
    }

    // Crossfade blending between models.
    IEnumerator CrossFadeModels()
    {
        float timer = 0f;
        // Assume you have materials set up to control opacity or use shader parameters.
        // For simplicity, here we just assume both models have a CanvasGroup-like component.
        CanvasGroup legacyGroup = legacyModel.GetComponent<CanvasGroup>();
        CanvasGroup animatorGroup = animatorModel.GetComponent<CanvasGroup>();

        if (legacyGroup == null || animatorGroup == null)
        {
            // If not, simply wait for the blend duration.
            yield return new WaitForSeconds(blendDuration);
        }
        else
        {
            // Initialize animator to fully opaque, legacy to fully opaque.
            legacyGroup.alpha = 1f;
            animatorGroup.alpha = 0f;

            while (timer < blendDuration)
            {
                float t = timer / blendDuration;
                legacyGroup.alpha = 1f - t;
                animatorGroup.alpha = t;
                timer += Time.deltaTime;
                yield return null;
            }
            legacyGroup.alpha = 0f;
            animatorGroup.alpha = 1f;
        }

        // Finally, disable the legacy model.
        legacyModel.SetActive(false);
        animatorModel.SetActive(true);

    }
}
