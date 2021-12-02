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
    public enum HintsPosition
    {
        Center = 0,
        Left = 1
    }
    public enum ActionStatus
    {
        NothingHappened = 0,
        WrongAction = 1,
        CorrectAction = 2
    }
    public Stages currentStage;
    Action actionOnComplete;
    Action<List<Tuple<TutorialScript.Actions, GameObject>>> permitedActionSetter;
    static bool isTutorialEnabled = false;
    public ActionStatus actionStatus = ActionStatus.NothingHappened;
    int gestureSubscriptionID;
    SwipesController swipesController;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        tutorialSwipeDown = GameObject.Find("SwipeDownGesture");
        tutorialSwipeUp = GameObject.Find("SwipeUpGesture");
        tutorialSwipeLeft = GameObject.Find("SwipeLeftGesture");
        tutorialSwipeRight = GameObject.Find("SwipeRightGesture");

        swipesController = GameObject.Find("Controller").GetComponent<SwipesController>();

        tutorialSwipeDown.SetActive(false);
        tutorialSwipeUp.SetActive(false);
        tutorialSwipeLeft.SetActive(false);
        tutorialSwipeRight.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<TutorialScript.Actions, GameObject>>> actionRestricter, HintsPosition pos)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        if (pos == HintsPosition.Center)
        {
            tutorialSwipeDown.transform.localPosition = new Vector3(0f, 0f, 0f);
            tutorialSwipeUp.transform.localPosition = new Vector3(0f, 0f, 0f);
            tutorialSwipeLeft.transform.localPosition = new Vector3(0f, 0f, 0f);
            tutorialSwipeRight.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
        else
        {
            tutorialSwipeDown.transform.localPosition = new Vector3(-400f, 0f, 0f);
            tutorialSwipeUp.transform.localPosition = new Vector3(-400f, 0f, 0f);
            tutorialSwipeLeft.transform.localPosition = new Vector3(-400f, 0f, 0f);
            tutorialSwipeRight.transform.localPosition = new Vector3(-400f, 0f, 0f);
        }
        StartNextStep();
    }
    public void DisableTutorial()
    {
        swipesController.UnsubscribeFromSwipe(gestureSubscriptionID);
        tutorialSwipeDown.SetActive(false);
        tutorialSwipeUp.SetActive(false);
        tutorialSwipeLeft.SetActive(false);
        tutorialSwipeRight.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }

    static public bool IsTutorialEnabled()
    {
        return isTutorialEnabled;
    }

    public void StartNextStep()
    {
        Action<Vector2> gestureNotificationHandler = (Vector2 delta) =>
        {
            swipesController.UnsubscribeFromSwipe(gestureSubscriptionID);
            gestureSubscriptionID = swipesController.SubscribeToSwipe(SwipesController.Swipe.Ending,
                (Vector2 delta1) =>
                {
                    swipesController.UnsubscribeFromSwipe(gestureSubscriptionID);
                    StartNextStep();
                });
        };
        currentStage++;
        List<Tuple<TutorialScript.Actions, GameObject>> newPermittedActions = new List<Tuple<TutorialScript.Actions, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.SwipeDown:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeDown, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeDown.SetActive(true);
                gestureSubscriptionID = swipesController.SubscribeToSwipe(SwipesController.Swipe.Down, gestureNotificationHandler);
                break;
            case Stages.SwipeUp:
                tutorialSwipeDown.SetActive(false);
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeUp, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeUp.SetActive(true);
                gestureSubscriptionID = swipesController.SubscribeToSwipe(SwipesController.Swipe.Up, gestureNotificationHandler);
                break;
            case Stages.SwipeLeft:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeLeft, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeUp.SetActive(false);
                tutorialSwipeLeft.SetActive(true);
                gestureSubscriptionID = swipesController.SubscribeToSwipe(SwipesController.Swipe.Left, gestureNotificationHandler);
                break;
            case Stages.SwipeRight:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeRight, null));
                permitedActionSetter(newPermittedActions);
                tutorialSwipeLeft.SetActive(false);
                tutorialSwipeRight.SetActive(true);
                gestureSubscriptionID = swipesController.SubscribeToSwipe(SwipesController.Swipe.Right, gestureNotificationHandler);
                break;
            case Stages.End:
                tutorialSwipeRight.SetActive(false);
                permitedActionSetter(newPermittedActions);
                actionOnComplete();
                currentStage = Stages.WaitingForStart;
                break;
        }
    }
}
