using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.Events;
public class TapOnObjectTutorial : MonoBehaviour
{
    public enum Stages
    {
        WaitingForStart = 0,
        Tapping = 1,
        End = 2,
    }
    GameObject targetObject;
    public Stages currentStage;
    Action actionOnComplete;
    Action<List<Tuple<TutorialScript.Actions, GameObject>>> permitedActionSetter;
    TapController tapController;
    CameraScript mainCamera;
    GameObject tapImage;
    int tapSubscription;
    UnityAction onClick;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        tapImage = GameObject.Find("TapImage");
        tapImage.SetActive(false);
        tapController = GameObject.Find("Controller").GetComponent<TapController>();
        onClick = () =>
        {
            StartNextStep();
        };
    }
    public void EnableTutorial(GameObject target, Action onComplete, Action<List<Tuple<TutorialScript.Actions, GameObject>>> actionRestricter)
    {
        targetObject = target;
        actionOnComplete = onComplete;
        permitedActionSetter = actionRestricter;
        currentStage = Stages.WaitingForStart;
        StartNextStep();
    }

    public void DisableTutorial()
    {
        //arrowPointer.SetActive(false);
        tapImage.SetActive(false);
        tapController.UnsubscribeFromTap(tapSubscription);
        currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        List<Tuple<TutorialScript.Actions, GameObject>> newPermittedActions = new List<Tuple<TutorialScript.Actions, GameObject>>();
        currentStage++;
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.Tapping:
                tapImage.SetActive(true);
                StartCoroutine(_updateTapImageCords());
                if (targetObject.GetComponent<Button>() != null)
                {
                    targetObject.GetComponent<Button>().onClick.AddListener(onClick);
                }
                else
                {
                    tapSubscription = tapController.SubscribeToTap(TapController.Tap.Tap,
                        (GameObject target) =>
                        {
                            if (target == targetObject)
                            {
                                StopCoroutine(_updateTapImageCords());

                                tapController.UnsubscribeFromTap(tapSubscription);
                                StartNextStep();
                            }
                        });
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.Twist, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.PinchClose, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.PinchOpen, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeDown, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeLeft, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeRight, null));
                    newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.SwipeUp, null));
                }
                newPermittedActions.Add(new Tuple<TutorialScript.Actions, GameObject>(TutorialScript.Actions.Tap, targetObject));
                permitedActionSetter(newPermittedActions);
                break;
            case Stages.End:
                tapImage.SetActive(false);
                if (targetObject.GetComponent<Button>() != null)
                {
                    targetObject.GetComponent<Button>().onClick.RemoveListener(onClick);
                }
                permitedActionSetter(newPermittedActions);
                currentStage = Stages.WaitingForStart;
                actionOnComplete();
                break;
        }
    }

    IEnumerator _updateTapImageCords()
    {
        while (true)
        {
            if (targetObject.GetComponent<Button>() != null)
            {
                tapImage.transform.position = targetObject.transform.position;
            }
            else
            {
                tapImage.transform.position = Camera.main.WorldToScreenPoint(targetObject.transform.position);
            }
            yield return new WaitForSeconds(0.001f);
        }
        yield break;
    }
}
