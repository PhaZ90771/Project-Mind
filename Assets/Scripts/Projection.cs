using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projection : MonoBehaviour {

    Transform target = null;
    Transform host = null;
    Transform hostHead = null;

    PlayerInput playerInput = null;
    ActionController actionController = null;

    public GameObject destructionEffect = null;
    public GameObject successEffect = null;
    public GameObject shootEffect = null;

    GameObject idleEffect = null;
    GameObject aimingEffect = null;

    [SerializeField]
    float startingStamina = 100;
    float stamina = 100;
    [SerializeField]
    float staminaDepletionRate = 5f;
    [SerializeField]
    float projectionRange = 4f;

    bool playedSuccessEffect = false;
    bool isACarcus = false;

    public float StartinStamina
    {
        get
        {
            return startingStamina;
        }
    }
    public float Stamina
    {
        get
        {
            return stamina;
        }
    }

    public bool IsAlive
    {
        get
        {
            return stamina > 0f;
        }
    }

    public bool IsLastAlive
    {
        get
        {
            return IsAlive && GameObject.FindGameObjectsWithTag("Creature").Length == 1;
        }
    }

    public bool HasHost
    {
        get
        {
            return host != null;
        }
    }

    public bool IsCarcus
    {
        get
        {
            return isACarcus;
        }
    }

	// Use this for initialization
	void Start () {
        host = this.transform.parent;
        playerInput = this.GetComponent<PlayerInput>();
        actionController = host.GetComponent<ActionController>();
        CameraControl.Instance.PlayerAlive = true;

        foreach (var head in GameObject.FindGameObjectsWithTag("Head"))
        {
            if (head.transform.root == host)
            {
                hostHead = head.transform;
            }
        }

        aimingEffect = this.transform.Find("VfxAiming").gameObject;
        idleEffect = this.transform.Find("Vfx_Idle").gameObject;

        aimingEffect.transform.SetParent(hostHead, false);
        idleEffect.transform.SetParent(hostHead, false);

        stamina = startingStamina;
    }
	
	// Update is called once per frame
	void Update () {
        if (isACarcus)
            return;

        if ((!IsAlive || IsLastAlive) && HasHost)
        {
            if (!IsAlive)
            {
                GameObject player = this.transform.gameObject;
                player.transform.SetParent(null, true);

                SoundManager.Instance.StopTrack("background");
                SoundManager.Instance.PlayEffect("disintegration");
                //Destroy(host.gameObject);

                isACarcus = true;
                CameraControl.Instance.PlayerAlive = false;
                actionController.Animator.SetTrigger("Death");
               
                if (destructionEffect != null)
                {
                    Instantiate(destructionEffect, host.transform.position, host.transform.rotation);
                }

                foreach (ParticleSystem ps in this.gameObject.GetComponentsInChildren<ParticleSystem>()) {
                    ps.Stop();
                }
            }
            if (IsLastAlive && !playedSuccessEffect)
            {
                playedSuccessEffect = true;
                Vector3 location = host.transform.position + new Vector3(0, 3, 0);
                Instantiate(successEffect, location, host.transform.rotation);
                SoundManager.Instance.PlayTrack("success");
            }
        }

        if (stamina > 0 && !IsLastAlive)
        {
            stamina -= staminaDepletionRate * Time.deltaTime;
            if (stamina < 0) stamina = 0;
        }
    }

    void OnDrawGizmos() {
        if (host != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(aimingEffect.transform.position, host.forward);
        }
    }

    public void SelectTarget() {
        stamina -= 10;
        if (stamina < 0f) stamina = 0f;

        RaycastHit hit;
        if (Physics.Raycast(aimingEffect.transform.position, host.forward, out hit, projectionRange))
        {
            if (hit.transform.CompareTag("Creature"))
            {
                target = hit.transform;
            }
            else
            {
                target = null;
            }
        }
        else
        {
            target = null;
        }

        if (target != null) Project();
    }

    private void Project() {
        if (target == null)
        {
            throw new UnityException("Missing Target");
        }
        if (target == host)
        {
            throw new UnityException("Target Must Not Be Self");
        }

        aimingEffect.transform.SetParent(target, false);
        idleEffect.transform.SetParent(target, false);

        // Move to Target & Destroy
        this.transform.SetParent(target, false);
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, host.transform.position, host.transform.rotation);
        }
        SoundManager.Instance.PlayEffect("disintegration");
        Destroy(host.gameObject);
        stamina = (startingStamina + stamina) / 2;
        SoundManager.Instance.PlayEffect("transmission");
        idleEffect.GetComponent<ParticleSystem>().Stop();
        idleEffect.GetComponent<ParticleSystem>().Play();

        // Update References
        host = this.transform.parent;
        actionController = host.GetComponent<ActionController>();
        playerInput.UpdateReferences();
        actionController.UpdateReferences();

        // Update reference to head
        foreach (var head in GameObject.FindGameObjectsWithTag("Head"))
        {
            if (head.transform.root == host)
            {
                hostHead = head.transform;
            }
        }

        aimingEffect.transform.SetParent(hostHead, false);
        idleEffect.transform.SetParent(hostHead, false);

        // Destroy AI
        foreach (Transform child in host.transform)
        {
            if (child.CompareTag("AI"))
            {
                Destroy(child.gameObject);
            }
        }

        actionController.Stasis(0.5f);
    }

    public void SprintBurn() {
        if (this.IsLastAlive) return;
        stamina -= 10f * Time.deltaTime;
        if (stamina < 0f) stamina = 0f;
    }
}
