using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PinchController : MonoBehaviour
{
    public enum Pinch
    {
        None = 0,
        PinchClose = 1,
        PinchOpen = 2,
        Ending = 3
    }
    static Dictionary<int, Tuple<Pinch, Action<float>>> subscriptionsOnPinches = new Dictionary<int, Tuple<Pinch, Action<float>>>();
    static private bool isGestureGoing = false;
    [SerializeField] private float minPinchSpreadDeltapos = 0.2f;

    public int SubscribeToPinch(Pinch pitchGesture, Action<float> action)
    {
        int key = 0;
        if (subscriptionsOnPinches.Count != 0)
            while (subscriptionsOnPinches.ContainsKey(key))
                key++;
        subscriptionsOnPinches.Add(key, new Tuple<Pinch, Action<float>>(pitchGesture, action));
        
        return key;
    }

    public void UnsubscribeFromPinch(int key)
    {
        if (subscriptionsOnPinches.ContainsKey(key))
        {
            subscriptionsOnPinches.Remove(key);
        }
    }

    public bool IsGestureGoing()
    {
        return isGestureGoing;
    }

    void _HandlePinches()
    {
        Pinch resultGesture = Pinch.None;
        float resultDelta = 0;

        if (Input.touchCount == 0 && isGestureGoing)
        {
            isGestureGoing = false;
            resultGesture = Pinch.Ending;
        }
        else if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            var touch0Direction = touch0.position - touch0.deltaPosition;
            var touch1Direction = touch1.position - touch1.deltaPosition;

            var dstBtwTouchesPosition = Vector2.Distance(touch0.position, touch1.position);
            var dstBtwTouchesDirections = Vector2.Distance(touch0Direction, touch1Direction);

            resultDelta = dstBtwTouchesPosition - dstBtwTouchesDirections;

            if (resultDelta > minPinchSpreadDeltapos)
            {
                resultGesture = Pinch.PinchOpen;
            }
            else if (resultDelta < minPinchSpreadDeltapos * -1)
            {
                resultGesture = Pinch.PinchClose;
            }
            else
            {
                resultDelta = 0;
            }
        }
        if (resultGesture != Pinch.None && _IsActionPermited(resultGesture))
        {
            isGestureGoing = (resultGesture != Pinch.Ending);
            _NotifyAboutPinch(resultGesture, resultDelta);
        }
        
    }

    private bool _IsActionPermited(Pinch gesture)
    {
        bool result = false;
        switch (gesture)
        {
            case Pinch.PinchClose:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.PinchClose);
                break;
            case Pinch.PinchOpen:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.PinchOpen);
                break;
            default:
                result = true;
                break;
        }
        return result;
    }

    void _NotifyAboutPinch(Pinch pinch, float pinchInfo)
    {
        Debug.LogError("Notifing " + subscriptionsOnPinches.Values.Count + " about " + pinch);
        Tuple<Pinch, Action<float>>[] handlers = new Tuple<Pinch, Action<float>>[subscriptionsOnPinches.Values.Count];
        subscriptionsOnPinches.Values.CopyTo(handlers, 0);
        foreach (Tuple<Pinch, Action<float>> handler in handlers)
        {
            if (handler.Item1 == pinch)
            {
                handler.Item2(pinchInfo);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuScript.IsOnPause())
        {
            _HandlePinches();
        }
    }
}
