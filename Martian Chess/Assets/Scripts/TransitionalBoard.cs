using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionalBoard : IComparer<TransitionalBoard>
{
    private readonly IChessboardActions pastBoard;
    private readonly IChessboardActions nextBoard;
    private int changeValue;
    public TransitionalBoard(IChessboardActions pastBoard, IChessboardActions nextBoard) {
        this.pastBoard = pastBoard;
        this.nextBoard = nextBoard;
        ComputeChangeValue();
    }

    private void ComputeChangeValue() {
        MovementChange lastMove = nextBoard.GetLastMove();
        // It's good if a piece of low value captures a piece of high value
        // also if a piece is moving from the back row forward
        changeValue = 0;

        Piece pieceMoved = pastBoard.GetPieceAtGridPosition(lastMove.getOrigin());
        Piece pieceCaptured = pastBoard.GetPieceAtGridPosition(lastMove.getDestination());
        if (pieceCaptured != null && !(lastMove.getOrigin().y < 0 ^ lastMove.getDestination().y >= 0))
        {
            changeValue += pieceCaptured.GetValue() - pieceMoved.GetValue();
        } else if (pieceCaptured != null && lastMove.getOrigin().y < 0 ^ lastMove.getDestination().y >= 0)
        {
            changeValue += 9;
        }

        changeValue += Mathf.Abs(lastMove.getOrigin().y) - Mathf.Abs(lastMove.getDestination().y) + 1;

    }

    public IChessboardActions GetBoard()
    {
        return nextBoard;
    }

    public Piece GetPieceMoved()
    {
        return pastBoard.GetPieceAtGridPosition(nextBoard.GetLastMove().getOrigin());
    }

    public int GetChangeValue() {
        return changeValue;
    }

    public int Compare(TransitionalBoard x, TransitionalBoard y) {
        return y.changeValue - x.changeValue;
    }
}
