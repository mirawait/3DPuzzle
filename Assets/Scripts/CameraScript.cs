using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System;


public class CameraScript : MonoBehaviour
{
    public float solarRotationSpeed = 0.8f,
                 planetRotationSpeed = 0.4f,
                 zoomSpeed = 0.2f,
                 minSolarZoom = 0.1f,
                 maxSolarZoom = 179.9f,
                 minPlanetZoom = 0.1f,
                 maxPlanetZoom = 90f,
                 CameraYLimitUp = 50,
                 CameraYLimitDown = 90;
    public Vector3 focusOffset;
    public GameObject camStartPos, camSettingsPos;
    
    public enum Phase
    {
        Menu,
        Settings,
        Free,
        PlanetLock,
        PuzzleLock,
        GoingMenu,
        GoingSettings,
        GoingFree,
        GoingPlanetLock
    }
    private IEnumerator curentCoroutine = null;
    private Phase currentPhase = Phase.Menu;
    private float currentMaxZoom,
                  currentMinZoom,
                  currentZoom,
                  currentRotationSpeed;
                  
    private uint planetClickSubscription,
                 swipeUpSubscription,
                 swipeDownSubscription,
                 swipeLeftSubscription,
                 swipeRightSubscription,
                 swipeUpLeftSubscription,
                 swipeDownLeftSubscription,
                 swipeUpRightSubscription,
                 swipeDownRightSubscription,
                 spreadSubscription,
                 pinchSubscription;
    private bool isSubscribedToGestures = false;
    
