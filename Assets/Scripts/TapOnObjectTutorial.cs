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
        targetObject = target;
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        StartNextStep();
    }

    public void DisableTutorial()
    {
        arrowPointer.SetActive(false);
        SolarSystemController.unsubscribeToPlanetClick(planetClickSubscription);
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
                arrowPointer.SetActive(true);
                planetClickSubscription = SolarSystemController.subscribeToPlanetClick(
                    (GameObject target) =>
                    {
                        if (target == targetObject)
                            StartNextStep();
                    });
                permitedActionSetter(TutorialScript.Actions.Tapping);
                break;
            case Stages.End:
                arrowPointer.SetActive(false);
                SolarSystemController.unsubscribeToPlanetClick(planetClickSubscription);
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
