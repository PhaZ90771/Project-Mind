using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour {
    ActionController actionController;
    Projection projection;

	// Use this for initialization
	void Start () {
        actionController = this.transform.GetComponentInParent<ActionController>();
        projection = this.transform.GetComponent<Projection>();
    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            actionController.QueueAction(ActionController.ACTION.Jump);
        }
        if (Input.GetKey(KeyCode.W))
        {
            actionController.QueueAction(ActionController.ACTION.MoveForward);
        }
        if (Input.GetKey(KeyCode.A))
        {
            actionController.QueueAction(ActionController.ACTION.MoveLeft);
        }
        if (Input.GetKey(KeyCode.S))
        {
            actionController.QueueAction(ActionController.ACTION.MoveBackward);
        }
        if (Input.GetKey(KeyCode.D))
        {
            actionController.QueueAction(ActionController.ACTION.MoveRight);
        }
        if (Input.GetMouseButtonDown(0))
        {
            actionController.QueueAction(ActionController.ACTION.Projection);
        }
        if ((!projection.IsAlive || projection.IsLastAlive) && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Restarting...");
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            actionController.QueueAction(ActionController.ACTION.Run);
        }
        else
        {
            actionController.QueueAction(ActionController.ACTION.Walk);
        }
    }

    public void UpdateReferences() {
        actionController = this.transform.GetComponentInParent<ActionController>();
    }
}
