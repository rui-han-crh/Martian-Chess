using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Tilemaps;

public class HypotheticalBoard : IChessboardActions {
    private static readonly int xSize = 4;
    private static readonly int xSizeOffset = xSize / 2;
    private static readonly int ySize = 8;
    private static readonly int ySizeOffSet = ySize / 2;
    private static readonly int MAX_MOVES_WITHOUT_CAPTURE = 7;

    private Piece[] pieces;
    private Piece[,] grid;
    private ScoreKeeper scoreKeeper;

    private MovementChange lastMove;

    private int movesUntilCapitulation = MAX_MOVES_WITHOUT_CAPTURE;

    public HypotheticalBoard(GameObject[] pieceGameObjects, Tilemap tileMap)
    {
        List<Piece> pieceList = new List<Piece>();

        for (int i = 0; i < pieceGameObjects.Length; i++)
        {
            GameObject pieceObject = pieceGameObjects[i];
            Piece currentPiece;

            if (!pieceObject.activeInHierarchy)
            {
                continue;
            }

            GameObject pieceGameObject = pieceGameObjects[i];
            Vector2Int piecePosition = (Vector2Int)tileMap.WorldToCell(pieceGameObject.transform.position);
            if (pieceGameObject.name.StartsWith("Paw"))
            {
                currentPiece = new PawnPiece(piecePosition, this);
            }
            else if (pieceGameObject.name.StartsWith("Dro"))
            {
                currentPiece = new DronePiece(piecePosition, this);
            }
            else if (pieceGameObject.name.StartsWith("Que"))
            {
                currentPiece = new QueenPiece(piecePosition, this);
            }
            else
            {
                throw new Exception($"Piece not defined {pieceObject.name}");
            }
            pieceList.Add(currentPiece);
        }

        this.pieces = pieceList.ToArray();
        this.grid = new Piece[xSize, ySize];
        foreach (Piece piece in pieces)
        {
            Vector2Int pieceLocation = piece.GetPosition();
            grid[pieceLocation.x + xSizeOffset, pieceLocation.y + ySizeOffSet] = piece;
        }
        this.scoreKeeper = new ScoreKeeper();
        this.lastMove = MovementChange.Empty();
    }

    public HypotheticalBoard(IChessboardActions chessboard)
    {
        this.pieces = chessboard.GetAllPieces(Player.ALL).Select(p => p.Clone(this)).ToArray();
        this.grid = new Piece[xSize, ySize];
        foreach (Piece piece in pieces)
        {
            Vector2Int pieceLocation = piece.GetPosition();
            grid[pieceLocation.x + xSizeOffset, pieceLocation.y + ySizeOffSet] = piece;
        }
        this.scoreKeeper = chessboard.GetScoreKeeper();
        this.lastMove = chessboard.GetLastMove();
        this.movesUntilCapitulation = chessboard.GetMovesLeft();
    }

    public ScoreKeeper GetScoreKeeper() {
        return scoreKeeper.Clone();
    }

    public Piece[] GetAllPieces() {
        return pieces.Where(p => p.IsInPlay()).ToArray();
    }

    public Piece[] GetAllPieces(Player player) {
        return player switch {
            Player.ONE => pieces.Where(p => p.IsInPlay() && p.GetPosition().y < 0).ToArray(),
            Player.TWO => pieces.Where(p => p.IsInPlay() && p.GetPosition().y >= 0).ToArray(),
            _ => pieces.Where(p => p.IsInPlay()).OrderBy(p => p.GetValue()).ToArray()
        };
    }

    public Piece GetPieceAtGridPosition(Vector2Int gridPosition) {
        return grid[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet];
    }


    public bool HasPieceAtPosition(Vector2Int gridPosition) {
        return grid[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet] != null;
    }

    private bool DoesPieceExist(Piece piece) {
        foreach (Piece pieceOfBoard in pieces) {
            if (pieceOfBoard.Equals(piece)) {
                return true;
            }
        }
        return false;
    }

    public void MovePiecePosition(Piece piece, Vector2Int newLocation) {
        if (!DoesPieceExist(piece)) {
            throw new Exception($"Piece {piece} does not exist in this board");
        }

        Vector2Int piecePosition = piece.GetPosition();
        if (piece.CanMoveToPosition(newLocation, lastMove)) {
            lastMove = new MovementChange(piecePosition, newLocation);
            grid[piecePosition.x + xSizeOffset, piecePosition.y + ySizeOffSet] = null;
            if (HasPieceAtPosition(newLocation))
            {
                Piece previousPiece = GetPieceAtGridPosition(newLocation);
                grid[newLocation.x + xSizeOffset, newLocation.y + ySizeOffSet] = GetPromotedPiece(piece, previousPiece);
                Debug.Log("Promoted");
                RebuildPieceArray();
            }
            else
            {
                grid[newLocation.x + xSizeOffset, newLocation.y + ySizeOffSet] = piece;
                piece.SetPiecePosition(newLocation);
            }
            scoreKeeper.UpdateScore(this);
        } else
        {
            throw new Exception($"{piece} cannot be moved to {newLocation}");
        }
        movesUntilCapitulation--;
    }

    private Piece GetPromotedPiece(Piece piece1, Piece piece2)
    {
        if ((piece1 is PawnPiece && piece2 is DronePiece) || (piece1 is DronePiece && piece2 is PawnPiece))
        {
            return new QueenPiece(piece2.GetPosition(), this);
        } else if (piece1 is PawnPiece && piece2 is PawnPiece) {
            return new DronePiece(piece2.GetPosition(), this);
        }
        throw new Exception("No promotion available");
    }

    public void RemovePieceAtPosition(Vector2Int gridPosition, Player player) {
        Piece piece = GetPieceAtGridPosition(gridPosition);
        piece.Remove();
        scoreKeeper.StoreCapturedPiece(piece, player);
        grid[gridPosition.x + xSizeOffset, gridPosition.y + ySizeOffSet] = null;
        movesUntilCapitulation = MAX_MOVES_WITHOUT_CAPTURE;
    }

    private void RebuildPieceArray()
    {
        List<Piece> pieceList = new List<Piece>();
        foreach (Piece piece in grid)
        {
            if (piece != null)
            {
                pieceList.Add(piece);
            }
        }
        pieces = pieceList.ToArray();
    }

    public MovementChange GetLastMove() {
        return lastMove;
    }

    public int GetMovesLeft()
    {
        return movesUntilCapitulation;
    }

    public int EvaluateScore() {
        scoreKeeper.UpdateScore(this);
        if (movesUntilCapitulation == 0)
        {
            return scoreKeeper.GetWinnerCaptureScore();
        }

        return scoreKeeper.GetScore();
    }

    public bool isGameOver() {
        return scoreKeeper.GetScore() == int.MaxValue || scoreKeeper.GetScore() == int.MinValue || movesUntilCapitulation == 0;
    }
}
