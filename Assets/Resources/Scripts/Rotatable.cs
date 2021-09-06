using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rotatable : MonoBehaviour
{
    private int swipeUpSubscription;
    private int swipeDownSubscription;
    private int swipeLeftSubscription;
    private int swipeRightSubscription;
    private int twistSubscription;
    private const float rotationSpeed = 5f;
    private const float twistSpeed = 15f;
    private bool isPermited = false;
    private bool isAroundOnlyPermited = false;
    private SwipesController swipeController;
    private TwistController twistController;

    private void Start()
    {
        
    }

    public void Permit(bool aroundOnly = false)
    {
        isAroundOnlyPermited = aroundOnly;
        if (!isPermited)
        {
            swipeController = GameObject.Find("Controller").GetComponent<SwipesController>();
            twistController = GameObject.Find("Controller").GetComponent<TwistController>();
            isPermited = true;
            swipeDownSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Down, _SwipeHandler);
            swipeLeftSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Left, _SwipeHandler);
            swipeRightSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Right, _SwipeHandler);
            swipeUpSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Up, _SwipeHandler);
            twistSubscription = twistController.SubscribeToTwist(TwistController.Twist.Twist, _TwistHandler);
        }
    }

    public void Forbid()
    {
        if (isPermited)
        {
            swipeController.UnsubscribeFromSwipe(swipeDownSubscription);
            swipeController.UnsubscribeFromSwipe(swipeLeftSubscription);
            swipeController.UnsubscribeFromSwipe(swipeRightSubscription);
            swipeController.UnsubscribeFromSwipe(swipeUpSubscription);
            twistController.UnsubscribeFromTwist(twistSubscription);
            isPermited = false;
        }
    }

    private void OnDestroy()
    {
        if (isPermited)
        {
            swipeController.UnsubscribeFromSwipe(swipeDownSubscription);
            swipeController.UnsubscribeFromSwipe(swipeLeftSubscription);
            swipeController.UnsubscribeFromSwipe(swipeRightSubscription);
            swipeController.UnsubscribeFromSwipe(swipeUpSubscription);
            twistController.UnsubscribeFromTwist(twistSubscription);
        }
    }

    private void _SwipeHandler(Vector2 delta)
    {
        if (!isAroundOnlyPermited)
        {
            transform.Rotate(Camera.main.transform.right * delta.y * rotationSpeed * Time.deltaTime, Space.World);
            transform.Rotate(Camera.main.transform.up * -1 * delta.x * rotationSpeed * Time.deltaTime, Space.World);
        }
    }
    
    private void _TwistHandler(float angle)
    {
        Vector3 axis = Camera.main.transform.position - transform.position;
        axis.Normalize();
        transform.RotateAround(transform.position, axis, angle * twistSpeed * Time.deltaTime);

    }
}
