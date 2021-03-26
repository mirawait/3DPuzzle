using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
public class SolarSystemController : MonoBehaviour
{

    static Dictionary<uint, Action<GameObject>> subscriptionsOnPlanetClick = new Dictionary<uint, Action<GameObject>>();
    private CameraScript mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
    }

    static public uint subscribeToPlanetClick(Action<GameObject> doOnClick)
    {
        uint key = 0;
        if (subscriptionsOnPlanetClick.Count != 0)
            while (subscriptionsOnPlanetClick.ContainsKey(key))
                key++;
        subscriptionsOnPlanetClick.Add(key, doOnClick);
        return key;
    }

    static public void unsubscribeToPlanetClick(uint key)
    {
        if (subscriptionsOnPlanetClick.ContainsKey(key))
        {
            subscriptionsOnPlanetClick.Remove(key);
        }
    }

    void _HandlePlanetClick()
    {
        if ((Input.touchCount == 1) && (Input.GetTouch(0).phase == TouchPhase.Began)/* && isCurrentPhase(CameraScript.Phase.Free)*/) //ReadyForLock())
        {
            Ray raycast = mainCamera.camera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity))
            {
                Debug.DrawRay(transform.position, raycastHit.point, Color.red, 5f);
                if (raycastHit.transform.gameObject.tag == "Planet")
                {
                    Debug.Log("Clicked planet name:" + raycastHit.transform.gameObject.name);
                    if (!TutorialScript.IsActionPermitted(TutorialScript.Actions.Tapping, raycastHit.transform.gameObject))
                    {
                        Debug.Log("Click on " + raycastHit.transform.gameObject.name + "is not permited");
                        return;
                    }
                    foreach (Action<GameObject> actionOnClick in subscriptionsOnPlanetClick.Values)
                    {
                        actionOnClick(raycastHit.transform.gameObject);
                    }
                    //FocusOn(raycastHit.transform.gameObject);
                    //StartCoroutine(_WaitForCameraLock(raycastHit.transform.gameObject.GetComponent<PlanetScript>().GetIndex()));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        _HandlePlanetClick();
    }
}
