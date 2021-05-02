using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialZoom : MonoBehaviour
{
    GameObject tutorialZoomIn;
    GameObject tutorialZoomOut;
    public enum Stages
    {
        WaitingForStart = 0,
        ZoomIn = 1,
        ZoomOut = 2,
        End = 3,
    }
    public enum ActionStatus
    {
        NothingHappened = 0,
        WrongAction = 1,
        CorrectAction = 2
    }
    public Stages currentStage;
    Action actionOnComplete;
    Action<List<Tuple<GesturesController.Gestures, GameObject>>> permitedActionSetter;
    public ActionStatus actionStatus = ActionStatus.NothingHappened;
    uint gestureSubscriptionID, endingSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        tutorialZoomIn = GameObject.Find("ZoomIn");
        tutorialZoomOut = GameObject.Find("ZoomOut");

        tutorialZoomIn.SetActive(false);
        tutorialZoomOut.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<GesturesController.Gestures, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }
    public void DisableTutorial()
    {
        GesturesController.unsubscribeFromGesture(gestureSubscriptionID);
        tutorialZoomIn.SetActive(false);
        tutorialZoomOut.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        Action<GesturesController.Gestures, Vector2> gestureNotificationHandler = (GesturesController.Gestures gesture, Vector2 delta) => {
            GesturesController.unsubscribeFromGesture(gestureSubscriptionID);
            gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Ending,
                (GesturesController.Gestures endingGesture, Vector2 delta1) =>
                {
                    GesturesController.unsubscribeFromGesture(gestureSubscriptionID);
                    StartNextStep();
                });
        };
        currentStage++;
        List<Tuple<GesturesController.Gestures, GameObject>> newPermittedActions = new List<Tuple<GesturesController.Gestures, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.ZoomIn:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.Spread, null));
                permitedActionSetter(newPermittedActions);
                tutorialZoomIn.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Spread, gestureNotificationHandler);
                break;
            case Stages.ZoomOut:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.Pinch, null));
                permitedActionSetter(newPermittedActions);
                tutorialZoomIn.SetActive(false);
                tutorialZoomOut.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Pinch, gestureNotificationHandler);
                break;
            case Stages.End:
                tutorialZoomOut.SetActive(false);
                permitedActionSetter(newPermittedActions);
                actionOnComplete();
                currentStage = Stages.WaitingForStart;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
