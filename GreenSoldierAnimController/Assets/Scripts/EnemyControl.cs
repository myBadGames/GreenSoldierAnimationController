using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    private CharacterController controller;
    private GameObject player;
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
    [SerializeField] private float walkAnimSpeed = 1.25f;
    [SerializeField] private float shootAnimSpeed = 2.0f;
    [SerializeField] private float reloadAnimSpeed = 1.25f;
    [SerializeField] private string animName;

    [SerializeField] private bool walking;
    [SerializeField] private bool shooting;

    [SerializeField] private float distanceToPlayer;
    [SerializeField] private float distanceToPlayerLimit = 25.0f;
    [SerializeField] private bool playerClose;
    [SerializeField] private GameObject playerTracker;
    [SerializeField] private Vector3 playerPos;
    [SerializeField] private float playerPosZLimit = -1.0f;
    [SerializeField] private Vector3 playerDirection;
    [SerializeField] private bool playerSpotted;
    [SerializeField] private Quaternion attackRotation;
    [SerializeField] private float slerpSpeed = 20.0f;
    private Vector3 offsetEyes = new Vector3(0, 1.5f, 0);

    [SerializeField] private float movementSpeed;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animation>();
        player = GameObject.FindGameObjectWithTag("Player");
        walking = true;
        shooting = false;
    }

    void Update()
    {
        Movement();
        AnimControls();
        SearchPlayerLogic();
        TestControls();
    }

    private void FixedUpdate()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
    }

    void Movement()
    {
        if (walking && !playerSpotted)
        {
            movementSpeed = 0;
        }
        else if (walking && playerSpotted)
        {
            movementSpeed = 100;
        }
        else if (shooting && !walking)
        {
            movementSpeed = 0;
        }

        controller.SimpleMove(transform.forward * movementSpeed * Time.deltaTime);
    }

    void SearchPlayerLogic()
    {
        if (distanceToPlayer <= distanceToPlayerLimit)
        {
            playerClose = true;
        }
        else
        {
            playerClose = false;
        }

        if (playerClose)
        {
            playerTracker.transform.position = player.transform.position;
            playerPos = playerTracker.transform.localPosition;
            playerDirection = (player.transform.position - transform.position).normalized;
        }

        if (!playerSpotted)
        {
            if (playerClose)
            {
                if (playerPos.z > playerPosZLimit)
                {
                    if (IsPlayerSeen())
                    {
                        playerSpotted = true;
                    }
                }
            }
        }
        else 
        {
            if (playerClose)
            {
                attackRotation = Quaternion.LookRotation(playerDirection, Vector3.up);

                if (playerPos.z > playerPosZLimit)
                {
                    if (IsPlayerSeen())
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, attackRotation, Time.deltaTime * slerpSpeed);
                        shooting = true;
                        walking = false;
                    }
                    else
                    {
                        shooting = false;
                        walking = true;
                    }                         
                }
            }
            else
            {
                shooting = false;
                walking = true;
            }
        }
    }

    private bool IsPlayerSeen()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + offsetEyes, playerDirection * distanceToPlayerLimit, out hit))
        {
            if (hit.collider.gameObject == player)
            {
                Debug.DrawRay(transform.position +offsetEyes, playerDirection * distanceToPlayerLimit, Color.green);
                return true;
            }
            else
            {
                Debug.DrawRay(transform.position + offsetEyes, playerDirection * distanceToPlayerLimit, Color.blue);
                return false;
            }
        }
        else
        {
            Debug.DrawRay(transform.position + offsetEyes, playerDirection * distanceToPlayerLimit, Color.yellow);
            return false;
        }
    }

    void AnimControls()
    {
        anim["Soldier_Walk"].speed = walkAnimSpeed;
        anim["A_shoot"].speed = shootAnimSpeed;
        anim["A_reloading"].speed = reloadAnimSpeed;

        if (walking && !shooting)
        {
            if (!anim.IsPlaying("Soldier_Walk"))
            {
                anim.Play("Soldier_Walk");
            }
        }
        else if (shooting && !walking)
        {
            if (!anim.IsPlaying("A_shoot"))
            {
                anim.Play("A_shoot");
            }
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



    void TestControls()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            anim.Play(animName);
        }

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
}
