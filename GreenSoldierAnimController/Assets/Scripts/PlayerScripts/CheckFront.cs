using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFront : MonoBehaviour
{
    public bool occupied;
    [SerializeField] private Transform soldier;
    private Vector3 localOffset = new Vector3(0, 0.72f, 0.5f);

    void Update()
    {
        Quaternion yRotationOnly = Quaternion.Euler(0, soldier.eulerAngles.y, 0);

        transform.position = soldier.position + yRotationOnly * localOffset;

        transform.rotation = yRotationOnly;

    }
    private void OnTriggerEnter(Collider other)
    {
        occupied = true;
    }

    private void OnTriggerStay(Collider other)
    {
        occupied = true;
    }

    private void OnTriggerExit(Collider other)
    {
        occupied = false;
    }
}
