using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;

public class TutorialDoubleTap : MonoBehaviour
{
    GameObject doubleTapGesture;
    public enum Stages
    {
        WaitingForStart = 0,
        DoubleTap = 1,
        End = 2,
    }
    public Stages currentStage;
    Action actionOnComplete;
    Action<List<Tuple<GesturesController.Gestures, GameObject>>> permitedActionSetter;
    uint doubleTapSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        doubleTapGesture = GameObject.Find("DoubleTapGesture");

        doubleTapGesture.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<GesturesController.Gestures, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }
    public void DisableTutorial()
    {
        GesturesController.unsubscribeFromGesture(doubleTapSubscriptionID);
        doubleTapGesture.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        Action<GesturesController.Gestures, Vector2> gestureNotificationHandler = (GesturesController.Gestures gesture, Vector2 delta) => {
            GesturesController.unsubscribeFromGesture(doubleTapSubscriptionID);
            StartNextStep();
        };
        currentStage++;
        List<Tuple<GesturesController.Gestures, GameObject>> newPermittedActions = new List<Tuple<GesturesController.Gestures, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.DoubleTap:
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.DoubleTap, null));
                permitedActionSetter(newPermittedActions);
                doubleTapGesture.SetActive(true);
                doubleTapSubscriptionID = GesturesController.subscribeToGesture(GesturesController.Gestures.DoubleTap, gestureNotificationHandler);
                break;
            case Stages.End:
                doubleTapGesture.SetActive(false);
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
