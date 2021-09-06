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
    Action<List<Tuple<TutorialScript.Actions, GameObject>>> permitedActionSetter;
    TapController tapController;
    int doubleTapSubscriptionID;
    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        doubleTapGesture = GameObject.Find("DoubleTapGesture");
        tapController = GameObject.Find("Controller").GetComponent<TapController>();
        doubleTapGesture.SetActive(false);
    }
    public void EnableTutorial(Action onComplete, Action<List<Tuple<TutorialScript.Actions, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }
    public void DisableTutorial()
    {
        tapController.UnsubscribeFromTap(doubleTapSubscriptionID);
        doubleTapGesture.SetActive(false);
        currentStage = currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        Action<GameObject> gestureNotificationHandler = (GameObject target) => {
            tapController.UnsubscribeFromTap(doubleTapSubscriptionID);
            StartNextStep();
        };
        currentStage++;
        List<Tuple<TutorialScript.Actions, GameObject>> newPermittedActions = new List<Tuple<TutorialScript.Actions, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.DoubleTap:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.DoubleTap, null));
                permitedActionSetter(newPermittedActions);
                doubleTapGesture.SetActive(true);
                doubleTapSubscriptionID = tapController.SubscribeToTap(TapController.Tap.DoubleTap, gestureNotificationHandler);
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
