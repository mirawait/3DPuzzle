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
    GameObject tutorialZoomIn;
    GameObject tutorialZoomOut;
    public enum Stages
    {
        WaitingForStart = 0,
        SwipeUp = 1,
        SwipeDown = 2,
        SwipeLeft = 3,
        SwipeRight = 4,
        ZoomIn = 5,
        ZoomOut = 6,
        End = 7,
    }
    public enum ActionStatus
    {
        NothingHappened = 0,
        WrongAction = 1,
        CorrectAction = 2
    }
    public Stages currentStage;
    Action actionOnComplete;
    Action<TutorialScript.Actions> permitedActionSetter;
    bool isZoomTutorialEnabled;
    public ActionStatus actionStatus = ActionStatus.NothingHappened;
    uint gestureSubscriptionID, endingSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        tutorialSwipeDown = GameObject.Find("SwipeDown");
        tutorialSwipeUp = GameObject.Find("SwipeUp");
        tutorialSwipeLeft = GameObject.Find("SwipeLeft");
        tutorialSwipeRight = GameObject.Find("SwipeRight");
        tutorialZoomIn = GameObject.Find("ZoomIn");
        tutorialZoomOut = GameObject.Find("ZoomOut");

        tutorialSwipeDown.SetActive(false);
        tutorialSwipeUp.SetActive(false);
        tutorialSwipeLeft.SetActive(false);
        tutorialSwipeRight.SetActive(false);
        tutorialZoomIn.SetActive(false);
        tutorialZoomOut.SetActive(false);
    }
    public void EnableTutorial(bool withZooming, Action onComplete, Action<TutorialScript.Actions> actionRestricter)
    {
        isZoomTutorialEnabled = withZooming;
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
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.SwipeUp:
                permitedActionSetter(TutorialScript.Actions.CameraRotationUp);
                tutorialSwipeUp.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeUp, gestureNotificationHandler);
                break;
            case Stages.SwipeDown:
                permitedActionSetter(TutorialScript.Actions.CameraRotationDown);
                tutorialSwipeUp.SetActive(false);
                tutorialSwipeDown.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDown, gestureNotificationHandler);
                break;
            case Stages.SwipeLeft:
                permitedActionSetter(TutorialScript.Actions.CameraRotationLeft);
                tutorialSwipeDown.SetActive(false);
                tutorialSwipeLeft.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeLeft, gestureNotificationHandler);
                break;
            case Stages.SwipeRight:
                permitedActionSetter(TutorialScript.Actions.CameraRotationRight);
                tutorialSwipeLeft.SetActive(false);
                tutorialSwipeRight.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeRight, gestureNotificationHandler);
                break;
            case Stages.ZoomIn:
                tutorialSwipeRight.SetActive(false);
                if (isZoomTutorialEnabled)
                {
                    permitedActionSetter(TutorialScript.Actions.CameraZoomIn);
                    tutorialZoomIn.SetActive(true);
                    gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Spread, gestureNotificationHandler);
                }
                else
                {
                    tutorialZoomOut.SetActive(false);
                    permitedActionSetter(TutorialScript.Actions.Any);
                    actionOnComplete();
                    currentStage = Stages.WaitingForStart;
                }
                break;
            case Stages.ZoomOut:
                permitedActionSetter(TutorialScript.Actions.CameraZoomOut);
                tutorialZoomIn.SetActive(false);
                tutorialZoomOut.SetActive(true);
                gestureSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Pinch, gestureNotificationHandler);
                break;
            case Stages.End:
                tutorialZoomOut.SetActive(false);
                permitedActionSetter(TutorialScript.Actions.Any);
                actionOnComplete();
                currentStage = Stages.WaitingForStart;
                break;
        }
    }

    //void _CheckForSwipes()
    //{
    //    if (actionStatus != ActionStatus.NothingHappened)
    //        return;
    //    if (Input.touchCount == 1)
    //    {
    //        Touch touch = Input.GetTouch(0);
    //        Vector2 deltaPos = touch.deltaPosition;
    //
    //        if (Mathf.Abs(deltaPos.x) > 0 || Mathf.Abs(deltaPos.y) > 0)
    //        {
    //            if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y) && (deltaPos.x > 0 && currentStage == Stages.SwipeRight || deltaPos.x < 0 && currentStage == Stages.SwipeLeft)
    //            || Mathf.Abs(deltaPos.x) < Mathf.Abs(deltaPos.y) && (deltaPos.y > 0 && currentStage == Stages.SwipeUp || deltaPos.y < 0 && currentStage == Stages.SwipeDown))
    //            {
    //                actionStatus = ActionStatus.CorrectAction;
    //            }
    //            else
    //            {
    //                actionStatus = ActionStatus.WrongAction;
    //            }
    //            StartCoroutine(_WaitForTouchCountEqualsZero());
    //        }
    //    }
    //    if (Input.touchCount == 2)
    //    {
    //        Touch touchZero = Input.GetTouch(0);
    //        Touch touchOne = Input.GetTouch(1);
    //
    //        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition,
    //                touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
    //
    //        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude,
    //              touchDeltaMag = (touchZero.position - touchOne.position).magnitude,
    //              deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
    //        if (deltaMagnitudeDiff != 0)
    //        {
    //            if (deltaMagnitudeDiff > 0 && currentStage == Stages.ZoomOut
    //            || deltaMagnitudeDiff < 0 && currentStage == Stages.ZoomIn)
    //            {
    //                actionStatus = ActionStatus.CorrectAction;
    //            }
    //            else
    //            {
    //                actionStatus = ActionStatus.WrongAction;
    //            }
    //            StartCoroutine(_WaitForTouchCountEqualsZero());
    //        }
    //    }
    //}

    IEnumerator _WaitForEndOfGesture()
    {
        while (GesturesController.IsGestureGoing())
        {
            yield return new WaitForSeconds(0.001f);
        }
        StartNextStep();
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        //if (currentStage != Stages.WaitingForStart)
        //{
        //    _CheckForSwipes();
        //}
    }
}
