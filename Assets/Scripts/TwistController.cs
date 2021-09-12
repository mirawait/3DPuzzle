using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TwistController : MonoBehaviour
{
    public enum Twist
    {
        None = 0,
        Twist = 1,
        Ending = 2
    }
    static Dictionary<int, Tuple<Twist, Action<float>>> subscriptionsOnTwist = new Dictionary<int, Tuple<Twist, Action<float>>>();

    static private bool isGestureGoing = false;
    [SerializeField] private float minTwistAngle = 0.2f;

    public int SubscribeToTwist(Twist twistGesture, Action<float> action)
    {
        int key = 0;
        if (subscriptionsOnTwist.Count != 0)
            while (subscriptionsOnTwist.ContainsKey(key))
                key++;
        subscriptionsOnTwist.Add(key, new Tuple<Twist, Action<float>>(twistGesture, action));

        return key;
    }

    public void UnsubscribeFromTwist(int key)
    {
        if (subscriptionsOnTwist.ContainsKey(key))
        {
            subscriptionsOnTwist.Remove(key);
        }
    }

    public bool IsGestureGoing()
    {
        return isGestureGoing;
    }

    void _HandleTwists()
    {
        Twist resultGesture = Twist.None;
        float resultAngle = 0;

        if (Input.touchCount == 0 && isGestureGoing)
        {
            isGestureGoing = false;
            resultGesture = Twist.Ending;
        }
        if (Input.touchCount == 2)
        {
            var touch0 = Input.GetTouch(0);
            var touch1 = Input.GetTouch(1);

            var touch0Direction = touch0.position - touch0.deltaPosition;
            var touch1Direction = touch1.position - touch1.deltaPosition;

            if (touch1Direction != touch1.position)
            {
                resultAngle = Vector3.SignedAngle(touch1.position - touch0.position, touch1Direction - touch0Direction, Camera.main.transform.forward);
                resultAngle = (Camera.main.transform.forward.z < 0 ? resultAngle * -1 : resultAngle);
                resultGesture = Mathf.Abs(resultAngle) > minTwistAngle ? Twist.Twist : Twist.None;
            }
        }
        if (resultGesture != Twist.None && _IsActionPermited(resultGesture))
        {
            isGestureGoing = (resultGesture != Twist.Ending);
            _NotifyAboutTwist(resultGesture, resultAngle);
        }
    }

    private bool _IsActionPermited(Twist gesture)
    {
        bool result = false;
        switch (gesture)
        {
            case Twist.Twist:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.Twist);
                break;
            default: 
                result = true;
                break;
        }
        return result;
    }

    void _NotifyAboutTwist(Twist twistGesture, float angle)
    {
        Debug.LogError("Notifing " + subscriptionsOnTwist.Values.Count + " about twist");
        Tuple<Twist, Action<float>>[] handlers = new Tuple<Twist, Action<float>>[subscriptionsOnTwist.Values.Count];
        subscriptionsOnTwist.Values.CopyTo(handlers, 0);
        foreach (Tuple<Twist, Action<float>> handler in handlers)
        {
            if (handler.Item1 == twistGesture)
            {
                handler.Item2(angle);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuScript.IsOnPause())
        {
            _HandleTwists();
        }
    }
}
