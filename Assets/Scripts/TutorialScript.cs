using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialScript : MonoBehaviour
{
    public enum Actions
    {
        Any = 0,
        CameraRotationLeft = 1,
        CameraRotationRight = 2,
        CameraRotationUp = 3,
        CameraRotationDown = 4,
        CameraZoomIn = 5,
        CameraZoomOut = 6,
        Tapping = 7
    }
    TutorialSwipes swipeTutorial;
    TapOnObjectTutorial tappingTutorial;
    public enum TutorialStage
    {
        WaitingForStart = 0,
        SolarSystemRotationAndZoom = 1,
        SolarSystemTap = 2,
        PuzzleStart = 3,
        End = 4
    }

    bool isTutorialEnabled;
    public TutorialStage currentStage = TutorialStage.WaitingForStart;
    static public GameObject tappingTarget;
    static public Actions pertitedAction = Actions.Any;

    void Start()
    {
        swipeTutorial = GameObject.Find("SwipesTutorial").GetComponent<TutorialSwipes>();
        tappingTutorial = GameObject.Find("TappingTutorial").GetComponent<TapOnObjectTutorial>();
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
        pertitedAction = Actions.Any;
        currentStage = TutorialStage.WaitingForStart;
    }

    static public bool IsActionPermitted(Actions action, GameObject target = null)
    {
        return ((pertitedAction == Actions.Tapping && action == pertitedAction && target == tappingTarget) 
                || (pertitedAction != Actions.Tapping && action == pertitedAction) 
                || pertitedAction == Actions.Any);
    }

    void SetPermittedAction(Actions newPermitedAction)
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
            case TutorialStage.SolarSystemRotationAndZoom:
                swipeTutorial.EnableTutorial(true, () => { StartNextStep(); }, (Actions newPermitedAction) => { SetPermittedAction(newPermitedAction); });
                break;
            case TutorialStage.SolarSystemTap:
                tappingTutorial.EnableTutorial(tappingTarget, () => { StartNextStep(); }, (Actions newPermitedAction) => { SetPermittedAction(newPermitedAction); });
                break;
            case TutorialStage.PuzzleStart:
                swipeTutorial.EnableTutorial(false, () => { StartNextStep(); }, (Actions newPermitedAction) => { SetPermittedAction(newPermitedAction); });
                break;
            case TutorialStage.End:
                isTutorialEnabled = false;
                pertitedAction = Actions.Any;
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
