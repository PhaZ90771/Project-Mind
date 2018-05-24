using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Abilities
{
    public int movePoints;
    public int jumpPoints;
}

public class AbilityBehaviour : MonoBehaviour
{
    [SerializeField]
    private Abilities abilities;

    private ActionController controller;

    private void OnEnable()
    {
        controller = this.gameObject.GetComponentInParent<ActionController>();

        if (controller == null)
            return;

        controller.IncludeAbilities(abilities);
    }

    private void OnDisable()
    {
        if(controller != null)
            controller.RemoveAbilities(abilities);
    }
}