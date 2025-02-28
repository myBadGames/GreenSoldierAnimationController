using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private Animation anim;
    [SerializeField] private GameObject realModel;
    [SerializeField] private GameObject fakeModel;
    [SerializeField] private Transform realSpine;
    [SerializeField] private Transform fakeSpine;
    [SerializeField] private Transform[] realLegL;
    [SerializeField] private Transform[] realLegR;
    [SerializeField] private Transform[] fakeLegL;
    [SerializeField] private Transform[] fakeLegR;
    [SerializeField] private float delay;
    [SerializeField] private float clipSpeed;

    void Start()
    {
        anim = GetComponentInChildren<Animation>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.Play("Soldier_Walk");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            anim.Play("A_shoot");
            StartCoroutine(Shoot());
        }

        anim["A_shoot"].speed = clipSpeed;

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(DeathUchiMata());
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            CopyTransformSpine(realSpine, fakeSpine);
            CopyTransformList(realLegL, fakeLegL);
            CopyTransformList(realLegR, fakeLegR);
            fakeModel.SetActive(true);
            realModel.SetActive(false);
        }    
        
        if (Input.GetKeyDown(KeyCode.J))
        {
            fakeModel.transform.localPosition = Vector3.zero;
            fakeModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            fakeModel.SetActive(false);
            realModel.SetActive(true);
        }
    }

    private void CopyTransformSpine(Transform sourceTransform, Transform destinationTransform)
    {
        for (int i = 0; i < sourceTransform.childCount; i++)
        {
            var source = sourceTransform.GetChild(i);
            var destination = destinationTransform.GetChild(i);
            destination.position = source.position;
            destination.rotation = source.rotation;
            CopyTransformSpine(source, destination);
        }
    }

    private void CopyTransformList(Transform[] sourceTransform, Transform[] destinationTransform)
    {
        for (int i = 0; i < sourceTransform.Length - 1; i++)
        {
            destinationTransform[i].position = sourceTransform[i].position;
        }
    }

    IEnumerator DeathUchiMata()
    {
        anim.Play("B_grenade");
        yield return new WaitForSeconds(delay);
        anim.Stop();
        CopyTransformSpine(realSpine, fakeSpine);
        CopyTransformList(realLegL, fakeLegL);
        CopyTransformList(realLegR, fakeLegR);
        fakeModel.SetActive(true);
        realModel.SetActive(false);
    }

    [SerializeField] private GameObject clip;

    IEnumerator Shoot()
    {
        while (anim["A_shoot"] == anim.clip)
        {
            yield return new WaitForSeconds(.41515f);
            Debug.Log("hit");
        }
    }
}
