using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEditor;
using UnityEditorInternal;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float rotationSpeed = 1f,
                 zoomSpeed = 0.5f,
                 minSolarZoom = 0.1f,
                 maxSolarZoom = 179.9f,
                 minPlanetZoom = 0.1f,
                 maxPlanetZoom = 90f;
    [SerializeField]
    private GameObject camStartPos;
    [SerializeField]
    private GameObject camEndPos;

    private enum Phase
    {
        Menu,
        Free,
        GoingMenu,
        GoingFree
    }
    private Phase currentPhase = Phase.Menu;
    private float currentMaxZoom, 
                  currentMinZoom;
    private float CameraYLimitUp = 180f,
                  CameraYLimitDown = -5f;
    //private float CameraXLimitRight = 21;
    //private float CameraXLimitLeft = -21;
    private GameObject sun,
                       target;
    private Camera camera;
    private Vector3 defaultPosition;
    private Quaternion defaultQuaternion;

    
    // Start is called before the first frame update
    void Start()
    {
        sun = GameObject.Find("Sun");
        camera = GetComponent<Camera>();
        defaultPosition = transform.position;
        defaultQuaternion = transform.rotation;
    }
    public void GoFree()
    {
        currentPhase = Phase.GoingFree;
        currentMaxZoom = maxSolarZoom;
        currentMinZoom = minSolarZoom;
        target = sun;

        Debug.Log("Going free");
        StartCoroutine(_LockOnTargetTransition(maxSolarZoom, new Vector3(0,40,0), Phase.Free));

    }
    IEnumerator _LockOnTargetTransition(float targetDistance, Vector3 targetAngle, Phase targetPhase)
    {
        while (true)
        {
            Debug.Log("_LockOnTargetTransition WHILE");

            Vector3 targetDir = (transform.position - target.transform.position);
            targetDir.Normalize();

            float distanceFromLockTarget = Vector3.Distance(transform.position, target.transform.position),
                  diffTargetDistance = targetDistance - distanceFromLockTarget;
            float angleY = Vector3.Angle(targetDir, Vector3.up),
                  diffAngle = angleY - targetAngle.y;
            Debug.Log("diffAngle" + diffAngle);
            Debug.Log("diffDistance" + diffTargetDistance);
            Vector3 targetPos = transform.position + targetDir * diffTargetDistance 
                + new Vector3(0, 1, 0) * rotationSpeed * diffAngle * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed * 10 * Time.deltaTime);

            Quaternion targetRot = Quaternion.LookRotation(-targetDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

            if (targetDistance == distanceFromLockTarget && transform.rotation == targetRot)
            {
                currentPhase = targetPhase;
                Debug.Log("_LockOnTargetTransition end");
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
        Debug.Log(direction+"==="+newDirection);
        if (newDistance > currentMinZoom && newDistance < currentMaxZoom && direction==newDirection) 
            transform.position = newPos;
        //if(transform.position.y < 20f)
        //{
        //    transform.position = new Vector3(transform.position.x, 20f, transform.position.z);
        //}
    }
    void _HandleRotation()
    {
        if (Input.touchCount != 1)
            return;

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
            if (deltaPos.y < 0 && transform.rotation.x < CameraYLimitUp)
            {
                moveDirection = Vector3.up;
            }
            else if (deltaPos.y > 0 && transform.rotation.x > CameraYLimitDown)
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
        //Vector3 targetDir = transform.position - sun.transform.position;
        //Quaternion targetRot = Quaternion.LookRotation(-
        //       targetDir);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        //
        //Debug.Log(angle);
        switch (currentPhase)
        {
            case Phase.GoingFree:
                //_GoFree();
                break;
            case Phase.Free:
                _HandleRotation();
                _HandleZoom();
                break;
        }
    }
}
