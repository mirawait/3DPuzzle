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
        ShuffleDown = 9,
        ShuffleUp = 10,
        SwipeTopRight = 11,
        SwipeTopleft = 12,
        SwipeDownRight = 13,
        SwipeDownLeft = 14,
        DoubleTap = 15,
        FreeAreaTap = 16,
        Ending
    }
    static Dictionary<uint, Action<GameObject>> subscriptionsOnObjectTaps = new Dictionary<uint, Action<GameObject>>();
    static Dictionary<uint, Tuple<Gestures, Action<Gestures>>> subscriptionsOnGestures = new Dictionary<uint, Tuple<Gestures, Action<Gestures>>>();
    CameraScript mainCamera;
    static public bool isGestureGoing = false;
    static Gestures currentGesture = Gestures.Undefined;
    private System.DateTime lastTapStartDateTime, lastTapEndDateTime;
    private System.TimeSpan singleTapInterval = System.TimeSpan.FromMilliseconds(200),
                            doubleTapInterval = System.TimeSpan.FromMilliseconds(300);
    private int tapsCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
    }

    static public uint subscribeToPlanetClick(Action<GameObject> doOnClick)
    {
        Debug.LogError("Somebody is subbing");
        uint key = 0;
        if (subscriptionsOnObjectTaps.Count != 0)
            while (subscriptionsOnObjectTaps.ContainsKey(key))
                key++;
        subscriptionsOnObjectTaps.Add(key, doOnClick);
        Debug.LogError("subscription end " + key);
        return key;
    }

    static public void unsubscribeToPlanetClick(uint key)
    {
        if (subscriptionsOnObjectTaps.ContainsKey(key))
        {
            subscriptionsOnObjectTaps.Remove(key);
            Debug.LogError("removing tap sub " + key);
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
        if ((Input.touchCount == 1) && !isGestureGoing)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if ((System.DateTime.Now - lastTapStartDateTime) < singleTapInterval)
                {
                    Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
                    {
                        Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                        _notifyAboutObjectTap(raycastHit.transform.gameObject);
                    }
                    else if (lastTapStartDateTime - lastTapEndDateTime <= doubleTapInterval)
                    {
                        _notifyAboutGesture(Gestures.DoubleTap);
                    }
                    else
                    {
                        _notifyAboutGesture(Gestures.FreeAreaTap);
                    }
                }
                lastTapEndDateTime = System.DateTime.Now;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                lastTapStartDateTime = System.DateTime.Now;
            }
        }
    }
    void _HandleSwipes()
    {
        if (Input.touchCount == 0 && currentGesture != Gestures.Undefined)
        {
            isGestureGoing = false;
            currentGesture = Gestures.Ending;
        }
        else if (Input.touchCount == 1 && (currentGesture == Gestures.Undefined || currentGesture == Gestures.SwipeLeft 
                || currentGesture == Gestures.SwipeRight || currentGesture == Gestures.SwipeUp || currentGesture == Gestures.SwipeDown
                || currentGesture == Gestures.SwipeDownLeft || currentGesture == Gestures.SwipeDownRight
                || currentGesture == Gestures.SwipeTopleft || currentGesture == Gestures.SwipeTopRight))
        {
            Touch touch = Input.GetTouch(0);
            Vector2 deltaPos = touch.deltaPosition;

            if (Mathf.Abs(Mathf.Abs(deltaPos.x) - Mathf.Abs(deltaPos.y)) < 5)
            {
                if (deltaPos.x < 0)
                {
                    if (deltaPos.y < 0)
                    {
                        currentGesture = Gestures.SwipeDownLeft;
                    }
                    else if (deltaPos.y > 0)
                    {
                        currentGesture = Gestures.SwipeTopleft;
                    }
                }
                else if (deltaPos.x > 0)
                {
                    if (deltaPos.y < 0)
                    {
                        currentGesture = Gestures.SwipeDownRight;
                    }
                    else if (deltaPos.y > 0)
                    {
                        currentGesture = Gestures.SwipeTopRight;
                    }
                }
            
                
            }
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
            {
                if (deltaPos.x < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationLeft))
                {
                    currentGesture = Gestures.SwipeLeft;
                }
                else if (deltaPos.x > 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationRight))
                {
                    currentGesture = Gestures.SwipeRight;
                }
            }
            else
            {
                if (deltaPos.y < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationDown))
                {
                    currentGesture = Gestures.SwipeDown;
                }
                else if (deltaPos.y > 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraRotationUp))
                {
                    currentGesture = Gestures.SwipeUp;
                }
            }
        }
        else if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).deltaPosition.x < 15 && Input.GetTouch(1).deltaPosition.x < 15 && 
                (currentGesture == Gestures.Undefined || currentGesture == Gestures.ShuffleDown || currentGesture == Gestures.ShuffleUp))
            {
                Touch touch0, touch1;
                if (Input.GetTouch(0).position.x <= Input.GetTouch(1).position.x)
                {
                    touch0 = Input.GetTouch(0);
                    touch1 = Input.GetTouch(1);
                }
                else
                {
                    touch1 = Input.GetTouch(0);
                    touch0 = Input.GetTouch(1);
                }

                Vector2 deltaPos0 = touch0.deltaPosition;
                Vector2 deltaPos1 = touch1.deltaPosition;


                if ((deltaPos0.y < 0) && (deltaPos1.y > 0))
                {
                    currentGesture = Gestures.ShuffleDown;
                }
                else if ((deltaPos0.y > 0) && (deltaPos1.y < 0))
                {
                    currentGesture = Gestures.ShuffleUp;
                }
            }
            else if (currentGesture == Gestures.Undefined || currentGesture == Gestures.Pinch || currentGesture == Gestures.Spread)
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
                    currentGesture = Gestures.Pinch;
                }
                if (deltaMagnitudeDiff < 0 && TutorialScript.IsActionPermitted(TutorialScript.Actions.CameraZoomIn))
                {
                    currentGesture = Gestures.Spread;
                }
            }
        }

        if (currentGesture != Gestures.Undefined)
        {
            isGestureGoing = true;
            _notifyAboutGesture(currentGesture);
            if (currentGesture == Gestures.Ending)
            {
                isGestureGoing = false;
                currentGesture = Gestures.Undefined;
            }
        }
    }
    void _notifyAboutGesture(Gestures gesture)
    {
        Tuple<Gestures, Action<Gestures>>[] values = new Tuple<Gestures, Action<Gestures>>[subscriptionsOnGestures.Values.Count];
        subscriptionsOnGestures.Values.CopyTo(values, 0);
        Debug.LogError("Norifing " + values.Length + " about " + gesture);
        foreach (Tuple<Gestures, Action<Gestures>> subscription in values)
        {
            if (subscription.Item1 == gesture)
                subscription.Item2(gesture);
        }
    }

    void _notifyAboutObjectTap(GameObject obj)
    {
        Debug.LogError("Clicked object name:" + obj.name);
        if (!TutorialScript.IsActionPermitted(TutorialScript.Actions.Tapping, obj))
        {
            Debug.Log("Click on " + obj.name + "is not permited");
            return;
        }
        Debug.LogError("Notyfiing " + subscriptionsOnObjectTaps.Count + " subs");
        Action<GameObject>[] values = new Action<GameObject>[subscriptionsOnObjectTaps.Values.Count];
        subscriptionsOnObjectTaps.Values.CopyTo(values, 0);
        foreach (Action<GameObject> actionOnClick in values)
        {
            actionOnClick(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuScript.IsOnPause())
        {
            _HandleSwipes();
            _HandleObjectTap();
        }
    }
}
