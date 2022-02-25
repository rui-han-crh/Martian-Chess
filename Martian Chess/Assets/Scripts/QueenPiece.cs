using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenPiece : Piece
{
    public QueenPiece(Vector2Int position, IChessboardActions pieceManager) : base(position, pieceManager) { }

    public override Vector2Int[] GetPossibleMovementPlaces(MovementChange lastMove) {
        List<Vector2Int> possibleLocations = new List<Vector2Int>();
        float quarterAngle = Mathf.PI / 4;

        for (int i = 0; i < 8; i++) {
            Vector2Int unitDisplacementVector = new Vector2Int(
                Mathf.RoundToInt(Mathf.Cos(quarterAngle * i)),
                Mathf.RoundToInt(Mathf.Sin(quarterAngle * i))
                );

            int j = 1;
            Vector2Int destination = positionOnBoard + unitDisplacementVector * j++;
            MovementChange undoneMovement = new MovementChange(destination, positionOnBoard);

            while (IsWithinBoard(destination) && !chessboard.HasPieceAtPosition(destination)) {
                if (!undoneMovement.Equals(lastMove)) {
                    possibleLocations.Add(destination);
                }
                destination = positionOnBoard + unitDisplacementVector * j++;
                undoneMovement = new MovementChange(destination, positionOnBoard);
            }

            if (IsWithinBoard(destination)
                && !undoneMovement.Equals(lastMove)
                && !(positionOnBoard.y < 0 ^ destination.y >= 0)) {
                possibleLocations.Add(destination);
            }
        }

        return possibleLocations.ToArray();
    }

    public override int GetValue() {
        return 7;
    }

    public override Piece Clone(IChessboardActions chessboard) {
        return new QueenPiece(positionOnBoard, chessboard);
    }
}
