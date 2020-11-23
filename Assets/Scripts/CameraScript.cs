using System.Collections;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float rotationSpeed = 0.8f,
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
        GoingMenu,
        GoingSettings,
        GoingFree,
        GoingPlanetLock
    }
    private Phase currentPhase = Phase.Menu;
    private float currentMaxZoom,
                  currentMinZoom,
                  currentZoom;
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
    public void GoFree()
    {
        currentPhase = Phase.GoingFree;
        currentMaxZoom = maxSolarZoom;
        currentMinZoom = minSolarZoom;
        target = sun;

        Debug.Log("Going free");
        StopAllCoroutines();
        StartCoroutine(_LockOnTargetTransition(maxSolarZoom, new Vector3(0, 60, 0), Phase.Free));
    }
    public void GoMenu()
    {
        currentPhase = Phase.GoingMenu;
        //transform.position = camStartPos.transform.position;
        //transform.rotation = camStartPos.transform.rotation;
        StopAllCoroutines();
        StartCoroutine(_MoveToPoint(camStartPos.transform.position, camStartPos.transform.rotation, Phase.Menu));
    }
    public void GoSettings()
    {
        currentPhase = Phase.GoingSettings;
        StopAllCoroutines();
        StartCoroutine(_MoveToPoint(camSettingsPos.transform.position, camSettingsPos.transform.rotation, Phase.Settings));
    }
    
    public bool isCurrentPhase(Phase phase)
    {
        return (currentPhase == phase);
    }

    //public bool IsCurrentPhaseMenu()
    //{
    //    return (currentPhase == Phase.Menu);
    //}
    //public bool IsCurrentPhasePlanetLock()
    //{
    //    return (currentPhase == Phase.PlanetLock);
    //}
    //public bool ReadyForLock()
    //{
    //    return (currentPhase == Phase.Free);
    //}
    public void FocusOn(GameObject newTarget)
    {
        currentPhase = Phase.GoingPlanetLock;
        currentMaxZoom = maxPlanetZoom;
        currentMinZoom = minPlanetZoom;
        target = newTarget;

        Debug.Log("Going planet lock");
        StartCoroutine(_LockOnTargetTransition(maxPlanetZoom, new Vector3(0, 90, 0), Phase.PlanetLock));
    }
    void _FollowTarget()
    {
        float distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position),
                  diffTargetDistance = currentZoom - distanceFromLockTarget;
        Vector3 targetDir = (transform.position - target.transform.position);
        targetDir.Normalize();
        if (distanceFromLockTarget != currentZoom)
        {
            Debug.Log("diffDistance" + diffTargetDistance);
            Vector3 targetPos = transform.position + targetDir * diffTargetDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, target.GetComponent<PlanetScript>().solarRotationSpeed);
        }

        Vector3 focusDir = targetDir;
        focusDir.y = 0;
        focusDir *= (targetDir.magnitude / focusDir.magnitude);
        Vector3 focusPoint = Quaternion.AngleAxis(90, Vector3.up) * focusDir;
        focusPoint = target.transform.position + new Vector3(focusPoint.x * focusOffset.x, focusPoint.y * focusOffset.y, focusPoint.z * focusOffset.z);
        //focusSphere.transform.position = focusPoint;

        transform.RotateAround(sun.transform.position, new Vector3(0, 1, 0), target.GetComponent<PlanetScript>().solarRotationSpeed * Time.deltaTime);
        transform.LookAt(focusPoint);

    }

    IEnumerator _LockOnTargetTransition(float targetDistance, Vector3 targetAngle, Phase targetPhase)
    {
        while (true)
        {
            Debug.Log("_LockOnTargetTransition WHILE");

            Vector3 targetDir = (transform.position - target.transform.position);
            targetDir.Normalize();

            float distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position);
            float diffTargetDistance = targetDistance - distanceFromLockTarget;
            float angleY = Vector3.Angle(targetDir, Vector3.up);
            float diffAngle = angleY - targetAngle.y;
            Debug.Log("diffAngle" + diffAngle);
            Debug.Log("diffDistance" + diffTargetDistance);
            Vector3 targetPos = transform.position + targetDir * diffTargetDistance
                + new Vector3(0, 1, 0) * rotationSpeed * diffAngle * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed * 20 * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(-targetDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 3 * Time.deltaTime);

            distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position);
            if (Mathf.RoundToInt(Mathf.Abs(targetDistance - distanceFromLockTarget)) == 0 && transform.rotation == targetRot)
            {
                currentPhase = targetPhase;
                currentZoom = targetDistance;
                Debug.Log("_LockOnTargetTransition end");
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    IEnumerator _MoveToPoint(Vector3 targetPos, Quaternion targetRot, Phase targetPhase)
    {
        float initDist = Vector3.Distance(transform.position, targetPos);
        while (true)
        {
            float dist = Vector3.Distance(transform.position, targetPos);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed * 20 * Time.deltaTime);
            Debug.Log("CAMERA POSITION = " + transform.position);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * 2 * (initDist - dist) / initDist * Time.deltaTime);

            if (transform.position == targetPos && transform.rotation == targetRot)
            {
                Debug.Log("_START POSITION SETTED");
                currentPhase = targetPhase;
                yield break;
            }
            yield return new WaitForSeconds(0.01f);
        }
    }
    void _HandleZoom()
    {
        if (Input.touchCount != 2)
            return;

        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition,
                touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude,
              touchDeltaMag = (touchZero.position - touchOne.position).magnitude,
              deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        Vector3 direction = transform.position - target.transform.position;
        direction.Normalize();
        Vector3 newPos = transform.position + direction * deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;
        Vector3 newDirection = newPos - target.transform.position;
        newDirection.Normalize();
        float newDistance = Vector3.Distance(target.transform.position, newPos);
        if (newDistance > currentMinZoom && newDistance < currentMaxZoom && direction == newDirection)
            transform.position = newPos;
        currentZoom = Vector3.Distance(transform.position, target.transform.position);
    }
    void _HandleRotation()
    {
        if (Input.touchCount != 1)
            return;
        transform.LookAt(target.transform);

        Touch touch = Input.GetTouch(0);

        Vector2 deltaPos = touch.deltaPosition;

        Vector3 moveDirection = new Vector3(0, 0, 0);

        if (Mathf.Abs(deltaPos.x) > Mathf.Abs(deltaPos.y))
        {
            if (deltaPos.x < 0/* && transform.position.x < CameraXLimitRight*/)
            {
                moveDirection = Vector3.right;
            }
            else if (deltaPos.x > 0/* && transform.position.x > CameraXLimitLeft*/)
            {
                moveDirection = Vector3.left;
            }
        }
        else
        {
            Vector3 direction = transform.position - target.transform.position;
            float angleY = Vector3.Angle(direction, Vector3.up);
            //Debug.Log("CameraYLimitUp = " + CameraYLimitUp);
            if (deltaPos.y < 0 && angleY > CameraYLimitUp)
            {
                moveDirection = Vector3.up;
            }
            else if (deltaPos.y > 0 && angleY < CameraYLimitDown)
            {
                moveDirection = Vector3.down;
            }
        }
        transform.Translate(moveDirection * Time.deltaTime * rotationSpeed);
        transform.LookAt(target.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            Vector3 direction = transform.position - target.transform.position;
            float angleY = Vector3.Angle(direction, Vector3.up);
            Debug.Log(angleY);
        }
        switch (currentPhase)
        {
            
            case Phase.Free:
                //_HandlePlanetClick();
                _HandleRotation();
                _HandleZoom();
                break;
            case Phase.PlanetLock:
                _HandleRotation();
                _HandleZoom();
                _FollowTarget();
                break;
        }
    }
}
