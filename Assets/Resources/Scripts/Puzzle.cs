using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public struct Config
    {
        public Material material;

        public GameObject planetOutline;
        public GameObject puzzleFrame;

        public Vector3 pieceFitOnPos;
    }

    public void Init(Config config)
    {
        material = config.material;
        puzzleFrame = config.puzzleFrame;
        planetOutline = config.planetOutline;
        pieceFitOnPos = config.pieceFitOnPos;

        pieces = ExtractPieces(gameObject);
        puzzleFramePieces = ExtractPieces(puzzleFrame);
        fitPieces = new List<GameObject>();

        InitPieces();
        InitHud();

        puzzleFrame.GetComponent<Rotatable>().Permit();
        planetOutline.GetComponent<Rotatable>().Permit();

        isInited = true;
    }

    public bool IsAssembled()
    {
        return (pieces.Count == 0);
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isInited)
            return;

        HandleInput();
        HandleCurrentPiece();
    }

    private static System.TimeSpan mouseClickInterval = System.TimeSpan.FromMilliseconds(250);
    private static System.TimeSpan mouseDoubleClickInterval = System.TimeSpan.FromMilliseconds(300);

    private bool isInited = false;

    private System.DateTime lastMouseDownDateTime;
    private System.DateTime lastMouseUpDateTime;
    private int mouseClickCount = 0;

    private Material material;

    private GameObject hud;

    private List<GameObject> pieces;
    private List<GameObject> fitPieces;
    private List<GameObject> puzzleFramePieces;

    private GameObject currentPiece;

    private GameObject puzzleFrame;
    private GameObject planetOutline;

    private Vector3 pieceFitOnPos;

    private static List<GameObject> ExtractPieces(GameObject puzzle)
    {
        var pieces = new List<GameObject>();

        foreach (Transform piece in puzzle.transform)
        {
            pieces.Add(piece.gameObject);
        }

        return pieces;
    }

    private void InitPieces()
    {
        var pieceZoomablePos = CountPieceZoomablePos();
        var pieceTravelSpeed = CountPieceTravelSpeed();
        var pieceMaxZoomIn = CountPieceMaxZoomIn();

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].AddComponent<Piece>();
            var pieceScript = pieces[i].GetComponent<Piece>();

            Piece.Config config;

            config.material = material;
            config.twin = puzzleFramePieces[i];
            config.centerPos = transform.position;
            config.zoomablePos = pieceZoomablePos;
            config.travelSpeed = pieceTravelSpeed;
            config.fitOnPos = pieceFitOnPos;
            config.maxZoomIn = pieceMaxZoomIn;
            config.maxZoomOut = 0;

            pieceScript.Init(config);
        }
    }

    private void InitHud()
    {
        hud = new GameObject("PuzzleHud", typeof(Hud));
        var hudScript = hud.GetComponent<Hud>();

        Hud.Config config;

        config.pieces = pieces;

        hudScript.Init(config);
    }

    private void RegisterCurrentPieceAsFit()
    {
        pieces.Remove(currentPiece);

        hud.GetComponent<Hud>().RemovePiece(currentPiece);

        fitPieces.Add(currentPiece);

        currentPiece = null;
    }

    private void HandleInput()
    {
#if UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            lastMouseDownDateTime = System.DateTime.Now;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseClickCount++;

            if ((System.DateTime.Now - lastMouseDownDateTime) < mouseClickInterval)
            {
                HandleMouseUp();
            }

            lastMouseUpDateTime = System.DateTime.Now;
        }
        else
        {
            if ((System.DateTime.Now - lastMouseUpDateTime) >= mouseDoubleClickInterval)
            {
                mouseClickCount = 0;
            }
        }
#endif

#if UNITY_ANDROID
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                lastMouseDownDateTime = System.DateTime.Now;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                mouseClickCount++;

                if ((System.DateTime.Now - lastMouseDownDateTime) < mouseClickInterval)
                {
                    HandleMouseUp();
                }

                lastMouseUpDateTime = System.DateTime.Now;
            }
            else
            {
                if ((System.DateTime.Now - lastMouseUpDateTime) >= mouseDoubleClickInterval)
                {
                    mouseClickCount = 0;
                }
            }

            //mouseClickCount = Input.GetTouch(0).tapCount;

            //HandleMouseUp();
        }
