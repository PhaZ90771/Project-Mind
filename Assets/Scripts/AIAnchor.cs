using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAnchor : MonoBehaviour
{
    public float radius;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
