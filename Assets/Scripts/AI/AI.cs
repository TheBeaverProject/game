using System;
using System.Collections;
using System.Collections.Generic;
using Guns;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using PlayerManagement;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

public class AI : MonoBehaviour
{
    public enum State
    {
        Patrol,
        Follow,
        Attack,
        Dead
    }

    public State state;
    
    [SerializeField]
    public GameObject Destinations;

    public Transform[] dest;

    public NavMeshAgent agent;

    private Random _random = new Random();

    public PlayerManager PlayerManager;

    private float shootDistance = 20f;

    private float maxFollowDistance = 30f;

    public GameObject weapon;

    private bool inSight;

    private Transform target;

    private Vector3 directionToTarget;
    
    
    private void Start()
    {
        PlayerManager = GetComponent<PlayerManager>();
        dest = Destinations.GetComponentsInChildren<Transform>();
        state = State.Patrol;
    }
    
    private bool initialized;
    void Update()
    {
        if (!initialized && PhotonNetwork.IsConnectedAndReady)
        {
            PlayerManager.AddGunPrefabToPlayer(weapon);
            initialized = true;
        }

        if (PlayerManager.Health <= 0)
        {
            state = State.Dead;
            PlayerManager.EnterSpecMode();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForPlayer();
        switch (state)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Follow:
                Follow();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    void Patrol()
    {
        if (!agent.hasPath)
        {
            int choice = _random.Next(dest.Length);
            agent.SetDestination(dest[choice].position);
        }

        if (inSight)
        {
            state = State.Follow;
        }
    }

    void CheckForPlayer()
    {
        Collider[] allPlayers = Physics.OverlapSphere(transform.position, 70, LayerMask.GetMask("playerMain"));
        List<Transform> ennemies = new List<Transform>();
        foreach (Collider col in allPlayers)
        {
            
            
                ennemies.Add(col.transform);
            
        }

        List<Transform> inView = new List<Transform>();
        foreach (Transform ennemy in ennemies)
        {
            directionToTarget = ennemy.position - transform.position;

            RaycastHit rayHit;
            PlayerManager player;
            if (Physics.Raycast(transform.position, directionToTarget.normalized, out rayHit))
            {
                inSight = rayHit.collider.TryGetComponent<PlayerManager>(out player);
                inView.Add(ennemy);
            }
        }
        
        foreach (Transform ennemy in inView)
        {
            if (target == null || Mathf.Abs((ennemy.position - transform.position).magnitude) <
                Mathf.Abs((target.position - transform.position).magnitude))
            {
                target = ennemy;
            }
        }
    }

    void Follow()
    {
        if (directionToTarget.magnitude <= shootDistance && inSight)
        {
            agent.ResetPath();
            state = State.Attack;
        }
        else
        {
            if (target != null)
            {
                agent.SetDestination(target.position);
            }

            if (directionToTarget.magnitude > maxFollowDistance)
            {
                state = State.Patrol;
            }
        }
    }

    void Attack()
    {
        if (!inSight || directionToTarget.magnitude > shootDistance)
        {
            state = State.Follow;
        }
        LookTarget();
        
        PlayerManager.playerWeapon.GetComponent<Gunnable>().AIShooting = true;
    }

    void LookTarget()
    {
        Vector3 lookDirection = directionToTarget;
        lookDirection.y = 0f;
        
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation,Time.deltaTime*agent.angularSpeed);
        
    }
    
}
