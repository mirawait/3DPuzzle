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
    }

    private const float rotationSpeed = 5f;

    private bool isPermited = false;
    private bool isAroundOnlyPermited = false;

    private void rotationHandler(GesturesController.Gestures gesture, Vector2 delta)
    {
        Vector3 axis = Vector3.zero;
        Vector2 rotatedDelta = new Vector2(delta.y, delta.x);
        switch (gesture)
        {
            case GesturesController.Gestures.SwipeRight:
            case GesturesController.Gestures.SwipeLeft:
            case GesturesController.Gestures.SwipeUp:
            case GesturesController.Gestures.SwipeDown:
            case GesturesController.Gestures.SwipeTopleft:
            case GesturesController.Gestures.SwipeTopRight:
            case GesturesController.Gestures.SwipeDownLeft:
            case GesturesController.Gestures.SwipeDownRight:
                if (!isAroundOnlyPermited)
                {
                    transform.Rotate(Camera.main.transform.right * delta.y * rotationSpeed * Time.deltaTime, Space.World);
                    transform.Rotate(Camera.main.transform.up * -1 * delta.x * rotationSpeed * Time.deltaTime, Space.World);
                }
                break;
            case GesturesController.Gestures.ShuffleUp:
                axis = Camera.main.transform.forward * -1;
                transform.Rotate(axis  * (rotatedDelta.sqrMagnitude / 8) * Time.deltaTime, Space.World);
                break;
            case GesturesController.Gestures.ShuffleDown:
                axis = Camera.main.transform.forward;
                transform.Rotate(axis * (rotatedDelta.sqrMagnitude / 8) * Time.deltaTime, Space.World);
                break;
        }
    }
}
