using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Objectives : MonoBehaviour
{
    private string description;
    public bool completed;

    public TMP_Text descriptionLabel;

    public void Setup(Objectives objective)
    {
        descriptionLabel.text = objective.description;
    }

    public Objectives(string description)
    {
        this.description = description;
        completed = false;
    }
}
