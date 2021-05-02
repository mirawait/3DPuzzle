using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TutorialSwipes : MonoBehaviour
{
    GameObject tutorialSwipeDown;
    GameObject tutorialSwipeUp;
    GameObject tutorialSwipeLeft;
    GameObject tutorialSwipeRight;
    public enum Stages
    {
        WaitingForStart = 0,
        SwipeDown = 1,
        SwipeUp = 2,
        SwipeLeft = 3,
        SwipeRight = 4,
        End = 5,
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
    bool isZoomTutorialEnabled;
    public ActionStatus actionStatus = ActionStatus.NothingHappened;
    uint gestureSubscriptionID, endingSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        tutorialSwipeDown = GameObject.Find("SwipeDownGesture");
        tutorialSwipeUp = GameObject.Find("SwipeUpGesture");
        tutorialSwipeLeft = GameObject.Find("SwipeLeftGesture");
        tutorialSwipeRight = GameObject.Find("SwipeRightGesture");

        tutorialSwipeDown.SetActive(false);
        tutorialSwipeUp.SetActive(false);
        tutorialSwipeLeft.SetActive(false);
        tutorialSwipeRight.SetActive(false);
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
        tutorialSwipeDown.SetActive(false);
        tutorialSwipeUp.SetActive(false);
        tutorialSwipeLeft.SetActive(false);
        tutorialSwipeRight.SetActive(false);
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
            case Stages.SwipeDown:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.SwipeDown, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeDown.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDown, gestureNotificationHandler);
                break;
            case Stages.SwipeUp:
                tutorialSwipeDown.SetActive(false);
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.SwipeUp, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeUp.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeUp, gestureNotificationHandler);
                break;
            case Stages.SwipeLeft:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.SwipeLeft, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeUp.SetActive(false);
                tutorialSwipeLeft.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeLeft, gestureNotificationHandler);
                break;
            case Stages.SwipeRight:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.SwipeRight, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeLeft.SetActive(false);
                tutorialSwipeRight.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeRight, gestureNotificationHandler);
                break;
            case Stages.End:
                tutorialSwipeRight.SetActive(false);
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
