using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnPiece : Piece
{
    public PawnPiece(Vector2Int position, IChessboardActions pieceManager) : base(position, pieceManager) { }

    public override Vector2Int[] GetPossibleMovementPlaces(MovementChange lastMove) {
        List<Vector2Int> possibleLocations = new List<Vector2Int>();
        float quarterAngle = Mathf.PI / 4;
        float halfAngle = Mathf.PI / 2;

        for (int i = 0; i < 4; i++) {
            Vector2Int unitDisplacementVector = new Vector2Int(
                Mathf.RoundToInt(Mathf.Cos(quarterAngle + halfAngle * i)),
                Mathf.RoundToInt(Mathf.Sin(quarterAngle + halfAngle * i))
                );

            Vector2Int destination = positionOnBoard + unitDisplacementVector;
            MovementChange undoneMovement = new MovementChange(destination, positionOnBoard);


            if (IsWithinBoard(destination)
                && !undoneMovement.Equals(lastMove)
                && (!chessboard.HasPieceAtPosition(destination) 
                    || !(positionOnBoard.y < 0 ^ destination.y >= 0))) {
                possibleLocations.Add(destination);
            }
        }

        return possibleLocations.ToArray();
    }

    public override int GetValue() {
        return 1;
    }

    public override Piece Clone(IChessboardActions chessboard) {
        return new PawnPiece(positionOnBoard, chessboard);
    }
}
