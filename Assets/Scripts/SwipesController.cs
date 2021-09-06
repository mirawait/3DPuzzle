using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SwipesController : MonoBehaviour
{
    public enum Swipe
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        Ending = 5
    }
    static Dictionary<int, Tuple<Swipe, Action<Vector2>>> subscriptionsOnSwipes = new Dictionary<int, Tuple<Swipe, Action<Vector2>>>();

    static private bool isGestureGoing = false;

    [SerializeField] private float minSwipeDeltapos = 0.2f;

    public int SubscribeToSwipe(Swipe swipeGesture, Action<Vector2> action)
    {
        int key = 0;
        if (subscriptionsOnSwipes.Count != 0)
            while (subscriptionsOnSwipes.ContainsKey(key))
                key++;
        subscriptionsOnSwipes.Add(key, new Tuple<Swipe, Action<Vector2>>(swipeGesture, action));

        return key;
    }

    public void UnsubscribeFromSwipe(int key)
    {
        if (subscriptionsOnSwipes.ContainsKey(key))
        {
            subscriptionsOnSwipes.Remove(key);
        }
    }

    public bool IsGestureGoing()
    {
        return isGestureGoing;
    }

    private void _HandleSwipes()
    {
        Swipe resultGesture = Swipe.None;
        Vector2 resultDelta = new Vector2(0,0);

        if (Input.touchCount == 0 && isGestureGoing)
        {
            isGestureGoing = false;
            resultGesture = Swipe.Ending;
        }
        if (Input.touchCount == 1)
        {
            resultDelta = Input.GetTouch(0).deltaPosition;

            if (resultDelta.sqrMagnitude < minSwipeDeltapos)
            {
                resultDelta = new Vector2(0, 0);
            }
            else
            {
                if (Mathf.Abs(resultDelta.x) > Mathf.Abs(resultDelta.y))
                {
                    resultGesture = resultDelta.x < 0 ? Swipe.Left : Swipe.Right;
                }
                else
                {
                    resultGesture = resultDelta.y < 0 ? Swipe.Down : Swipe.Up;
                }
            }
            if (TutorialSwipes.IsTutorialEnabled())
            {
                resultDelta.x = (resultGesture == Swipe.Up || resultGesture == Swipe.Down) ? 0 : resultDelta.x;
                resultDelta.y = (resultGesture == Swipe.Left || resultGesture == Swipe.Right) ? 0 : resultDelta.y;
            }
        }
        if (resultGesture != Swipe.None && _IsActionPermited(resultGesture))
        {
            isGestureGoing = (resultGesture != Swipe.Ending);
            _NotifyAboutSwipe(resultGesture, resultDelta);
        }
    }

    private bool _IsActionPermited(Swipe gesture)
    {
        bool result = false;
        switch (gesture)
        {
            case Swipe.Left:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.SwipeLeft);
                break;
            case Swipe.Right:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.SwipeRight);
                break;
            case Swipe.Up:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.SwipeUp);
                break;
            case Swipe.Down:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.SwipeDown);
                break;
            default:
                result = true;
                break;
        }
        return result;
    }

    private void _NotifyAboutSwipe(Swipe swipe, Vector2 swipeInfo)
    {
        Debug.LogError("Norifing " + subscriptionsOnSwipes.Values.Count + " about " + swipe);
        Tuple<Swipe, Action<Vector2>>[] handlers = new Tuple<Swipe, Action<Vector2>>[subscriptionsOnSwipes.Values.Count];
        subscriptionsOnSwipes.Values.CopyTo(handlers, 0);
        foreach (Tuple<Swipe, Action<Vector2>> handler in handlers)
        {
            if (handler.Item1 == swipe)
            {
                handler.Item2(swipeInfo);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuScript.IsOnPause())
        {
            _HandleSwipes();
        }
    }
}
