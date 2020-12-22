using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum Error
    {
        OK,
        CANNOT_LOAD_RESOURCE,
    }

    public enum Condition
    {
        RELEASED,
        FOCUSED,
        SELECTED,
        FIT,
    }

    public struct Config
    {
        public string materialResourceName;

        public GameObject twin;

        public Vector3 centerPos;
        public Vector3 zoomablePos;
        public Vector3 fitOnPos;

        public float travelSpeed;

        public float maxZoomIn;
        public float maxZoomOut;
    }

    public Error Init(Config config)
    {
        materialResourceName = config.materialResourceName;
        twin = config.twin;
        centerPos = config.centerPos;
        zoomablePos = config.zoomablePos;
        fitOnPos = config.fitOnPos;
        travelSpeed = config.travelSpeed;
        maxZoomIn = config.maxZoomIn;
        maxZoomOut = config.maxZoomOut;

        Error error;

        error = SetMaterial();
        if (Error.OK != error)
            return error;

        SetCollider();
        SetZoomable();
        SetRotatable();
        FaceToCamera();
        Hide();
        Hide(twin);

        defaultRotation = transform.rotation;
        travelTargetPos = transform.position;

        SetCondition(Condition.RELEASED);

        isInited = true;

        return Error.OK;
    }

    public Condition GetCondition()
    {
        return condition;
    }

    public void MoveToStand(Vector3 standPos)
    {
        this.standPos = standPos;

        TravelToPos(standPos);

        Show();
    }

    public void Hide()
    {
        Hide(gameObject);

        TravelToPos(new Vector3(centerPos.x, centerPos.y + 10, centerPos.z));
    }

    public void Show()
    {
        Show(gameObject);
    }

    public bool Release()
    {
        if (isTravelling || isStraightening)
            return false;

        SetCondition(Condition.RELEASED);

        GetComponent<Zoomable>().Forbid();
        GetComponent<Rotatable>().Forbid();

        TravelToPos(standPos);

        return true;
    }

    public bool Focus()
    {
        if (isTravelling || isStraightening)
            return false;

        SetCondition(Condition.FOCUSED);

        GetComponent<Zoomable>().Permit();
        GetComponent<Rotatable>().Permit();

        TravelToPos(zoomablePos);

        return true;
    }

    public bool Select()
    {
        if (isTravelling || isStraightening)
            return false;

        SetCondition(Condition.SELECTED);

        GetComponent<Zoomable>().Forbid();
        GetComponent<Rotatable>().Forbid();

        TravelToPos(fitOnPos);

        return true;
    }

    public bool TryFit()
    {
        if (Condition.FIT == condition)
            return true;

        if (IsFitToTwin())
        {
            Fit();

            return true;
        }

        return false;
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isInited)
            return;

        if (isTravelling)
        {
            transform.position = Vector3.MoveTowards(transform.position, travelTargetPos, Time.deltaTime * travelSpeed);

            if (transform.position == travelTargetPos)
            {
                isTravelling = false;
            }
        }

        if (isStraightening)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, defaultRotation, Time.deltaTime * straightenSpeed);

            if (transform.rotation == defaultRotation)
            {
                isStraightening = false;
            }
        }

        HandleCondition();
    }

    private const float straightenSpeed = 1000.0f;
    private const float maxFitDistanceFault = 0.2f;
    private const float maxFitAngleFault = 10.0f;

    private bool isInited = false;
    private bool isTravelling = false;
    private bool isStraightening = false;

    private string materialResourceName;

    private GameObject twin;

    private Condition condition;

    private Vector3 centerPos;
    private Vector3 standPos;
    private Vector3 zoomablePos;
    private Vector3 fitOnPos;
    private Vector3 travelTargetPos;

    private float travelSpeed;

    private float maxZoomIn;
    private float maxZoomOut;

    private Quaternion defaultRotation;

    private Error SetMaterial()
    {
        var material = Resources.Load(materialResourceName) as Material;

        if (null == material)
            return Error.CANNOT_LOAD_RESOURCE;

        GetComponent<Renderer>().material = material;

        return Error.OK;
    }

    private void SetCollider()
    {
        gameObject.AddComponent<MeshCollider>();
    }

    private void SetZoomable()
    {
        gameObject.AddComponent<Zoomable>();
        Zoomable zoomableScript = GetComponent<Zoomable>();

        Zoomable.Config config;

        config.startPos = zoomablePos;
        config.maxZoomIn = maxZoomIn;
        config.maxZoomOut = maxZoomOut;

        zoomableScript.Init(config);
    }

    private void SetRotatable()
    {
        gameObject.AddComponent<Rotatable>();
    }

    public void TravelToPos(Vector3 pos)
    {
        travelTargetPos = pos;
        isTravelling = true;
    }

    public void Straighten()
    {
        isStraightening = true;
    }

    private void Fit()
    {
        SetCondition(Condition.FIT);
    }

    private bool IsFitToTwin()
    {
        if (Condition.SELECTED != condition)
            return false;

        if ((Vector3.Distance(transform.position, twin.transform.position) > maxFitDistanceFault)
            || (Vector3.Angle(transform.forward, twin.transform.forward) > maxFitAngleFault))
        {
            return false;
        }

        return true;
    }

    private void HandleCondition()
    {
        if (Condition.FIT == condition)
        {
            RepeatAfterTwin();
        }
    }

    private void RepeatAfterTwin()
    {
        transform.position = twin.transform.position;
        transform.rotation = twin.transform.rotation;
    }

    private void Show(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = true;
    }

    private void Hide(GameObject obj)
    {
        obj.GetComponent<Renderer>().enabled = false;
    }

    private void SetCondition(Condition condition)
    {
        this.condition = condition;

        Straighten();
    }

    // workaround
    private void FaceToCamera()
    {
        var lastDistanceToFitOnPos = Vector3.Distance(fitOnPos, transform.position);
        var rotationDir = 1;
        var isDirChanged = false;

        while (true)
        {
            transform.RotateAround(centerPos, Vector3.up, rotationDir * 1);

            var currentDistanceToFitOnPos = Vector3.Distance(fitOnPos, transform.position);

            if (currentDistanceToFitOnPos < lastDistanceToFitOnPos)
                lastDistanceToFitOnPos = currentDistanceToFitOnPos;
            else
            {
                transform.RotateAround(centerPos, Vector3.down, rotationDir * 1);

                rotationDir *= -1;

                if (isDirChanged)
                    break;
                else
                    isDirChanged = true;
            }
        }

        isDirChanged = false;

        while (true)
        {
            transform.RotateAround(centerPos, Vector3.forward, rotationDir * 1);

            var currentDistanceToFitOnPos = Vector3.Distance(fitOnPos, transform.position);

            if (currentDistanceToFitOnPos < lastDistanceToFitOnPos)
                lastDistanceToFitOnPos = currentDistanceToFitOnPos;
            else
            {
                transform.RotateAround(centerPos, Vector3.back, rotationDir * 1);

                rotationDir *= -1;

                if (isDirChanged)
                    break;
                else
                    isDirChanged = true;
            }
        }

        isDirChanged = false;

        while (true)
        {
            transform.RotateAround(centerPos, Vector3.left, rotationDir * 1);

            var currentDistanceToFitOnPos = Vector3.Distance(fitOnPos, transform.position);

            if (currentDistanceToFitOnPos < lastDistanceToFitOnPos)
                lastDistanceToFitOnPos = currentDistanceToFitOnPos;
            else
            {
                transform.RotateAround(centerPos, Vector3.right, rotationDir * 1);

                rotationDir *= -1;

                if (isDirChanged)
                    break;
                else
                    isDirChanged = true;
            }
        }

        transform.Rotate(0, 0, Random.Range(0, 179), Space.World);
    }
}
