using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hud : MonoBehaviour
{
    public struct Config
    {
        public List<GameObject> pieces;
    }

    public void Init(Config config)
    {
        pieces = config.pieces;

        piecesAtStand = new Dictionary<GameObject, Vector3>();

        InitPiecesStandPos();
        ShufflePieces();

        isInited = true;
    }

    public void ShufflePieces()
    {
        foreach (var piece in piecesAtStand)
        {
            piece.Key.GetComponent<Piece>().Hide();
        }

        var n = pieces.Count;
        var rand = new System.Random();

        while (n > 1)
        {
            n--;

            var k = rand.Next(n + 1);

            var value = pieces[k];
            pieces[k] = pieces[n];
            pieces[n] = value;
        }

        ArrangePieces();
    }

    public void RemovePiece(GameObject removedPiece)
    {
        var standPos = piecesAtStand[removedPiece];

        piecesAtStand.Remove(removedPiece);
        pieces.Remove(removedPiece);

        foreach (var piece in pieces)
        {
            if (!piecesAtStand.ContainsKey(piece))
            {
                piecesAtStand.Add(piece, standPos);

                piece.GetComponent<Piece>().MoveToStand(standPos);

                break;
            }
        }
    }

    void Start()
    {
    }

    void Update()
    {
        if (!isInited)
            return;
    }

    private bool isInited = false;

    private List<GameObject> pieces;
    private List<Vector3> piecesStandPos;
    private Dictionary<GameObject, Vector3> piecesAtStand;

    private void ArrangePieces()
    {
        piecesAtStand = new Dictionary<GameObject, Vector3>();

        int interations = (piecesStandPos.Count < pieces.Count) ? piecesStandPos.Count : pieces.Count;

        for (int i = 0; i < interations; i++)
        {
            pieces[i].GetComponent<Piece>().MoveToStand(piecesStandPos[i]);

            piecesAtStand.Add(pieces[i], piecesStandPos[i]);

            pieces[i].GetComponent<Piece>().Show();
        }
    }

    // workaround
    private void InitPiecesStandPos()
    {
        var pieceDistanceToCamera = CountPieceDistanceToCamera();

        // 6 'cells' for currently selectable pieces
        piecesStandPos = new List<Vector3>(new Vector3[]{
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 6, 1 * (Screen.height / 3), pieceDistanceToCamera)),
            Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 6, 2 * (Screen.height / 3), pieceDistanceToCamera)),

            Camera.main.ScreenToWorldPoint(new Vector3(5 * (Screen.width / 6), 1 * (Screen.height / 3), pieceDistanceToCamera)),
            Camera.main.ScreenToWorldPoint(new Vector3(5 * (Screen.width / 6), 2 * (Screen.height / 3), pieceDistanceToCamera)),

            Camera.main.ScreenToWorldPoint(new Vector3(3 * (Screen.width / 8), Screen.height / 7, pieceDistanceToCamera)),
            Camera.main.ScreenToWorldPoint(new Vector3(5 * (Screen.width / 8), Screen.height / 7, pieceDistanceToCamera)),
        });
    }

    // workaround
    private float CountPieceDistanceToCamera()
    {
        // the greater a piece count, the less a piece, the less its distance to camera
        switch (pieces.Count)
        {
            case 6:
            default:
                return 10.0f;
            case 24:
                return 6.0f;
            case 96:
                return 3.5f;
        }
    }
}
