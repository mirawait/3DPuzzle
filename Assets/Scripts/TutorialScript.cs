using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

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
        SolveSwipes = 8,
        SolveRotation = 9,
        SolveDoubleTap = 10,
        ChoosePiece = 11,
        PieceRotation = 12,
        PieceConfirm = 13,
        PiecePlacing = 14,
        End = 15
    }

    public enum Actions
    {
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
        PinchClose,
        PinchOpen,
        Twist,
        Tap,
        DoubleTap
    }

    TutorialSwipes swipeTutorial;
    TapOnObjectTutorial tappingTutorial;
    TutorialZoom zoomTutorial;
    RotationTutorial rotationTutorial;
    TutorialDoubleTap doubleTapTutorial;
    SaveManager saveManager;
    static bool isTutorialEnabled;
    public TutorialStage currentStage = TutorialStage.WaitingForStart;
    static public GameObject tappingTarget;
    GameObject chosenPiece;
    GameObject sovleButton;
    private CameraScript mainCamera;
    GameObject tutorialCompleteText;
    VisualElement planetInfoScreen;
    EventCallback<ClickEvent> eventOnClick;
    bool textFadingOut = false, textFadingIn = false, timeSpan = false;
    static public List<Tuple<Actions, GameObject>> pertitedAction = new List<Tuple<Actions, GameObject>>();

    void Start()
    {
        planetInfoScreen = GameObject.Find("PlanetInfoUI").GetComponent<UIDocument>().rootVisualElement;
        swipeTutorial = GameObject.Find("SwipesTutorial").GetComponent<TutorialSwipes>();
        tappingTutorial = GameObject.Find("TappingTutorial").GetComponent<TapOnObjectTutorial>();
        zoomTutorial = GameObject.Find("ZoomTutorial").GetComponent<TutorialZoom>();
        rotationTutorial = GameObject.Find("RotationTutorial").GetComponent<RotationTutorial>();
        doubleTapTutorial = GameObject.Find("DoubleTapTutorial").GetComponent<TutorialDoubleTap>();
        saveManager = GameObject.FindGameObjectWithTag("LoadSceneTag").GetComponent<SaveManager>();
        tutorialCompleteText = GameObject.Find("TutorialCompleteText");
        tappingTarget = GameObject.Find("Settings");
        sovleButton = GameObject.Find("SolveButton");
        sovleButton.SetActive(false);

        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();

        var color = Color.white;
        color.a = 0;
        tutorialCompleteText.GetComponent<TextMeshProUGUI>().color = color;
        tutorialCompleteText.SetActive(false);

        eventOnClick = ev => { StartNextStep(); };
    }

    public void EnableTutorial()
    {
        if (!isTutorialEnabled)
        {
            isTutorialEnabled = true;
            currentStage = TutorialStage.WaitingForStart;
            StartNextStep();
        }
    }
    public static bool IsTutorialEnabled()
    {
        return isTutorialEnabled;
    }
    public void DisableTutorial()
    {
        isTutorialEnabled = false;
        swipeTutorial.DisableTutorial();
        zoomTutorial.DisableTutorial();
        tappingTutorial.DisableTutorial();
        doubleTapTutorial.DisableTutorial();
        rotationTutorial.DisableTutorial();
        pertitedAction.Clear();
        var but = planetInfoScreen?.Q("solve-button");
        but.style.backgroundImage = Resources.Load("UI/Buttons/SolveButton") as Texture2D;
        but.SetEnabled(true);
        currentStage = TutorialStage.WaitingForStart;
    }

    static public bool IsActionPermitted(Actions action, GameObject target = null)
    {
        if (pertitedAction.Count == 0)
            return true;
        bool isPermitted = false;
        foreach (Tuple<Actions, GameObject> gesture in pertitedAction)
        {
            if (gesture.Item1 == action && gesture.Item2 == target)
                isPermitted = true;
        }
        return isPermitted;
    }

    void SetPermittedActions(List<Tuple<Actions, GameObject>> newPermitedAction)
    {
        pertitedAction = newPermitedAction;
    }

    public void StartNextStep()
    {
        currentStage++;
        var but = planetInfoScreen?.Q("solve-button");
        Action<List<Tuple<TutorialScript.Actions, GameObject>>> permittedActionSetter = (List<Tuple<Actions, GameObject>> newPermitedAction) => { SetPermittedActions(newPermitedAction); };
        Action startNextStepHandler = () => { Invoke("StartNextStep", 0.5f); };
        switch (currentStage)
        {
            case TutorialStage.WaitingForStart:
                break;
            case TutorialStage.WaitingForCameraSolarPhase:
                StartCoroutine(_WaitForCameraPhase(CameraScript.Phase.Free));
                break;
            case TutorialStage.SolarSystemRotation:
                swipeTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter, TutorialSwipes.HintsPosition.Center);
                break;
            case TutorialStage.SolarSystemZoom:
                zoomTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.SolarSystemTap:
                tappingTutorial.EnableTutorial(tappingTarget, startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.WaitingForCameraPlanetLock:
                StartCoroutine(_WaitForCameraPhase(CameraScript.Phase.PlanetLock));
                break;
            case TutorialStage.PlanetRotation:
                but.SetEnabled(false);
                swipeTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter, TutorialSwipes.HintsPosition.Left);
                break;
            case TutorialStage.SolveClick:
                but.SetEnabled(true);
                but.style.backgroundImage = Resources.Load("UI/Buttons/SolveButtonTutorial") as Texture2D;
                but.RegisterCallback<ClickEvent>(eventOnClick);
                break;
            case TutorialStage.SolveSwipes:
                but.UnregisterCallback<ClickEvent>(eventOnClick);
                but.style.backgroundImage = Resources.Load("UI/Buttons/SolveButton") as Texture2D;
                swipeTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter, TutorialSwipes.HintsPosition.Center);
                break;
            case TutorialStage.SolveRotation:
                rotationTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.SolveDoubleTap:
                doubleTapTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.ChoosePiece:
                var puzzleObjects = GameObject.FindGameObjectsWithTag("piece");
                foreach(GameObject obj in puzzleObjects)
                {
                    if (obj.GetComponent<Piece>() != null && obj.GetComponent<Renderer>().enabled == true)
                    {
                        chosenPiece = obj;
                        tappingTutorial.EnableTutorial(chosenPiece, startNextStepHandler, permittedActionSetter);
                        return;
                    }
                }
                break;
            case TutorialStage.PieceRotation:
                rotationTutorial.EnableTutorial(startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.PieceConfirm:
                tappingTutorial.EnableTutorial(chosenPiece, startNextStepHandler, permittedActionSetter);
                break;
            case TutorialStage.PiecePlacing:
                chosenPiece.GetComponent<Piece>().MakeTwinMarkedForTutorial();
                StartCoroutine(_WaitForPieceToFit());
                break;
            case TutorialStage.End:
                isTutorialEnabled = false;
                saveManager.MakeTutorialDone();
                notifyAboutTutorialEnding();
                pertitedAction.Clear();
                currentStage = TutorialStage.WaitingForStart;
                Debug.Log("Tutorial Ended");
                break;
        }
    }

    IEnumerator _WaitForCameraPhase(CameraScript.Phase targetPhase)
    {
        while (!mainCamera.IsCurrentPhase(targetPhase))
        {
            yield return new WaitForSeconds(0.01f);
        }
        StartNextStep();
        yield break;
    }

    IEnumerator _WaitForPieceToFit()
    {
        while(chosenPiece.GetComponent<Piece>().GetCondition() != Piece.Condition.FIT)
        {
            yield return new WaitForSeconds(0.01f);
        }
        StartNextStep();
        yield break;
    }

    void notifyAboutTutorialEnding()
    {
        textFadingOut = true;
        textFadingIn = false; 
        timeSpan = false;
        var color = Color.white;
        color.a = 0;
        tutorialCompleteText.GetComponent<TextMeshProUGUI>().color = color;
        tutorialCompleteText.SetActive(true);
        StartCoroutine(_StartFadeOut());
    }
    IEnumerator _StartFadeOut()
    {
        while (tutorialCompleteText.GetComponent<TextMeshProUGUI>().color.a < 1)
        {
            float currentA = tutorialCompleteText.GetComponent<TextMeshProUGUI>().color.a;
            var color = Color.white;
            color.a = currentA + 0.1f;
            tutorialCompleteText.GetComponent<TextMeshProUGUI>().color = color;//CrossFadeAlpha(1, 1, false);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(_StartFadeIn());
        yield break;
    }
    IEnumerator _StartFadeIn()
    {
        while (tutorialCompleteText.GetComponent<TextMeshProUGUI>().color.a > 0)
        {
            float currentA = tutorialCompleteText.GetComponent<TextMeshProUGUI>().color.a;
            var color = Color.white;
            color.a = currentA - 0.1f;
            tutorialCompleteText.GetComponent<TextMeshProUGUI>().color = color;//CrossFadeAlpha(1, 1, false);
            yield return new WaitForSeconds(0.05f);
        }
        tutorialCompleteText.SetActive(false);
        yield break;
    }

    public void EndTutoial()
    {
        isTutorialEnabled = false;
        currentStage = TutorialStage.WaitingForStart;
    }
}
