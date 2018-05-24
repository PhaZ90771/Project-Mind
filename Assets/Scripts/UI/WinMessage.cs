using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinMessage : MonoBehaviour {

    Projection projection = null;
    Text text = null;

    // Use this for initialization
    void Start() {
        projection = GameObject.FindGameObjectWithTag("Player").GetComponent<Projection>();
        text = this.gameObject.GetComponent<Text>();

        text.enabled = false;
    }

    // Update is called once per frame
    void Update() {
        text.enabled = projection.IsLastAlive;
    }
}
