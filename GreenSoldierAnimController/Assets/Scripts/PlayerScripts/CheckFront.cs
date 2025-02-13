using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckFront : MonoBehaviour
{
    public bool occupied;
    [SerializeField] private Transform soldier;

    private void Update()
    {
        transform.position = soldier.transform.position; 
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
