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
        doubleTapSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.DoubleTap, 
            (GesturesController.Gestures gesture) => {
            if (currentPiece == null)
            { 
                hud.GetComponent<Hud>().ShufflePieces(); 
            }
        });
        freeAreaTapSubscription = GesturesController.subscribeToGesture(GesturesController.Gestures.FreeAreaTap, 
            (GesturesController.Gestures gesture) => {
            if (currentPiece != null)
            {
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
        });
        pieceTapSubscription = GesturesController.subscribeToPlanetClick(
            (GameObject obj) => {
                if (!isInited)
                    return;

                bool isPiece = isOneOfPieces(obj);
                if (isPiece)
                {
                    HandlePieceClicked(obj);
                }
            });
    }

    private void OnDestroy()
    {
        GesturesController.unsubscribeFromGesture(doubleTapSubscription);
        GesturesController.unsubscribeFromGesture(freeAreaTapSubscription);
        GesturesController.unsubscribeFromGesture(pieceTapSubscription);
    }

    void Update()
    {
        if (!isInited)
            return;

        HandleCurrentPiece();
    }

    private uint doubleTapSubscription, freeAreaTapSubscription, pieceTapSubscription;
    
    private bool isInited = false;

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
        hud.tag = "puzzle";
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

    private bool isOneOfPieces(GameObject obj)
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
