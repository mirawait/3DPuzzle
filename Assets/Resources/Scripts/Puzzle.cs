using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    public struct Config
    {
        public Material material;
        public Material transparentMaterial;

        public GameObject planetOutline;
        public GameObject puzzleFrame;


        public Vector3 pieceFitOnPos;
    }

    private int doubleTapSubscription, tapSubscription;
    private bool isInited = false;
    private Material material;
    private Material transparentMaterial;
    private GameObject hud;
    private List<GameObject> pieces;
    private List<GameObject> fitPieces;
    private List<GameObject> puzzleFramePieces;
    private GameObject currentPiece;
    private GameObject puzzleFrame;
    private GameObject planetOutline;
    private Vector3 pieceFitOnPos;
    private TapController tapController;

    void Start()
    {
        tapController = GameObject.Find("Controller").GetComponent<TapController>();
        doubleTapSubscription = tapController.SubscribeToTap(TapController.Tap.DoubleTap, _DoubleTapHandler);
        tapSubscription = tapController.SubscribeToTap(TapController.Tap.Tap, _TapHandler);
    }

    private void OnDestroy()
    {
        tapController.UnsubscribeFromTap(doubleTapSubscription);
        tapController.UnsubscribeFromTap(tapSubscription);
    }

    void Update()
    {
        if (!isInited)
            return;

        _HandleCurrentPiece();
    }
    public void Init(Config config)
    {
        material = config.material;
        transparentMaterial = config.transparentMaterial;
        puzzleFrame = config.puzzleFrame;
        planetOutline = config.planetOutline;
        pieceFitOnPos = config.pieceFitOnPos;

        pieces = _ExtractPieces(gameObject);
        puzzleFramePieces = _ExtractPieces(puzzleFrame);
        fitPieces = new List<GameObject>();

        _InitPieces();
        _InitHud();

        puzzleFrame.GetComponent<Rotatable>().Permit();
        planetOutline.GetComponent<Rotatable>().Permit();

        isInited = true;
    }

    public bool IsAssembled()
    {
        return (pieces.Count == 0);
    }

    private void _TapHandler(GameObject target)
    {
        Debug.Log("TAP CONTROLLER START");
        if (isInited && _IsOneOfPieces(target))
        {
            Debug.Log("_HandlePieceClicked(target);");
            _HandlePieceClicked(target);
        }
        else if (target != puzzleFrame && currentPiece != null)
        {
            var pieceScript = currentPiece.GetComponent<Piece>();
            Debug.Log("else if (target != puzzleFrame && currentPiece != null)");
            Debug.Log(pieceScript.GetCondition());
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
    }

    private void _DoubleTapHandler(GameObject target)
    {
        if (!_IsOneOfPieces(target) && target != puzzleFrame && currentPiece == null)
        {
            hud.GetComponent<Hud>().ShufflePieces();
        }
    }

    private static List<GameObject> _ExtractPieces(GameObject puzzle)
    {
        var pieces = new List<GameObject>();

        foreach (Transform piece in puzzle.transform)
        {
            pieces.Add(piece.gameObject);
        }

        return pieces;
    }

    private void _InitPieces()
    {
        var pieceZoomablePos = _CountPieceZoomablePos();
        var pieceTravelSpeed = _CountPieceTravelSpeed();
        var pieceMaxZoomIn = _CountPieceMaxZoomIn();

        for (int i = 0; i < pieces.Count; i++)
        {
            pieces[i].AddComponent<Piece>();
            var pieceScript = pieces[i].GetComponent<Piece>();

            Piece.Config config;

            config.material = material;
            config.transparentMaterial = transparentMaterial;
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

    private void _InitHud()
    {
        hud = new GameObject("PuzzleHud", typeof(Hud));
        hud.tag = "puzzle";
        var hudScript = hud.GetComponent<Hud>();

        Hud.Config config;

        config.pieces = pieces;

        hudScript.Init(config);
    }

    private void _RegisterCurrentPieceAsFit()
    {
        pieces.Remove(currentPiece);

        hud.GetComponent<Hud>().RemovePiece(currentPiece);

        fitPieces.Add(currentPiece);

        currentPiece = null;
    }

    private bool _IsOneOfPieces(GameObject obj)
    {
        foreach (var piece in pieces)
        {
            if (piece.Equals(obj))
            {
                return true;
            }
        }
        return false;
    }

    private void _HandlePieceClicked(GameObject clickedPiece)
    {
        Debug.Log("HANDE PIECE CLICKED START");
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

    private void _HandleCurrentPiece()
    {
        if (currentPiece == null)
            return;
        Debug.Log("_HandleCurrentPiece START");
        var pieceScript = currentPiece.GetComponent<Piece>();

        if (pieceScript.GetCondition() == Piece.Condition.SELECTED)
        {
            if (pieceScript.GetComponent<Piece>().TryFit())
            {
                _RegisterCurrentPieceAsFit();
            }
        }
    }

    // workaround
    private Vector3 _CountPieceZoomablePos()
    {
        var distance = Vector3.Distance(pieceFitOnPos, Camera.main.transform.position);
        Vector3 pieceZoomablePos;

        var dir = Camera.main.transform.forward * (-1);

        switch (pieces.Count)
        {
            // the greater a piece count, the less a piece, the less its distance to camera
            case 6:
            default:
                pieceZoomablePos = pieceFitOnPos + dir * (distance / 5);// new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 5));
                break;
            case 24:
                pieceZoomablePos = pieceFitOnPos + dir * (distance / 3);//new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 3));
                break;
            case 96:
                pieceZoomablePos = pieceFitOnPos + dir * (distance / 2);//new Vector3(pieceFitOnPos.x, pieceFitOnPos.y, pieceFitOnPos.z - (distance / 2));
                break;
        }

        return pieceZoomablePos;
    }

    // workaround
    private float _CountPieceTravelSpeed()
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
    private float _CountPieceMaxZoomIn()
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
