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
    Action<List<Tuple<TutorialScript.Actions, GameObject>>> permitedActionSetter;
    PinchController pinchController;
    public ActionStatus actionStatus = ActionStatus.NothingHappened;
    int gestureSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        tutorialZoomIn = GameObject.Find("SpreadGesture");
        tutorialZoomOut = GameObject.Find("PinchGesture");
        pinchController = GameObject.Find("Controller").GetComponent<PinchController>();
        tutorialZoomIn.SetActive(false);
        tutorialZoomOut.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<TutorialScript.Actions, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }
    public void DisableTutorial()
    {
        pinchController.UnsubscribeFromPinch(gestureSubscriptionID);
        tutorialZoomIn.SetActive(false);
        tutorialZoomOut.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        Action<float> gestureNotificationHandler = (float delta) => {
            pinchController.UnsubscribeFromPinch(gestureSubscriptionID);
            gestureSubscriptionID = pinchController.SubscribeToPinch(PinchController.Pinch.Ending,
                (float delta1) =>
                {
                    pinchController.UnsubscribeFromPinch(gestureSubscriptionID);
                    StartNextStep();
                });
        };
        currentStage++;
        List<Tuple<TutorialScript.Actions, GameObject>> newPermittedActions = new List<Tuple<TutorialScript.Actions, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.ZoomIn:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.PinchOpen, null));
                permitedActionSetter(newPermittedActions);
                tutorialZoomIn.SetActive(true);
                gestureSubscriptionID = pinchController.SubscribeToPinch(PinchController.Pinch.PinchOpen, gestureNotificationHandler);
                break;
            case Stages.ZoomOut:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.PinchClose, null));
                permitedActionSetter(newPermittedActions);
                tutorialZoomIn.SetActive(false);
                tutorialZoomOut.SetActive(true);
                gestureSubscriptionID = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, gestureNotificationHandler);
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
