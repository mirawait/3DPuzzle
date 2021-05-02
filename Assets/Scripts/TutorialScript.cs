using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialScript : MonoBehaviour
{
    TutorialSwipes swipeTutorial;
    TapOnObjectTutorial tappingTutorial;
    TutorialZoom zoomTutorial;
    public enum TutorialStage
    {
        WaitingForStart = 0,
        SolarSystemRotation = 1,
        SolarSystemZoom = 2,
        SolarSystemTap = 3,
        PuzzleStart = 4,
        End = 5
    }

    bool isTutorialEnabled;
    public TutorialStage currentStage = TutorialStage.WaitingForStart;
    static public GameObject tappingTarget;
    static public List<Tuple<GesturesController.Gestures, GameObject>> pertitedAction = new List<Tuple<GesturesController.Gestures, GameObject>>();

    void Start()
    {
        swipeTutorial = GameObject.Find("SwipesTutorial").GetComponent<TutorialSwipes>();
        tappingTutorial = GameObject.Find("TappingTutorial").GetComponent<TapOnObjectTutorial>();
        zoomTutorial = GameObject.Find("ZoomTutorial").GetComponent<TutorialZoom>();
        tappingTarget = GameObject.Find("Settings");
    }

    public void EnableTutorial()
    {
        isTutorialEnabled = true;
        StartNextStep();
    }
    public bool IsTutorialEnabled()
    {
        return isTutorialEnabled;
    }
    public void DisableTutorial()
    {
        isTutorialEnabled = false;
        swipeTutorial.DisableTutorial();
        tappingTutorial.DisableTutorial();
        pertitedAction.Clear();
        currentStage = TutorialStage.WaitingForStart;
    }

    static public bool IsActionPermitted(GesturesController.Gestures action, GameObject target = null)
    {
        if (pertitedAction.Count == 0)
            return true;
        bool isPermitted = false;
        foreach (Tuple<GesturesController.Gestures, GameObject> gesture in pertitedAction)
        {
            if (gesture.Item1 == action && gesture.Item2 == target)
                isPermitted = true;
        }
        return isPermitted;
    }

    void SetPermittedActions(List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction)
    {
        pertitedAction = newPermitedAction;
    }

    public void StartNextStep()
    {
        currentStage++;
        switch (currentStage)
        {
            case TutorialStage.WaitingForStart:
                break;
            case TutorialStage.SolarSystemRotation:
                swipeTutorial.EnableTutorial(() => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.SolarSystemZoom:
                zoomTutorial.EnableTutorial(() => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.SolarSystemTap:
                tappingTutorial.EnableTutorial(tappingTarget, () => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.PuzzleStart:
                swipeTutorial.EnableTutorial(() => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.End:
                isTutorialEnabled = false;
                pertitedAction.Clear();
                currentStage = TutorialStage.WaitingForStart;
                Debug.Log("Tutorial Ended");
                break;
        }
    }

    public void EndTutoial()
    {
        isTutorialEnabled = false;
        currentStage = TutorialStage.WaitingForStart;
    }
}
