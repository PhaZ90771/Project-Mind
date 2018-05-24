using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionController : MonoBehaviour
{
    private const float MOVEMENT_FORCE = 1f;

    public enum ACTION
    {
        Jump,
        MoveLeft,
        MoveRight,
        MoveForward,
        MoveBackward,
        MoveVector,
        Projection,
        Walk,
        Run
    }

    [SerializeField]
    private Rigidbody body;
    
    private Projection projection;
    private Abilities masterAbilities;
    private GameObject player;
    private Vector3 moveVector;
    private NavMeshAgent agent;
    private Queue<ACTION> queuedActions = new Queue<ACTION>();

    float stasis = 0f;
    float sprintMulti = 2f;

    bool isRun = false;

    public Animator Animator { get; private set; }
    public NavMeshAgent NavAgent { get; private set; }

    // Use this for initialization
    void Start () {
        if (body == null)
        {
            body = GetComponent<Rigidbody>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        if (projection == null)
        {
            projection = FindObjectOfType<Projection>();
        }

        NavAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponentInChildren<Animator>();
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        float multi = isRun ? sprintMulti : 1f;
        if (isRun) projection.SprintBurn();
        NavAgent.speed = multi * masterAbilities.movePoints * MOVEMENT_FORCE;
        Animator.SetFloat("WalkSpeed", Vector3.Magnitude(NavAgent.velocity));

        while (queuedActions.Count > 0)
        {
            ACTION action = queuedActions.Dequeue();
            

            if (stasis > 0f)
            {
                stasis -= Time.deltaTime;
            }
            else
            {
                stasis = 0f;
                HandleAction(action);
            }
        }
	}

    void HandleAction(ACTION action) {
        switch (action)
        {
            case ACTION.Jump:
                break;

            case ACTION.MoveForward:
                {
                    //body.AddForce(transform.forward * FORWARD_FORCE * masterAbilities.movePoints);
                    NavAgent.SetDestination(transform.position + transform.forward);
                }
                break;
            case ACTION.MoveBackward:
                {
                    //body.AddForce(-transform.forward * REVERSE_FORCE * masterAbilities.movePoints);
                    //NavAgent.SetDestination(transform.position - transform.forward);
                }
                break;
            case ACTION.MoveLeft:
                {
                    //body.AddForce(-transform.right * LEFT_FORCE * masterAbilities.movePoints);
                    NavAgent.SetDestination(transform.position - transform.right);
                }
                break;
            case ACTION.MoveRight:
                {
                    //body.AddForce(transform.right * RIGHT_FORCE * masterAbilities.movePoints);
                    NavAgent.SetDestination(transform.position + transform.right);
                }
                break;
            case ACTION.MoveVector:
                {
                    NavAgent.SetDestination(transform.position + moveVector);
                    transform.forward = Vector3.Lerp(transform.forward, moveVector, Time.deltaTime * 5);
                }
                break;
            case ACTION.Projection:
                {
                    projection.SelectTarget();
                }
                break;
            case ACTION.Run:
                {
                    isRun = true;
                }
                break;
            case ACTION.Walk:
                {
                    isRun = false;
                }
                break;
        }
    }

    public void SetMoveVector(Vector3 vector)
    {
        moveVector = vector;
    }

    public void Stasis(float time) {
        stasis = time;
    }

    public void UpdateReferences() {
        body = GetComponent<Rigidbody>();
        Animator = GetComponentInChildren<Animator>();

        NavAgent.SetDestination(NavAgent.transform.position);
        CameraControl.Instance.SetCreatureTransform(transform);
    }

    public void QueueAction(ACTION action) {
        queuedActions.Enqueue(action);
    }

    public void IncludeAbilities(Abilities abilities)
    {
        masterAbilities.movePoints += abilities.movePoints;
        masterAbilities.jumpPoints += abilities.jumpPoints;
    }

    public void RemoveAbilities(Abilities abilities)
    {
        masterAbilities.movePoints -= abilities.movePoints;
        masterAbilities.jumpPoints -= abilities.jumpPoints;
    }
}
