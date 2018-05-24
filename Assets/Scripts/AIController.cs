using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public enum AIState
    {
        Idle,
        Chase,
        Attack,
        Flee
    }

    public AIAnchor aiAnchor;

    private ActionController actionController;
    private Transform playerTransform;
    private Vector3 idleTarget;

    [SerializeField]
    private bool isAggressive = false;
    [SerializeField]
    private float attackRadius = 1;
    [SerializeField]
    private float chaseRadius = 3;
    [SerializeField]
    private float fleeRadius = 2;

    [SerializeField]
    private AIState state = AIState.Idle;
    private AIState prevState = AIState.Idle;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
    }

    // Use this for initialization
    void Start()
    {
        actionController = this.transform.GetComponentInParent<ActionController>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        idleTarget = GetNewIdleTarget();
    }

    private void Update()
    {
        state = GetCurrentState();

        switch (state)
        {
            case AIState.Idle:
                UpdateIdleState();
                break;

            case AIState.Chase:
                UpdateChaseState();
                break;

            case AIState.Attack:
                UpdateAttackState();
                break;

            case AIState.Flee:
                UpdateFleeState();
                break;
        }

        prevState = state;
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            var useAnchor = aiAnchor != null;
            Vector3 randomPoint = useAnchor ? aiAnchor.transform.position :  center + Random.insideUnitSphere * range;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    private Vector3 GetNewIdleTarget()
    {
        var newTarget = idleTarget;

        var useAnchor = aiAnchor != null;
        var anchorPoint = useAnchor ? aiAnchor.transform.position : transform.position;
        var anchorRadius = useAnchor ? aiAnchor.radius : 10f;

        Vector3 point;

        if (RandomPoint(anchorPoint, anchorRadius, out point))
        {
            Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
            newTarget = point;
        }

        return newTarget;
    }

    private void UpdateIdleState()
    {
        if (prevState != state)
        {
            idleTarget = GetNewIdleTarget();
        }
        else
        {
            if (Vector3.SqrMagnitude(new Vector3(idleTarget.x, 0, idleTarget.z) - new Vector3(actionController.transform.position.x, 0, actionController.transform.position.z)) < 1)
                idleTarget = GetNewIdleTarget();
        }

        actionController.NavAgent.SetDestination(idleTarget);

        Debug.DrawRay(idleTarget, Vector3.up * 10, Color.magenta);
    }

    private void UpdateAttackState()
    {

    }

    private void UpdateChaseState()
    {
        actionController.QueueAction(ActionController.ACTION.MoveVector);
        actionController.SetMoveVector((playerTransform.position - transform.position).normalized);
    }

    private void UpdateFleeState()
    {
        actionController.QueueAction(ActionController.ACTION.MoveVector);
        actionController.SetMoveVector((transform.position - playerTransform.position).normalized);
    }

    private AIState GetCurrentState()
    {
        var sqrMag = GetSqrDistFromPlayer();

        if (isAggressive && sqrMag < (attackRadius * attackRadius))
            return AIState.Attack;

        if (isAggressive && sqrMag < (chaseRadius * chaseRadius))
            return AIState.Chase;

        if (sqrMag < (fleeRadius * fleeRadius))
            return AIState.Flee;

        return AIState.Idle;
    }

    private float GetSqrDistFromPlayer()
    {
        if (playerTransform == null)
            return Mathf.Infinity;

        return Vector3.SqrMagnitude(transform.position - playerTransform.position);
    }
}