    //private float CameraXLimitRight = 21;
    //private float CameraXLimitLeft = -21;
    private GameObject sun,
                       target;
    public Camera camera;


    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.Find("Sun");
        camera = GetComponent<Camera>();
        
    }

    void subscribeToGestures()
    {
        if (!isSubscribedToGestures)
        {
            Action<GesturesController.Gestures, Vector2> rotationHandler = (GesturesController.Gestures gesture, Vector2 delta) =>
            {
                if (currentPhase == Phase.Free || currentPhase == Phase.PlanetLock)
                {
                    transform.LookAt(target.transform);
                    Vector3 axis = Vector3.zero;
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
                            axis = Vector3.left + Vector3.down;
                            break;
                    }
                    if (delta.y != 0)
                    {
                        Vector3 lockTargetDirection = transform.position - target.transform.position;
                        float angleY = Vector3.Angle(lockTargetDirection, Vector3.up);
                        Debug.Log(CameraYLimitUp + "<" + angleY + "<" + CameraYLimitDown);
                        if ((angleY < CameraYLimitUp && delta.y < 0) || (angleY > CameraYLimitDown && delta.y > 0))
                        {
                            delta.y = 0;
                        }
                    }
                    transform.Translate(axis * delta * Time.deltaTime * currentRotationSpeed);
                    transform.LookAt(target.transform);
                }
            };

            Action<GesturesController.Gestures, Vector2> zoomHandler = (GesturesController.Gestures gesture, Vector2 delta) =>
            {
                if (currentPhase == Phase.Free)
                {
                    transform.LookAt(target.transform);
                    int zoomDIr = 0;
                    switch (gesture)
                    {
                        case GesturesController.Gestures.Spread:
                            zoomDIr = -1;
                            break;
                        case GesturesController.Gestures.Pinch:
                            zoomDIr = 1;
                            break;
                    }
                    Vector3 targetDirection = transform.position - target.transform.position;
                    targetDirection.Normalize();
                    Vector3 newPos = transform.position + targetDirection * zoomDIr * delta.sqrMagnitude * zoomSpeed * Time.deltaTime;
                    Vector3 newDirection = newPos - target.transform.position;
                    newDirection.Normalize();
                    float newDistance = Vector3.Distance(target.transform.position, newPos);
                    if (newDistance > currentMinZoom && newDistance < currentMaxZoom && targetDirection == newDirection)
                        transform.position = newPos;
                    currentZoom = Vector3.Distance(transform.position, target.transform.position);
                }
            };

            swipeDownSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDown, rotationHandler);
            swipeLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeLeft, rotationHandler);
            swipeRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeRight, rotationHandler);
            swipeUpSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeUp, rotationHandler);

            swipeUpLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeTopleft, rotationHandler);
            swipeDownLeftSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDownLeft, rotationHandler);
            swipeUpRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeTopRight, rotationHandler);
            swipeDownRightSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.SwipeDownRight, rotationHandler);

            spreadSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.Spread, zoomHandler);
            pinchSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.Pinch, zoomHandler);
            isSubscribedToGestures = true;
        }
    }

    void unsubscribeToGestures()
    {
        if (isSubscribedToGestures)
        {
            GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownSubscription);
            GesturesController.unsubscribeFromGesture(swipeLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpSubscription);

            GesturesController.unsubscribeFromGesture(swipeUpLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownLeftSubscription);
            GesturesController.unsubscribeFromGesture(swipeUpRightSubscription);
            GesturesController.unsubscribeFromGesture(swipeDownRightSubscription);

            GesturesController.unsubscribeFromGesture(spreadSubscription);
            GesturesController.unsubscribeFromGesture(pinchSubscription);
            isSubscribedToGestures = false;
        }
    }

    public void GoFree()
    {
        subscribeToGestures();

        currentPhase = Phase.GoingFree;
        currentMaxZoom = maxSolarZoom;
        currentMinZoom = minSolarZoom;
        currentRotationSpeed = solarRotationSpeed;
        target = sun;

        Debug.Log("Going free");
        if (curentCoroutine != null)
            StopCoroutine(curentCoroutine);
        curentCoroutine = _LockOnTargetTransition(maxSolarZoom, new Vector3(0, 60, 0),
            () =>
            {
                currentPhase = Phase.Free;
                planetClickSubscription = GesturesController.subscribeToPlanetClick(
                    (GameObject target) => 
                        {
                            if (target.tag == "Planet")
                            {
                                GesturesController.unsubscribeToPlanetClick(planetClickSubscription);
                                FocusOn(target);
                            }
                        });
            });
        StartCoroutine(curentCoroutine);
    }
    public void GoMenu()
    {
        currentPhase = Phase.GoingMenu;
        //transform.position = camStartPos.transform.position;
        //transform.rotation = camStartPos.transform.rotation;
        if (curentCoroutine != null)
            StopCoroutine(curentCoroutine);
        curentCoroutine = _MoveToPoint(camStartPos.transform.position, camStartPos.transform.rotation, Phase.Menu);
        unsubscribeToGestures();
        StartCoroutine(curentCoroutine);
    }
    public void GoSettings()
    {
        currentPhase = Phase.GoingSettings;
        if (curentCoroutine != null)
            StopCoroutine(curentCoroutine);
        curentCoroutine = _MoveToPoint(camSettingsPos.transform.position, camSettingsPos.transform.rotation, Phase.Settings);
        StartCoroutine(curentCoroutine);
    }
    
    public bool isCurrentPhase(Phase phase)
    {
        return (currentPhase == phase);
    }
    public void FocusOn(GameObject newTarget)
    {
        currentPhase = Phase.GoingPlanetLock;
        currentMaxZoom = maxPlanetZoom;
        currentMinZoom = minPlanetZoom;
        currentRotationSpeed = planetRotationSpeed;
        target = newTarget;

        Debug.Log("Going planet lock");
        curentCoroutine = _LockOnTargetTransition(maxPlanetZoom, new Vector3(0, 90, 0),
            () =>
            {
                currentPhase = Phase.PlanetLock;
                curentCoroutine = _FollowTarget();
                StartCoroutine(curentCoroutine);
                //planetClickSubscription = SolarSystemController.subscribeToPlanetClick((GameObject target) => { FocusOn(target); });
            });
        StartCoroutine(curentCoroutine);
    }
    public void EnablePuzzleLock(bool enable)
    {
        if (enable)
        {
            currentPhase = Phase.PuzzleLock;
            if (curentCoroutine != null)
                StopCoroutine(curentCoroutine);
        }
        else
        {
            currentPhase = Phase.PlanetLock;
            curentCoroutine = _FollowTarget();
            StartCoroutine(curentCoroutine);
        }
    }
    IEnumerator _FollowTarget()
    {
        while (true)
        {
            var multiplier = Vector3.Distance(camera.transform.position, sun.transform.position) / Vector3.Distance(target.transform.position, sun.transform.position);
            transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), target.GetComponent<PlanetScript>().solarRotationSpeed * Time.deltaTime * multiplier);

            //transform.LookAt(focusPoint);
            float distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position),
                      diffTargetDistance = currentZoom - distanceFromLockTarget;
            Vector3 targetDir = (transform.position - target.transform.position);
            targetDir.Normalize();
            if (distanceFromLockTarget != currentZoom)
            {
                //Debug.Log("diffDistance" + diffTargetDistance);
                Vector3 targetPos = transform.position + targetDir * diffTargetDistance;
                transform.position = targetPos;//Vector3.MoveTowards(transform.position, targetPos, target.GetComponent<PlanetScript>().solarRotationSpeed);
            }

            Vector3 focusDir = targetDir;
            focusDir.y = 0;
            focusDir *= (targetDir.magnitude / focusDir.magnitude);
            Vector3 focusPoint = Quaternion.AngleAxis(90, Vector3.up) * focusDir;
            focusPoint = target.transform.position + new Vector3(focusPoint.x * focusOffset.x, focusPoint.y * focusOffset.y, focusPoint.z * focusOffset.z);
            //focusSphere.transform.position = focusPoint;
            
            transform.LookAt(focusPoint);
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator _LockOnTargetTransition(float targetDistance, Vector3 targetAngle, Action onComplete)
    {
        bool rotatedTo = false, 
             rotatedAround = false, 
             movedTo = false;
        while (true)
        {
            Debug.Log("_LockOnTargetTransition WHILE");

            Vector3 targetDir = (transform.position - target.transform.position);
            targetDir.Normalize();

            float distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position);
            float diffTargetDistance = targetDistance - distanceFromLockTarget;
            
            Debug.Log("_LockOnTargetTransition diffDistance" + diffTargetDistance);

            Vector3 targetPos = transform.position + targetDir * diffTargetDistance;

            if (!rotatedTo)
            {
                float angleY = Vector3.Angle(targetDir, Vector3.up);
                float diffAngle = angleY - targetAngle.y;
                Debug.Log("_LockOnTargetTransition diffAngle" + diffAngle);
                targetPos += new Vector3(0, 1, 0) * currentRotationSpeed * diffAngle * Time.deltaTime;
                rotatedTo = (Mathf.Abs(diffAngle) < 5);
            }
            else
                transform.LookAt(target.transform);

            if (!movedTo)
            {
                
                transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed * 800 * Time.deltaTime);
                distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position);
                movedTo = Mathf.RoundToInt(Mathf.Abs(targetDistance - distanceFromLockTarget)) == 0;
            }

            if (!rotatedTo)
            {
                Quaternion targetRot = Quaternion.LookRotation(-targetDir);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, currentRotationSpeed * Time.deltaTime);

                rotatedTo = transform.rotation == targetRot;
            }
            //transform.RotateAround(sun.transform.position, new Vector3(1, 0 ,0), rotationSpeed * 0.12f * Time.deltaTime);
            transform.LookAt(target.transform);

            if (movedTo && rotatedTo)
            {
                
                //currentPhase = targetPhase;
                currentZoom = targetDistance;
                Debug.Log("_LockOnTargetTransition end");
                onComplete();
                yield break;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }
    IEnumerator _MoveToPoint(Vector3 targetPos, Quaternion targetRot, Phase targetPhase)
    {
        float initDist = Vector3.Distance(transform.position, targetPos);
        while (true)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed * 800 * Time.deltaTime);
            Debug.Log("CAMERA POSITION = " + transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, currentRotationSpeed * 50 * (initDist - dist) / initDist * Time.deltaTime);

            if (transform.position == targetPos && transform.rotation == targetRot)
            {
                Debug.Log("_START POSITION SETTED");
                currentPhase = targetPhase;
                yield break;
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    

    // Update is called once per frame
    void Update()
    {
    }
}
