using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float rotationSpeed = 1f,
                 zoomSpeed = 0.5f,
                 minZoom = 0.1f,
                 maxZoom = 179.9f;

    enum Phase
    {
        Menu,
        Free,
        GoingMenu,
        GoingFree
    }

    private Phase currentPhase = Phase.Menu;
    private float CameraYLimitUp = 18;
    private float CameraYLimitDown = -5f;
    //private float CameraXLimitRight = 21;
    //private float CameraXLimitLeft = -21;
    private GameObject target;
    private Camera camera;
    private Vector3 defaultPosition;
    private Quaternion defaultQuaternion;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Sun");
        camera = GetComponent<Camera>();
        defaultPosition = transform.position;
        defaultQuaternion = transform.rotation;
    }

    public void GoFree()
    {
        currentPhase = Phase.GoingFree;
    }
    void _GoFree()
    {
        Vector3 direction = transform.position - target.transform.position;
        direction.Normalize();
        
        Vector3 targetPos = direction * maxZoom;
        targetPos.y = 15;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, zoomSpeed);

        Quaternion targetRot = Quaternion.LookRotation(-direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed);
        if (transform.position == targetPos && transform.rotation == targetRot)
        {
            currentPhase = Phase.Free;
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
        float newDistance = Vector3.Distance(target.transform.position, newPos);
        if (newDistance > minZoom && newDistance < maxZoom)
            transform.position = newPos;
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
            if (deltaPos.y < 0 && transform.position.y < CameraYLimitUp)
            {
                moveDirection = Vector3.up;
            }
            else if (deltaPos.y > 0 && transform.position.y > CameraYLimitDown)
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
        switch(currentPhase)
        {
            case Phase.GoingFree:
                _GoFree();
                break;
            case Phase.Free:
                _HandleRotation();
                _HandleZoom();
                break;
        }
    }
}
