using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public enum Condition
    {
        RELEASED,
        FOCUSED,
        SELECTED,
        FIT,
    }

    public struct Config
    {
        public Material material;

        public GameObject twin;

        public Vector3 centerPos;
        public Vector3 zoomablePos;
        public Vector3 fitOnPos;

        public float travelSpeed;

        public float maxZoomIn;
        public float maxZoomOut;
    }

    public void Init(Config config)
    {
        material = config.material;
        twin = config.twin;
        centerPos = config.centerPos;
        zoomablePos = config.zoomablePos;
        fitOnPos = config.fitOnPos;
        travelSpeed = config.travelSpeed;
        maxZoomIn = config.maxZoomIn;
        maxZoomOut = config.maxZoomOut;

        SetMaterial();
        SetCollider();
        SetZoomable();
        SetRotatable();
        FaceToCamera();
        Hide();
        Hide(twin);

        defaultRotation = transform.rotation;
        travelTargetPos = transform.position;

        SetCondition(Condition.RELEASED);

        tag = "piece";

        isInited = true;
    }

    public Condition GetCondition()
    {
        return condition;
    }

    public bool IsTravelling()
    {
        return isTravelling;
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

        //GetComponent<Zoomable>().Permit();
        GetComponent<Rotatable>().Permit(true);

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
        if (condition == Condition.FIT)
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
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, defaultRotation, Time.deltaTime * straightenSpeed);

            //if (transform.rotation == defaultRotation)
            //{
                isStraightening = false;
            //}
        }

        HandleCondition();
    }

    private const float straightenSpeed = 1000.0f;
    private const float maxFitDistanceFault = 0.25f;
    private const float maxFitAngleFault = 15.0f;

    private bool isInited = false;
    private bool isTravelling = false;
    private bool isStraightening = false;

    private Material material;

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

    private void SetMaterial()
    {
        GetComponent<Renderer>().material = material;
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
        twin.gameObject.GetComponent<Renderer>().enabled = false;
        SetCondition(Condition.FIT);
    }

    private bool IsFitToTwin()
    {
        if (condition != Condition.SELECTED)
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
        if (condition == Condition.FIT)
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

    public void MakeTwinMarkedForTutorial()
    {
        twin.gameObject.GetComponent<Renderer>().enabled = true;
        var color = Color.green;
        color.a = 0.0f;
        twin.gameObject.GetComponent<Renderer>().material.color = color;
    }

    // workaround
    private void FaceToCamera()
    {
        transform.position = twin.transform.position;
        transform.rotation = twin.transform.rotation;
        var dirFromCentertoPiece = transform.position - centerPos;

        while (true)
        {
            var firFromCenterToCamera = Camera.main.transform.position - centerPos;
            transform.RotateAround(centerPos, Camera.main.transform.up, Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.up));

            dirFromCentertoPiece = transform.position - centerPos;
            transform.RotateAround(centerPos, Camera.main.transform.right, Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.right));

            dirFromCentertoPiece = transform.position - centerPos;            
            if (Mathf.Abs(Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.right)) < 0.3 && Mathf.Abs(Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.up)) < 0.3)
            {
                Debug.Log("ANGLE Y:" + Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.right));
                Debug.Log("ANGLE X:" + Vector3.SignedAngle(dirFromCentertoPiece, firFromCenterToCamera, Camera.main.transform.up));
                break;
            }
        }
    }
}
