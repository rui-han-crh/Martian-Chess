using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public abstract class Piece {

    [SerializeField]
    protected Vector2Int positionOnBoard;

    protected static readonly int LEAST_Y_POSITION = -4;
    protected static readonly int MOST_Y_POSITION = 3;
    protected static readonly int LEAST_X_POSITION = -2;
    protected static readonly int MOST_X_POSITION = 1;

    protected static readonly int PAWN_VALUE = 1;
    protected static readonly int DRONE_VALUE = 3;
    protected static readonly int QUEEN_VALUE = 7;

    private bool inPlay;

    protected readonly IChessboardActions chessboard;

    public Piece(Vector2Int position, IChessboardActions chessboard) {
        this.positionOnBoard = position;
        this.chessboard = chessboard;
        this.inPlay = true;
    }


    public void SetPiecePosition(Vector2Int position) {
        positionOnBoard = position;
    }

    public Vector2Int GetPosition() {
        return positionOnBoard;
    }

    protected bool IsWithinBoard(Vector2Int position) {
        return position.x >= LEAST_X_POSITION && position.x <= MOST_X_POSITION
            && position.y >= LEAST_Y_POSITION && position.y <= MOST_Y_POSITION;
    }

    public bool CanMoveToPosition(Vector2Int position, MovementChange movementChange) {
        return GetPossibleMovementPlaces(movementChange).Contains(position);
    }

    public void Remove() {
        inPlay = false;
    }

    public void Unremove()
    {
        inPlay = true;
    }

    public bool IsInPlay() {
        return inPlay;
    }

    public abstract int GetValue();

    public abstract Vector2Int[] GetPossibleMovementPlaces(MovementChange movementChange);
    public abstract Piece Clone(IChessboardActions chessboard);

    public override string ToString()
    {
        return this.GetType().Name + " " + GetPosition().ToString();
    }
}
