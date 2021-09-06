using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GesturesController : MonoBehaviour
{
    public enum Gestures
    {
        SwipeUp,
        SwipeDown,
        SwipeLeft,
        SwipeRight,
        PinchClose,
        PinchOpen,
        Twist,
        Tap,
        DoubleTap
    }

    TapController tapController;
    TwistController twistController = new TwistController();
    SwipesController swipesController = new SwipesController();
    PinchController pinchController = new PinchController();

    public int SubscribeToGesture(Gestures gesture, Action<float> handler)
    {
        int subResult = -1;
        switch (gesture)
        {
            case Gestures.PinchClose:
                subResult = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, handler);
                break;
            case Gestures.PinchOpen:
                subResult = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, handler);
                break;
            case Gestures.Twist:
                subResult = twistController.SubscribeToTwist(TwistController.Twist.Twist, handler);
                break;
            default:
                break;
        }
        return subResult;
    }
    public int SubscribeToGesture(Gestures gesture, Action<Vector2> handler)
    {
        int subResult = -1;
        switch (gesture)
        {
            case Gestures.SwipeUp:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Up, handler as Action<Vector2>);
                break;
            case Gestures.SwipeDown:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Down, handler as Action<Vector2>);
                break;
            case Gestures.SwipeLeft:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Left, handler as Action<Vector2>);
                break;
            case Gestures.SwipeRight:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Right, handler as Action<Vector2>);
                break;
            default:
                break;
        }
        return subResult;
    }
 
    public int SubscribeToGesture<T>(Gestures gesture, Action<T> handler)
    {
        int subResult = -1;
        switch (gesture)
        {
            case Gestures.SwipeUp:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Up, handler as Action<Vector2>);
                break;
            case Gestures.SwipeDown:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Down, handler as Action<Vector2>);
                break;
            case Gestures.SwipeLeft:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Left, handler as Action<Vector2>);
                break;
            case Gestures.SwipeRight:
                subResult = swipesController.SubscribeToSwipe(SwipesController.Swipe.Right, handler as Action<Vector2>);
                break;
            case Gestures.PinchClose:
                subResult = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, handler as Action<float>);
                break;
            case Gestures.PinchOpen:
                subResult = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, handler as Action<float>);
                break;
            case Gestures.Twist:
                subResult = twistController.SubscribeToTwist(TwistController.Twist.Twist, handler as Action<float>);
                break;
            default:
                break;
        }
        return subResult;
    }
}
