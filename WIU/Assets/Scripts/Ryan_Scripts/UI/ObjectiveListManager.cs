using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectiveListManager : MonoBehaviour
{
    public GameObject objectivesPrefab;
    public Transform objectivesListParent;
    //public TMP_Text objectivesCompletedText;

    private List<Objectives> objectives = new List<Objectives>();

    void Start()
    {
        InitializeObjectives();
        UpdateObjectivesListUI();
    }

    void InitializeObjectives()
    {
        // Populate tasks here, you can load from a data file, for simplicity let's add them manually
        objectives.Add(new Objectives("Find a Gun"));
        objectives.Add(new Objectives("Find a Light"));
    }

    void MainFloorObjectives()
    {
        objectives.Clear();
        objectives.Add(new Objectives("Find Puzzle Room"));
        //objectives.Add(new Objectives("Totems Collected: " + ));
    }

    void SecondFloorObjectives()
    {
        objectives.Clear();

    }

    void UpdateObjectivesListUI()
    {
        foreach (Objectives objective in objectives)
        {
            GameObject objectiveObject = Instantiate(objectivesPrefab, objectivesListParent);
            Objectives objectivesUI = objectiveObject.GetComponent<Objectives>();
            objectivesUI.Setup(objective);
        }
    }

    public void CompleteTask(Objectives objectives)
    {
        objectives.completed = true;
        UpdateObjectivesListUI();
        CheckAllObjectivesCompleted();
    }

    void CheckAllObjectivesCompleted()
    {
        //bool allTasksCompleted = true;
        foreach (Objectives objectives in objectives)
        {
            if (!objectives.completed)
            {
                //allTasksCompleted = false;
                break;
            }
        }

        //if (allTasksCompleted)
        //{
        //    objectivesCompletedText.text = "All tasks completed!";
        //    // Implement logic for what happens when all tasks are completed
        //}
    }
}
