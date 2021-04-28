using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rotatable : MonoBehaviour
{
    private uint swipeUpSubscription,
                 swipeDownSubscription,
                 swipeLeftSubscription,
                 swipeRightSubscription,
                 swipeUpRightSubscription,
                 swipeDownRightSubscription,
                 swipeUpLeftSubscription,
                 swipeDownLeftSubscription,
                 shuffleUpSubscription,
                 shuffleDownSubscription;
    public void Permit(bool aroundOnly = false)
    {
        isAroundOnlyPermited = aroundOnly;
        if (!isPermited)
        {
            isPermited = true;
            swipeDownSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDown, rotationHandler);
            swipeLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeLeft, rotationHandler);
            swipeRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeRight, rotationHandler);
            swipeUpSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeUp, rotationHandler);
            swipeUpRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeTopRight, rotationHandler);
            swipeDownRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDownRight, rotationHandler);
            swipeUpLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeTopleft, rotationHandler);
            swipeDownLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDownLeft, rotationHandler);
            shuffleUpSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.ShuffleUp, rotationHandler);
            shuffleDownSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.ShuffleDown, rotationHandler);
        }
    }

    public void Forbid()
    {
        if (isPermited)
        {
            GesturesController.unsubscribeFromGesture(swipeDownSubscription);
            GesturesController.unsubscribeFromGesture(swipeLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownLeftSubscription);
            GesturesController.unsubscribeFromGesture(shuffleUpSubscription);
            GesturesController.unsubscribeFromGesture(shuffleDownSubscription);
            isPermited = false;
        }
    }

    private void OnDestroy()
    {
        if (isPermited)
        {
            GesturesController.unsubscribeFromGesture(swipeDownSubscription);
            GesturesController.unsubscribeFromGesture(swipeLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownLeftSubscription);
            GesturesController.unsubscribeFromGesture(shuffleUpSubscription);
            GesturesController.unsubscribeFromGesture(shuffleDownSubscription);
        }
    }

    void Start()
    {
    }

    void Update()
    {
        //HandleInput();
    }

    private const float rotationSpeed = 50f;

    private bool isPermited = false;
    private bool isAroundOnlyPermited = false;

    private void rotationHandler(GesturesController.Gestures gesture)
    {
        Vector3 axis = Vector3.zero;
        switch (gesture)
        {
            case GesturesController.Gestures.SwipeRight:
                if (!isAroundOnlyPermited)
                    axis = Camera.main.transform.up * -1;
                break;
            case GesturesController.Gestures.SwipeLeft:
                if (!isAroundOnlyPermited)
                    axis = Camera.main.transform.up;
                break;
            case GesturesController.Gestures.SwipeUp:
                if (!isAroundOnlyPermited)
                    axis = Camera.main.transform.right;
                break;
            case GesturesController.Gestures.SwipeDown:
                if (!isAroundOnlyPermited)
                    axis = Camera.main.transform.right * -1;
                break;
            case GesturesController.Gestures.SwipeTopleft:
                if (!isAroundOnlyPermited)
                {
                    axis += Camera.main.transform.up;
                    axis += Camera.main.transform.right;
                }
                break;
            case GesturesController.Gestures.SwipeTopRight:
                if (!isAroundOnlyPermited)
                {
                    axis += Camera.main.transform.right;
                    axis += Camera.main.transform.up * -1;
                }
                break;
            case GesturesController.Gestures.SwipeDownLeft:
                if (!isAroundOnlyPermited)
                {
                    axis += Camera.main.transform.up;
                    axis += Camera.main.transform.right * -1;
                }
                break;
            case GesturesController.Gestures.SwipeDownRight:
                if (!isAroundOnlyPermited)
                {
                    axis += Camera.main.transform.up * -1;
                    axis += Camera.main.transform.right * -1;
                }
                break;
            case GesturesController.Gestures.ShuffleUp:
                axis = Camera.main.transform.forward * -1;
                break;
            case GesturesController.Gestures.ShuffleDown:
                axis = Camera.main.transform.forward;
                break;
        }
        Rotate(axis);
    }

    void Rotate(Vector3 axis)
    {
        transform.Rotate(axis * rotationSpeed * Time.deltaTime, Space.World);
    }
}
