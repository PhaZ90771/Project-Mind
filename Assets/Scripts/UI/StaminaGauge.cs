using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaGauge : MonoBehaviour {

    Text text = null;
    Projection projection = null;
    [SerializeField]
    float numDivisions = 20;

    // Use this for initialization
    void Start () {
        text = GetComponent<Text>();
        projection = GameObject.FindGameObjectWithTag("Player").GetComponent<Projection>();
	}
	
	// Update is called once per frame
	void Update () {
        //int current = (int)(projection.Stamina / projection.StartinStamina * 100f);
        //text.text = current.ToString() + "%";

        float subDivisionValue = projection.StartinStamina / numDivisions;

        string output = "";
        for (float i = subDivisionValue; i < projection.Stamina; i += subDivisionValue)
        {
            output += "|";
        }
        text.text = output;

        text.enabled = projection.IsAlive;
    }
}