#endif
    }

    private void HandleMouseUp()
    {
        var clickedPiece = DetectClickedPiece();

        if (clickedPiece != null)
        {
            HandlePieceClicked(clickedPiece);
        }
        else
        {
            HandlePieceMisclicked();
        }
    }

    private GameObject DetectClickedPiece()
    {
#if UNITY_STANDALONE
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif

#if UNITY_ANDROID
        Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#endif

        GameObject clickedPiece = null;

        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, ray.direction, out hit, Mathf.Infinity);

        //if (gameObject.GetComponent<LineRenderer>() != null)
        //{
        //    Destroy(gameObject.GetComponent<LineRenderer>());
        //}

        //LineRenderer line = gameObject.AddComponent<LineRenderer>();
        //line.material = new Material(Shader.Find("Sprites/Default"));
        //line.positionCount = 2;
        //line.SetPosition(0, Camera.main.transform.position - new Vector3(0, 1, 0));
        //line.SetPosition(1, Camera.main.transform.position + (ray.direction * 10));
        //line.startColor = Color.green;
        //line.startWidth = 0.5f;
        //line.enabled = true;

        //if (hit.transform.name == "Test")
        //{
        //    hit.transform.gameObject.SetActive(false);
        //}

        //if (hit.transform != null)
        //{
        //    pieces[0].SetActive(false);
        //}

        //Debug.DrawRay(Camera.main.transform.position, ray.direction, Color.green, Mathf.Infinity);

        foreach (var piece in pieces)
        {
            //Physics.Raycast(ray, out hit);

            //if (hit.transform.gameObject && piece == hit.transform.gameObject) 
            if (Physics.Raycast(ray, out hit, Mathf.Infinity) && piece.Equals(hit.transform.gameObject))
            {
                clickedPiece = piece;
                //clickedPiece.SetActive(false);

                break;
            }
        }
        return clickedPiece;
    }

    private void HandlePieceClicked(GameObject clickedPiece)
    {
        if (!clickedPiece.GetComponent<Renderer>().enabled
            || (clickedPiece.GetComponent<Piece>().GetCondition() == Piece.Condition.SELECTED)
            || (clickedPiece.GetComponent<Piece>().GetCondition() == Piece.Condition.FIT))
        {
            return;
        }

        if (currentPiece != clickedPiece)
        {
            if (currentPiece != null)
            {
                if (currentPiece.GetComponent<Piece>().IsTravelling())
                    return;

                currentPiece.GetComponent<Piece>().Release();
            }

            if (!clickedPiece.GetComponent<Piece>().Focus())
                return;

            currentPiece = clickedPiece;

            puzzleFrame.GetComponent<Rotatable>().Forbid();
            planetOutline.GetComponent<Rotatable>().Forbid();
        }
        else
        {
            var pieceScript = currentPiece.GetComponent<Piece>();

            if (pieceScript.GetCondition() == Piece.Condition.FOCUSED)
            {
                if (!pieceScript.Select())
                    return;

                puzzleFrame.GetComponent<Rotatable>().Permit();
                planetOutline.GetComponent<Rotatable>().Permit();
            }
        }
    }

    private void HandlePieceMisclicked()
    {
        if (currentPiece == null)
        {
            if (mouseClickCount == 2)
            {
                hud.GetComponent<Hud>().ShufflePieces();
            }
            return;
        }

        var pieceScript = currentPiece.GetComponent<Piece>();

        switch (pieceScript.GetCondition())
        {
            case Piece.Condition.FOCUSED:
                if (!pieceScript.Release())
                    return;

                currentPiece = null;

                puzzleFrame.GetComponent<Rotatable>().Permit();
                planetOutline.GetComponent<Rotatable>().Permit();

                break;

            case Piece.Condition.SELECTED:
                if (!pieceScript.Focus())
                    return;

                puzzleFrame.GetComponent<Rotatable>().Forbid();
                planetOutline.GetComponent<Rotatable>().Forbid();

                break;

            default:
                break;
        }
    }

    private void HandleCurrentPiece()
    {
        if (currentPiece == null)
            return;

        var pieceScript = currentPiece.GetComponent<Piece>();

        if (pieceScript.GetCondition() == Piece.Condition.SELECTED)
        {
            if (pieceScript.GetComponent<Piece>().TryFit())
            {
                RegisterCurrentPieceAsFit();
            }
        }
    }

    // workaround
    private Vector3 CountPieceZoomablePos()
    {
        var distance = Vector3.Distance(pieceFitOnPos, Camera.main.transform.position);
        Vector3 pieceZoomablePos;

        switch (pieces.Count)
        {
            // the greater a piece count, the less a piece, the less its distance to camera
            case 6:
            default:
                pieceZoomablePos = new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 5));
                break;
            case 24:
                pieceZoomablePos = new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 3));
                break;
            case 96:
                pieceZoomablePos = new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 2));
                break;
        }

        return pieceZoomablePos;
    }

    // workaround
    private float CountPieceTravelSpeed()
    {
        switch (pieces.Count)
        {
            // the greater a piece count, the less a piece, the less its distance to outline center, the less its travel speed
            case 6:
            default:
                return 60.0f;
            case 24:
                return 30.0f;
            case 96:
                return 15.0f;
        }
    }

    // workaround
    private float CountPieceMaxZoomIn()
    {
        switch (pieces.Count)
        {
            // the greater a piece count, the less a piece, the less its distance to camera, the greater its max zoom-in value
            case 6:
            default:
                return 0.0f;
            case 24:
                return 0.9f;
            case 96:
                return 1.2f;
        }
    }
}
