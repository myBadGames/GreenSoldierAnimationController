using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clip : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float force;
    [SerializeField] private float spin;
    [SerializeField] private Transform father;
    [SerializeField] private float timer = 3.0f;
    public Vector3 pushDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        transform.SetParent(null);
        rb.AddForce(pushDirection * force, ForceMode.Impulse);
        rb.AddTorque(pushDirection * spin, ForceMode.Impulse);
        Invoke("Revert", timer);
    }

    void Revert()
    {
        transform.SetParent(father);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, -90, 0);
        gameObject.SetActive(false);
    }
}
