using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class TutorialScript : MonoBehaviour
{
    public enum TutorialStage
    {
        WaitingForStart = 0,
        WaitingForCameraSolarPhase = 1,
        SolarSystemRotation = 2,
        SolarSystemZoom = 3,
        SolarSystemTap = 4,
        WaitingForCameraPlanetLock = 5,
        PlanetRotation = 6,
        SolveClick = 7,
        End = 8
    }

    TutorialSwipes swipeTutorial;
    TapOnObjectTutorial tappingTutorial;
    TutorialZoom zoomTutorial;
    bool isTutorialEnabled;
    public TutorialStage currentStage = TutorialStage.WaitingForStart;
    static public GameObject tappingTarget;
    Button sovleButton;
    private CameraScript mainCamera;
    static public List<Tuple<GesturesController.Gestures, GameObject>> pertitedAction = new List<Tuple<GesturesController.Gestures, GameObject>>();

    void Start()
    {
        swipeTutorial = GameObject.Find("SwipesTutorial").GetComponent<TutorialSwipes>();
        tappingTutorial = GameObject.Find("TappingTutorial").GetComponent<TapOnObjectTutorial>();
        zoomTutorial = GameObject.Find("ZoomTutorial").GetComponent<TutorialZoom>();
        tappingTarget = GameObject.Find("Settings");
        sovleButton = GameObject.Find("SolveButton").GetComponent<Button>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

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
            case TutorialStage.WaitingForCameraSolarPhase:
                StartCoroutine(_WaitForCameraPhase(CameraScript.Phase.Free));
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
            case TutorialStage.WaitingForCameraPlanetLock:
                StartCoroutine(_WaitForCameraPhase(CameraScript.Phase.PlanetLock));
                break;
            case TutorialStage.PlanetRotation:
                swipeTutorial.EnableTutorial(() => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.SolveClick:
                tappingTutorial.EnableTutorial(sovleButton.gameObject, () => { StartNextStep(); }, (List<Tuple<GesturesController.Gestures, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); });
                break;
            case TutorialStage.End:
                isTutorialEnabled = false;
                pertitedAction.Clear();
                currentStage = TutorialStage.WaitingForStart;
                Debug.Log("Tutorial Ended");
                break;
        }
    }

    IEnumerator _WaitForCameraPhase(CameraScript.Phase targetPhase)
    {
        while (!mainCamera.isCurrentPhase(targetPhase))
        {
            yield return new WaitForSeconds(0.01f);
        }
        StartNextStep();
        yield break;
    }

    public void EndTutoial()
    {
        isTutorialEnabled = false;
        currentStage = TutorialStage.WaitingForStart;
    }
}
