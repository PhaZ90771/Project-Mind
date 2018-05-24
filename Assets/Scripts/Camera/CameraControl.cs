using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    private static CameraControl instance = null;
    public static CameraControl Instance { get { if (instance == null) { instance = FindObjectOfType<CameraControl>(); } return instance; } }

    [SerializeField]
    private Transform target;
    [SerializeField]
    private Transform creatureTransform;
    [SerializeField]
    private float pitch = 0;
    [SerializeField]
    private float yaw = 0;
    [SerializeField]
    private float distance = 10;
    [SerializeField]
    private float smoothness = 5;
    [SerializeField]
    private float mouseXSensetivity = 5;
    [SerializeField]
    private float mouseYSensetivity = 5;
    [SerializeField]
    private bool invertedYAxis = false;
    [SerializeField]
    private float minPitch = 20;
    [SerializeField]
    private float maxPitch = 60;
    
    public float Pitch { get { return pitch; } set { pitch = value; } }
    public float Yaw { get { return yaw; } set { yaw = value; } }
    public float Distance { get { return distance; } set { distance = value; } }

    public float MinPitch { get { return minPitch; } set { minPitch = value; } }
    public float MaxPitch { get { return maxPitch; } set { maxPitch = value; } }

    public bool PlayerAlive { get; set; }

    public Vector3 ForwardPlane { get { return new Vector3(transform.forward.x, 0, transform.forward.z).normalized; } }
    
    private float targetPitch;
    private float targetYaw;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        targetYaw = yaw;
        targetPitch = pitch;
    }

    private void Update()
    {
        targetYaw += Input.GetAxis("Mouse X") * mouseXSensetivity;
        targetPitch += Input.GetAxis("Mouse Y") * mouseYSensetivity * (invertedYAxis ? 1 : -1);
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);

        if (PlayerAlive)
        {
            if (creatureTransform != null)
            {
                creatureTransform.forward = Vector3.Lerp(creatureTransform.forward, ForwardPlane, Time.deltaTime * smoothness);// (creatureTransform.position + ForwardPlane);
            }
            else
            {
                Debug.LogWarning("CreatureTransform is null");
            }
        }
    }

    private void LateUpdate()
    {
        Yaw = Mathf.Lerp(Yaw, targetYaw, Time.deltaTime * smoothness);
        Pitch = Mathf.Lerp(Pitch, targetPitch, Time.deltaTime * smoothness);

        var rotation = Quaternion.Euler(pitch, yaw, 0);

        if (target != null)
        {
            transform.position = target.position + rotation * new Vector3(0, 0, -distance);
            transform.rotation = rotation;
        }
    }

    public void SetCreatureTransform(Transform creatureTransform)
    {
        this.creatureTransform = creatureTransform;
        targetYaw = yaw = creatureTransform.eulerAngles.y;

    }
}
