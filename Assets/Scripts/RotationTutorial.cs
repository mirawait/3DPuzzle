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
    TwistController twistController;
    Action<List<Tuple<TutorialScript.Actions, GameObject>>> permitedActionSetter;
    
    bool isZoomTutorialEnabled;
    int twistSubscriptionID;

    // Start is called before the first frame update
    void Start()
    {
        currentStage = Stages.WaitingForStart;
        rotationGesture = GameObject.Find("RotationGesture");
        twistController = GameObject.Find("Controller").GetComponent<TwistController>();
        rotationGesture.SetActive(false);
    }

    public void EnableTutorial(Action onComplete, Action<List<Tuple<TutorialScript.Actions, GameObject>>> actionRestricter)
    {
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        
        StartNextStep();
    }
    
    public void DisableTutorial()
    {
        twistController.UnsubscribeFromTwist(twistSubscriptionID);
        rotationGesture.SetActive(false);
        currentStage = Stages.WaitingForStart;
    }

    public void StartNextStep()
    {
        Action<float> gestureNotificationHandler = (float angle) => {
            twistController.UnsubscribeFromTwist(twistSubscriptionID);
            twistSubscriptionID = twistController.SubscribeToTwist(TwistController.Twist.Ending,
                (float _) =>
                {
                    twistController.UnsubscribeFromTwist(twistSubscriptionID);
                    StartNextStep();
                });
        };
        currentStage++;
        List<Tuple<TutorialScript.Actions, GameObject>> newPermittedActions = new List<Tuple<TutorialScript.Actions, GameObject>>();
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.Rotation:
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.Twist, null));
                permitedActionSetter(newPermittedActions);
                rotationGesture.SetActive(true);
                twistSubscriptionID = twistController.SubscribeToTwist(TwistController.Twist.Twist, gestureNotificationHandler);
                break;
            case Stages.End:
                rotationGesture.SetActive(false);
                permitedActionSetter(newPermittedActions);
                actionOnComplete();
                currentStage = Stages.WaitingForStart;
                break;
        }
    }
}
