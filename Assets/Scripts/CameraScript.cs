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

    private int planetClickSubscription,
                 swipeUpSubscription,
                 swipeDownSubscription,
                 swipeLeftSubscription,
                 swipeRightSubscription,
                 spreadSubscription,
                 pinchSubscription;
    private bool isSubscribedToGestures = false;
    private SwipesController swipeController;
    private PinchController pinchController;
    private TapController tapController;
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
        tapController = GameObject.Find("Controller").GetComponent<TapController>();
        pinchController = GameObject.Find("Controller").GetComponent<PinchController>();
        swipeController = GameObject.Find("Controller").GetComponent<SwipesController>();
    }

    private void _SubscribeToGestures()
    {
        if (!isSubscribedToGestures)
        {
            swipeDownSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Down, _SwipeHandler);
            swipeLeftSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Left, _SwipeHandler);
            swipeRightSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Right, _SwipeHandler);
            swipeUpSubscription = swipeController.SubscribeToSwipe(SwipesController.Swipe.Up, _SwipeHandler);
            spreadSubscription = pinchController.SubscribeToPinch(PinchController.Pinch.PinchOpen, _PinchHandler);
            pinchSubscription = pinchController.SubscribeToPinch(PinchController.Pinch.PinchClose, _PinchHandler);
            isSubscribedToGestures = true;
        }
    }

    private void _SwipeHandler(Vector2 delta) //ПЕРЕПИСАТЬ ЭТУ ПОМОЙКУ НАХУЙ
    {
        if (currentPhase == Phase.Free || currentPhase == Phase.PlanetLock)
        {
            transform.LookAt(target.transform);
            Vector3 axis = Vector3.left + Vector3.down;
            if (delta.y != 0)
            {
                Vector3 lockTargetDirection = transform.position - target.transform.position;
                float angleY = Vector3.Angle(lockTargetDirection, Vector3.up);
                if ((angleY < CameraYLimitUp && delta.y < 0) || (angleY > CameraYLimitDown && delta.y > 0))
                {
                    delta.y = 0;
                }
            }
            transform.Translate(axis * delta * Time.deltaTime * currentRotationSpeed);
            transform.LookAt(target.transform);
        }
    }

    private void _PinchHandler(float delta)
    {
        if (currentPhase == Phase.Free)
        {
            Debug.LogError("Delta:" + delta);
            transform.LookAt(target.transform);
            Vector3 targetDirection = target.transform.position - transform.position;
            targetDirection.Normalize();
            Vector3 newPos = transform.position + targetDirection * delta * zoomSpeed * Time.deltaTime;
            Vector3 newDirection = target.transform.position - newPos;
            newDirection.Normalize();
            float newDistance = Vector3.Distance(target.transform.position, newPos);
            if (newDistance > currentMinZoom && newDistance < currentMaxZoom && targetDirection == newDirection)
            {
                transform.position = newPos;
            }
            currentZoom = Vector3.Distance(transform.position, target.transform.position);
        }
    }

    private void _UnsubscribeToGestures()
    {
        if (isSubscribedToGestures)
        {
            tapController.UnsubscribeFromTap(planetClickSubscription);
            swipeController.UnsubscribeFromSwipe(swipeDownSubscription);
            swipeController.UnsubscribeFromSwipe(swipeLeftSubscription);
            swipeController.UnsubscribeFromSwipe(swipeRightSubscription);
            swipeController.UnsubscribeFromSwipe(swipeUpSubscription);
            pinchController.UnsubscribeFromPinch(spreadSubscription);
            pinchController.UnsubscribeFromPinch(pinchSubscription);
            isSubscribedToGestures = false;
        }
    }

    public void GoFree()
    {
        _SubscribeToGestures();

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
                planetClickSubscription = tapController.SubscribeToTap(TapController.Tap.Tap,
                    (GameObject target) =>
                        {
                            if (target.tag == "Planet")
                            {
                                tapController.UnsubscribeFromTap(planetClickSubscription);
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
        _UnsubscribeToGestures();
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

    public bool IsCurrentPhase(Phase phase)
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
    private IEnumerator _FollowTarget()
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

    private IEnumerator _LockOnTargetTransition(float targetDistance, Vector3 targetAngle, Action onComplete)
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
    private IEnumerator _MoveToPoint(Vector3 targetPos, Quaternion targetRot, Phase targetPhase)
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
}
