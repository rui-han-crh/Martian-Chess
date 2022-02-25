using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceManager : MonoBehaviour {
    private static readonly int xSize = 4;
    private static readonly int xSizeOffset = xSize / 2;
    private static readonly int ySize = 8;
    private static readonly int ySizeOffSet = ySize / 2;

    [SerializeField]
    private Tilemap tilemap;


    [SerializeField]
    private GameObject[] pieceGameObjects;
    private Piece[] pieces;
    private (GameObject, Piece)[,] pieceLocations;

    private ScoreKeeper scoreKeeper = new ScoreKeeper();

    private Vector3 offset = new Vector3(0.5f, 0.5f, 0);

    private Vector2Int lastMoveOrigin;
    private Vector2Int lastMoveDestination;

    private void Start() {
        pieceLocations = new (GameObject, Piece)[xSize, ySize];
        pieces = new Piece[pieceGameObjects.Length];
        
    }

    public void UpdatePiecePosition(Piece piece) {
        Vector2Int piecePosition = piece.GetPosition();
        pieceLocations[piecePosition.x + xSizeOffset, piecePosition.y + ySizeOffSet].Item1.transform.position
            = tilemap.CellToWorld((Vector3Int)piecePosition) + offset;
    }

    public Piece GetPieceAtGridPosition(Vector2Int gridPosition) {
        return pieceLocations[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet].Item2;
    }

    private GameObject GetGameObjectAtGridPosition(Vector2Int gridPosition) {
        return pieceLocations[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet].Item1;
    }

    public bool HasPieceAtPosition(Vector2Int gridPosition) {
        return pieceLocations[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet].Item2 != null;
    }

    public void RemovePieceAtPosition(Vector2Int gridPosition, Player player) {
        Piece piece = GetPieceAtGridPosition(gridPosition);
        if (piece != null) {
            piece.Remove();
            GetGameObjectAtGridPosition(gridPosition).SetActive(false);
            scoreKeeper.StoreCapturedPiece(piece, player);
            pieceLocations[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet].Item2 = null;
        }
    }

    //public void MovePiecePosition(Piece piece, Vector2Int newLocation) {
    //    Vector2Int piecePosition = piece.GetPosition();
    //    if (piece.CanMoveToPosition(newLocation)) {
    //        lastMoveOrigin = piecePosition;
    //        lastMoveDestination = newLocation;
    //        (GameObject, Piece) pieceData = pieceLocations[piecePosition.x + xSizeOffset, piecePosition.y + ySizeOffSet];
    //        pieceLocations[piecePosition.x + xSizeOffset, piecePosition.y + ySizeOffSet] = (null, null);
    //        pieceLocations[newLocation.x + xSizeOffset, newLocation.y + ySizeOffSet] = pieceData;
    //        piece.SetPiecePosition(newLocation);
    //        UpdatePiecePosition(piece);
    //        //scoreKeeper.UpdateScore(this);
    //    }
    //}

    public Piece[] GetAllPieces() {
        return pieces.Where(p => p.IsInPlay()).ToArray();
    }

    public Piece[] GetAllPiecesCloned(IChessboardActions boardToCloneTo) {
        return pieces.Where(p => p.IsInPlay()).Select(p => p.Clone(boardToCloneTo)).ToArray();
    }

    public (Vector2Int, Vector2Int) GetLastMove() {
        return (lastMoveOrigin, lastMoveDestination);
    }

    public int EvaluateScore() {
        return scoreKeeper.GetScore();
    }

    public bool isGameOver() {
        return scoreKeeper.GetScore() == int.MaxValue || scoreKeeper.GetScore() == int.MinValue;
    }

}
