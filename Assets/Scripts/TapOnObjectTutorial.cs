using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TapOnObjectTutorial : MonoBehaviour
{
    public enum Stages
    {
        WaitingForStart = 0,
        Tapping = 1,
        End = 2,
    }
    GameObject arrowPointer, targetObject;
    public Stages currentStage;
    Action actionOnComplete;
    Action<TutorialScript.Actions> permitedActionSetter;
    CameraScript mainCamera;
    uint planetClickSubscription;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        arrowPointer = GameObject.Find("TappingPointer");
        arrowPointer.SetActive(false);
    }
    public void EnableTutorial(GameObject target, Action onComplete, Action<TutorialScript.Actions> actionRestricter)
    {
        Debug.LogError("Tapping tutorial enabled");
        targetObject = target;
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        currentStage = Stages.WaitingForStart;
        StartNextStep();
    }

    public void DisableTutorial()
    {
        arrowPointer.SetActive(false);
        GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
        currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        currentStage++;
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.Tapping:
                Debug.LogError("Tappong stage setted");
                arrowPointer.SetActive(true);
                planetClickSubscription = GesturesController.subscribeToPlanetClick(
                    (GameObject target) =>
                    {
                        Debug.LogError("Handling tutorial tap object");
                        if (target == targetObject)
                            StartNextStep();
                        else
                            Debug.LogError("something went wrong");
                    });
                permitedActionSetter(TutorialScript.Actions.Tapping);
                break;
            case Stages.End:
                arrowPointer.SetActive(false);
                GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
                permitedActionSetter(TutorialScript.Actions.Any);
                currentStage = Stages.WaitingForStart;
                actionOnComplete();
                break;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
    }
}
