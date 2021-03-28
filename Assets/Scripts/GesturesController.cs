using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GesturesController : MonoBehaviour
{
    public enum Gestures
    {
        Undefined = 0,
        SwipeUp = 1,
        SwipeDown = 2,
        SwipeLeft = 3,
        SwipeRight = 4,
        Pinch = 5,
        Spread = 6,
        Tapping = 7,
        Any = 8,
        Ending
    }
    static Dictionary<uint, Action<GameObject>> subscriptionsOnObjectTaps = new Dictionary<uint, Action<GameObject>>();
    static Dictionary<uint, Tuple<Gestures, Action<Gestures>>> subscriptionsOnGestures = new Dictionary<uint, Tuple<Gestures, Action<Gestures>>>();
    CameraScript mainCamera;
    static public bool isGestureGoing = false;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
    }

    static public uint subscribeToPlanetClick(Action<GameObject> doOnClick)
    {
        uint key = 0;
        if (subscriptionsOnObjectTaps.Count != 0)
            while (subscriptionsOnObjectTaps.ContainsKey(key))
                key++;
        subscriptionsOnObjectTaps.Add(key, doOnClick);
        return key;
    }

    static public void unsubscribeToPlanetClick(uint key)
    {
        if (subscriptionsOnObjectTaps.ContainsKey(key))
        {
            subscriptionsOnObjectTaps.Remove(key);
        }
    }

    static public uint subscribeToGesture(Gestures gesture, Action<Gestures> action)
    {
        uint key = 0;
        if (subscriptionsOnGestures.Count != 0)
            while (subscriptionsOnGestures.ContainsKey(key))
                key++;
        subscriptionsOnGestures.Add(key, new Tuple<Gestures, Action<Gestures>>(gesture, action));
        return key;
    }

    static public void unsubscribeFromGesture(uint key)
    {
        if (subscriptionsOnGestures.ContainsKey(key))
        {
            subscriptionsOnGestures.Remove(key);
        }
    }
    static public bool IsGestureGoing()
    {
        return isGestureGoing;
    }

    void _HandleObjectTap()
    {
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began)/* && isCurrentPhase(CameraScript.Phase.Free)*/) //ReadyForLock())
        {
            Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                //if (raycastHit.transform.gameObject.tag == "Planet")
                //{
                    Debug.Log("Clicked object name:" + raycastHit.transform.gameObject.name);
                    if (!TutorialScript.IsActionPermitted(TutorialScript.Actions.Tapping, raycastHit.transform.gameObject))
                    {
                        Debug.Log("Click on " + raycastHit.transform.gameObject.name + "is not permited");
                        return;
                    }
                    foreach (Action<GameObject> actionOnClick in subscriptionsOnObjectTaps.Values)
                    {
                        actionOnClick(raycastHit.transform.gameObject);
                    }
                    //FocusOn(raycastHit.transform.gameObject);
                    //StartCoroutine(_WaitForCameraLock(raycastHit.transform.gameObject.GetComponent<PlanetScript>().GetIndex()));
                //}
            }
        }
        
    }
    void _HandleSwipes()
    {
        Gestures gesture = Gestures.Undefined;

        if (Input.touchCount == 0 && isGestureGoing)
        {
            isGestureGoing = false;
            gesture = Gestures.Ending;
        }
        else if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 deltaPos = touch.deltaPosition;


            if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
            {
                if (deltaPos.x < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationLeft))
                {
                    gesture = Gestures.SwipeRight;
                }
                else if (deltaPos.x > 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationRight))
                {
                    gesture = Gestures.SwipeLeft;
                }
            }
            else
            {
                if (deltaPos.y < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationDown))
                {
                    gesture = Gestures.SwipeUp;
                }
                else if (deltaPos.y > 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationUp))
                {
                    gesture = Gestures.SwipeDown;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition,
                    touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude,
                  touchDeltaMag = (touchZero.position - touchOne.position).magnitude,
                  deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (deltaMagnitudeDiff > 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraZoomOut))
            {
                gesture = Gestures.Pinch;
            }
            if (deltaMagnitudeDiff < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraZoomIn))
            {
                gesture = Gestures.Spread;
            }
        }

        foreach (Tuple<Gestures, Action<Gestures>> subscription in subscriptionsOnGestures.Values)
        {
            if (subscription.Item1 == gesture)
                subscription.Item2(gesture);
        }
    }

    // Update is called once per frame
    void Update()
    {
        _HandleObjectTap();
        _HandleSwipes();
    }
}
