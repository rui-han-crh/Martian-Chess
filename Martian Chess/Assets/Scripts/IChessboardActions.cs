using UnityEngine;
public interface IChessboardActions {
    public Piece GetPieceAtGridPosition(Vector2Int gridPosition);
    public bool HasPieceAtPosition(Vector2Int gridPosition);
    public void RemovePieceAtPosition(Vector2Int gridPosition, Player player);
    public void MovePiecePosition(Piece piece, Vector2Int newLocation);

    public Piece[] GetAllPieces(Player player);
    public int EvaluateScore();
    public MovementChange GetLastMove();

    public int GetMovesLeft();

    public bool isGameOver();

    public ScoreKeeper GetScoreKeeper();
}
