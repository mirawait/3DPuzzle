using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TapController : MonoBehaviour
{
    public enum Tap
    {
        None = 0,
        Tap = 1,
        DoubleTap = 2,
        Ending = 3
    }
    static Dictionary<int, Tuple<Tap, Action<GameObject>>> subscriptionsOnTaps = new Dictionary<int, Tuple<Tap, Action<GameObject>>>();

    private System.DateTime lastTapStartDateTime, lastTapEndDateTime;

    [SerializeField] private System.TimeSpan singleTapInterval = System.TimeSpan.FromMilliseconds(200);
    [SerializeField] private System.TimeSpan doubleTapInterval = System.TimeSpan.FromMilliseconds(300);

    public int SubscribeToTap(Tap tapGesture, Action<GameObject> action)
    {
        int key = 0;
        if (subscriptionsOnTaps.Count != 0)
            while (subscriptionsOnTaps.ContainsKey(key))
                key++;
        subscriptionsOnTaps.Add(key, new Tuple<Tap, Action<GameObject>>(tapGesture, action));

        return key;
    }

    public void UnsubscribeFromTap(int key)
    {
        if (subscriptionsOnTaps.ContainsKey(key))
        {
            subscriptionsOnTaps.Remove(key);
        }
    }

    void _HandleTaps()
    {
        if (Input.touchCount == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                if ((System.DateTime.Now - lastTapStartDateTime) < singleTapInterval)
                {
                    GameObject tapTarget = null;
                    Tap resultGesture = Tap.Tap;

                    Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                    RaycastHit raycastHit;
                    if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
                    {
                        Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                        tapTarget = raycastHit.transform.gameObject;
                    }
                    if (lastTapStartDateTime - lastTapEndDateTime <= doubleTapInterval)
                    {
                        resultGesture = Tap.DoubleTap;
                    }
                    if (_IsActionPermited(resultGesture, tapTarget))
                    {
                        _NotifyAboutTap(resultGesture, tapTarget);
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

    private bool _IsActionPermited(Tap gesture, GameObject target)
    {
        bool result = false;
        switch (gesture)
        {
            case Tap.Tap:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.Tap, target);
                break;
            case Tap.DoubleTap:
                result = TutorialScript.IsActionPermitted(TutorialScript.Actions.DoubleTap, target);
                break;
            default:
                result = true;
                break;
        }
        return result;
    }

    void _NotifyAboutTap(Tap tap, GameObject obj)
    {
        Debug.LogError("Notyfiing " + subscriptionsOnTaps.Count + " about" + tap);
        Tuple<Tap, Action<GameObject>>[] handlers = new Tuple<Tap, Action<GameObject>>[subscriptionsOnTaps.Values.Count];
        subscriptionsOnTaps.Values.CopyTo(handlers, 0);
        foreach (Tuple<Tap, Action<GameObject>> handler in handlers)
        {
            if (handler.Item1 == tap)
            {
                handler.Item2(obj);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!MenuScript.IsOnPause())
        {
            _HandleTaps();
        }
    }
}
