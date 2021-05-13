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
    Action<List<Tuple<GesturesController.Gestures, GameObject>>> permitedActionSetter;
    CameraScript mainCamera;
    GameObject tapImage;
    uint planetClickSubscription;
    UnityAction onClick;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
        tapImage = GameObject.Find("TapImage");
        tapImage.SetActive(false);

        onClick = () =>
        {
            StartNextStep();
        };
    }
    public void EnableTutorial(GameObject target, Action onComplete, Action<List<Tuple<GesturesController.Gestures, GameObject>>> actionRestricter)
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
        //arrowPointer.SetActive(false);
        tapImage.SetActive(false);
        GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
        currentStage = Stages.WaitingForStart;
    }
    public void StartNextStep()
    {
        List<Tuple<GesturesController.Gestures, GameObject>> newPermittedActions = new List<Tuple<GesturesController.Gestures, GameObject>>();
        currentStage++;
        switch (currentStage)
        {
            case Stages.WaitingForStart:
                break;
            case Stages.Tapping:
                Debug.LogError("Tappong stage setted");
                //arrowPointer.SetActive(true);
                tapImage.SetActive(true);
                StartCoroutine(_updateTapImageCords());
                if (targetObject.GetComponent<Button>() != null)
                {
                    targetObject.GetComponent<Button>().onClick.AddListener(onClick);
                }
                else
                {
                    planetClickSubscription = GesturesController.subscribeToPlanetClick(
                        (GameObject target) =>
                        {
                            Debug.LogError("Handling tutorial tap object");
                            if (target == targetObject)
                            {
                                StopCoroutine(_updateTapImageCords());
                                
                                GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
                                StartNextStep();
                            }
                            else
                                Debug.LogError("something went wrong");
                        });
                    newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.Pinch, null));
                    newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.Spread, null));
                }
                newPermittedActions.Add(new Tuple<GesturesController.Gestures, GameObject>(GesturesController.Gestures.Tapping, targetObject));
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

    // Update is called once per frame
    void LateUpdate()
    {
    }
}
