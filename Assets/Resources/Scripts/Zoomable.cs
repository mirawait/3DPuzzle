using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoomable : MonoBehaviour
{
    public struct Config
    {
        public Vector3 startPos;
        public float maxZoomIn;
        public float maxZoomOut;
    }

    public void Init(Config config)
    {
        startPos = config.startPos;
        maxZoomInCord = startPos.z - config.maxZoomIn;
        maxZoomOutCord = startPos.z + config.maxZoomOut;

        isInited = true;
    }

    public void Permit()
    {
        isPermited = true;
    }

    public void Forbid()
    {
        isPermited = false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isInited)
            return;

        HandleInput();
    }

    private const float zoomSpeed = 0.01f;

    private bool isInited = false;
    private bool isPermited = false;

    private Vector3 startPos;
    private float maxZoomInCord;
    private float maxZoomOutCord;

    void HandleInput()
    {
        if (isPermited)
        {
        #if UNITY_STANDALONE
                    int zoomDir = 0;

                    if (Input.GetAxis("Mouse ScrollWheel") > 0)
                        zoomDir = -1;
                    else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                        zoomDir = 1;

                    Zoom(zoomDir);
        #endif

        #if UNITY_ANDROID
                    if (Input.touchCount == 2)
                    {
                        Touch touchZero = Input.GetTouch(0);
                        Touch touchOne = Input.GetTouch(1);

                        Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
                        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

                        float prevTouchDeltaMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
                        float touchDeltaMagnitude = (touchZero.position - touchOne.position).magnitude;
                        float zoomDir = prevTouchDeltaMagnitude - touchDeltaMagnitude;

                        Zoom((int) zoomDir);
                    }
        #endif
        }
    }

    void Zoom(int dir)
    {
        var z = transform.position.z;

        z += dir * zoomSpeed;
        z = Mathf.Clamp(z, maxZoomInCord, maxZoomOutCord);

        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
