using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class RotationTutorial : MonoBehaviour
{
    GameObject rotationGesture;
    public enum Stages
    {
        WaitingForStart = 0,
        Rotation = 1,
        End = 2,
    }
    public Stages currentStage;
    Action actionOnComplete;
    Action<List<Tuple<GesturesController.Gestures, GameObject>>> permitedActionSetter;
    bool isZoomTutorialEnabled;
    uint shufleUpSubscriptionID, shufleDownSubscriptionID, endingSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        rotationGesture = GameObject.Find("RotationGesture");

        rotationGesture.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<GesturesController.Gestures, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }
    public void DisableTutorial()
    {
        GesturesController.unsubscribeFromGesture(shufleUpSubscriptionID);
        GesturesController.unsubscribeFromGesture(shufleDownSubscriptionID);
        rotationGesture.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        Action<GesturesController.Gestures, Vector2> gestureNotificationHandler = (GesturesController.Gestures gesture, Vector2 delta) => {
            GesturesController.unsubscribeFromGesture(shufleUpSubscriptionID);
            GesturesController.unsubscribeFromGesture(shufleDownSubscriptionID);
            endingSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.Ending,
                (GesturesController.Gestures endingGesture, Vector2 delta1) =>
                {
                    GesturesController.unsubscribeFromGesture(endingSubscriptionID);
                    StartNextStep();
                });
        };
        currentStage++;
        List<Tuple<GesturesController.Gestures, GameObject>> newPermittedActions = new List<Tuple<GesturesController.Gestures, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.Rotation:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.ShuffleDown, null));
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.ShuffleUp, null));
                permitedActionSetter(newPermittedActions);
                rotationGesture.SetActive(true);
                shufleUpSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.ShuffleUp, gestureNotificationHandler);
                shufleDownSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.ShuffleDown, gestureNotificationHandler);
                break;
            case Stages.End:
                rotationGesture.SetActive(false);
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
