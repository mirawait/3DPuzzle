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
    static Dictionary<uint, Tuple<Gestures, Action<Gestures, Vector2>>> subscriptionsOnGestures = new Dictionary<uint, Tuple<Gestures, Action<Gestures, Vector2>>>();
    CameraScript mainCamera;
    static public bool isGestureGoing = false;
    static Gestures currentGesture = Gestures.Undefined;
    private System.DateTime lastTapStartDateTime, lastTapEndDateTime;
    private System.TimeSpan singleTapInterval = System.TimeSpan.FromMilliseconds(200),
                            doubleTapInterval = System.TimeSpan.FromMilliseconds(300);
    private float minTouchDeltapos = 0.2f;
    private Vector2 firstTouchStartPos = new Vector2(-1, -1), 
                    secondTouchStartPos = new Vector2(-1, -1);
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

    static public uint subscribeToGesture(Gestures gesture, Action<Gestures, Vector2> action)
    {
        uint key = 0;
        if (subscriptionsOnGestures.Count != 0)
            while (subscriptionsOnGestures.ContainsKey(key))
                key++;
        subscriptionsOnGestures.Add(key, new Tuple<Gestures, Action<Gestures, Vector2>>(gesture, action));
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
                        if (TutorialScript.IsActionPermitted(Gestures.DoubleTap))
                            _notifyAboutGesture(Gestures.DoubleTap, Vector2.zero);
                    }
                    else
                    {
                        if (TutorialScript.IsActionPermitted(Gestures.FreeAreaTap))
                            _notifyAboutGesture(Gestures.FreeAreaTap, Vector2.zero);
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
        Vector2 swipeDelta = Vector2.zero;
        if (Input.touchCount == 0 && currentGesture != Gestures.Undefined)
        {
            isGestureGoing = false;
            currentGesture = Gestures.Ending;
        }
        if (Input.touchCount == 1  && Input.GetTouch(0).deltaPosition.magnitude > minTouchDeltapos && (currentGesture == Gestures.Undefined || currentGesture == Gestures.SwipeLeft
                || currentGesture == Gestures.SwipeRight || currentGesture == Gestures.SwipeUp || currentGesture == Gestures.SwipeDown
                || currentGesture == Gestures.SwipeDownLeft || currentGesture == Gestures.SwipeDownRight
                || currentGesture == Gestures.SwipeTopleft || currentGesture == Gestures.SwipeTopRight))
        {
            Touch touch = Input.GetTouch(0);

            Vector2 deltaPos = touch.deltaPosition;
            if (deltaPos.sqrMagnitude < minTouchDeltapos)
            {
                return;
            }
            //else if (Mathf.Abs(Mathf.Abs(deltaPos.x) - Mathf.Abs(deltaPos.y)) < 5)
            //{
            //    if (deltaPos.x < 0)
            //    {
            //        if (deltaPos.y < 0)
            //        {
            //            currentGesture = Gestures.SwipeDownLeft;
            //        }
            //        else if (deltaPos.y > 0)
            //        {
            //            currentGesture = Gestures.SwipeTopleft;
            //        }
            //    }
            //    else if (deltaPos.x > 0)
            //    {
            //        if (deltaPos.y < 0)
            //        {
            //            currentGesture = Gestures.SwipeDownRight;
            //        }
            //        else if (deltaPos.y > 0)
            //        {
            //            currentGesture = Gestures.SwipeTopRight;
            //        }
            //    }
            //}
            else if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
            {
                if (deltaPos.x < 0 && TutorialScript.IsActionPermitted(Gestures.SwipeLeft))
                {
                    currentGesture = Gestures.SwipeLeft;
                }
                else if (deltaPos.x > 0 && TutorialScript.IsActionPermitted(Gestures.SwipeRight))
                {
                    currentGesture = Gestures.SwipeRight;
                }
            }
            else
            {
                if (deltaPos.y < 0 && TutorialScript.IsActionPermitted(Gestures.SwipeDown))
                {
                    currentGesture = Gestures.SwipeDown;
                }
                else if (deltaPos.y > 0 && TutorialScript.IsActionPermitted(Gestures.SwipeUp))
                {
                    currentGesture = Gestures.SwipeUp;
                }
            }

            swipeDelta = deltaPos;
        }
        else if (Input.touchCount == 2)
        {
            if (currentGesture == Gestures.Undefined && firstTouchStartPos.Equals(new Vector2(-1,-1)) && secondTouchStartPos.Equals(new Vector2(-1, -1)))
            {
                firstTouchStartPos = Input.GetTouch(0).position;
                secondTouchStartPos = Input.GetTouch(1).position;
                return;
            }
            if (Input.GetTouch(0).deltaPosition.sqrMagnitude < minTouchDeltapos || Input.GetTouch(0).deltaPosition.sqrMagnitude < minTouchDeltapos)
                return;
                
            Debug.LogWarning("FIRST " + Input.GetTouch(0).position + ":::" + firstTouchStartPos);
            Debug.LogWarning("Second " + Input.GetTouch(1).position + ":::" + secondTouchStartPos);
            Vector3 firstTouchDir = Input.GetTouch(0).position - firstTouchStartPos;
            Vector3 secondTouchDir = Input.GetTouch(1).position - firstTouchStartPos;
           
            float angle = Vector3.Angle(firstTouchDir.normalized, secondTouchDir.normalized);
            Debug.LogWarning("ANGLE BETWEEN VECTORS:" + angle);
            if (30 < angle && angle < 150)
            {
                Debug.LogWarning("ITS SHUFFLE!");
            }
            else Debug.LogWarning("ITS ZOOM!");

            if (30 < angle && angle < 150 &&
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
                
                if (deltaPos0.sqrMagnitude <= minTouchDeltapos || deltaPos1.sqrMagnitude <= minTouchDeltapos)
                {
                    return;
                }
                //if (deltaPos0.y > deltaPos0.x && deltaPos1.y > deltaPos1.x)
                //{
                    if ((deltaPos0.y < 0) && (deltaPos1.y > 0))
                    {
                        currentGesture = Gestures.ShuffleDown;
                    }
                    else if ((deltaPos0.y > 0) && (deltaPos1.y < 0))
                    {
                        currentGesture = Gestures.ShuffleUp;
                    }
                //}
                //else
                //{
                //    if ((deltaPos0.x < 0) && (deltaPos1.x > 0))
                //    {
                //        currentGesture = Gestures.ShuffleDown;
                //    }
                //    else if ((deltaPos0.x > 0) && (deltaPos1.x < 0))
                //    {
                //        currentGesture = Gestures.ShuffleUp;
                //    }
                //}
                swipeDelta = deltaPos0;
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

                if (deltaMagnitudeDiff > minTouchDeltapos && TutorialScript.IsActionPermitted(Gestures.Pinch))
                {
                    currentGesture = Gestures.Pinch;
                }
                if (deltaMagnitudeDiff < minTouchDeltapos * -1 && TutorialScript.IsActionPermitted(Gestures.Spread))
                {
                    currentGesture = Gestures.Spread;
                }
                swipeDelta = touchZero.deltaPosition;
            }
        }
        if (currentGesture != Gestures.Undefined)
        {
            isGestureGoing = true;
            _notifyAboutGesture(currentGesture, swipeDelta);
            if (currentGesture == Gestures.Ending)
            {
                isGestureGoing = false;
                firstTouchStartPos = new Vector2(-1, -1);
                secondTouchStartPos = new Vector2(-1, -1);
                Debug.LogWarning("START POS SETTED TO DEFAULT");
                currentGesture = Gestures.Undefined;
            }
        }
    }
    void _notifyAboutGesture(Gestures gesture, Vector2 delta)
    {
        Tuple<Gestures, Action<Gestures, Vector2>>[] values = new Tuple<Gestures, Action<Gestures, Vector2>>[subscriptionsOnGestures.Values.Count];
        subscriptionsOnGestures.Values.CopyTo(values, 0);
        Debug.LogError("Norifing " + values.Length + " about " + gesture);
        foreach (Tuple<Gestures, Action<Gestures, Vector2>> subscription in values)
        {
            if (subscription.Item1 == gesture)
                subscription.Item2(gesture, delta);
        }
    }

    void _notifyAboutObjectTap(GameObject obj)
    {
        Debug.LogError("Clicked object name:" + obj.name);
        if (!TutorialScript.IsActionPermitted(Gestures.Tapping, obj))
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
