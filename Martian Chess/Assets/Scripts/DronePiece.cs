using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DronePiece : Piece
{
    public DronePiece(Vector2Int position, IChessboardActions pieceManager) : base(position, pieceManager) { }
    public override Vector2Int[] GetPossibleMovementPlaces(MovementChange lastMove) {
        List<Vector2Int> possibleLocations = new List<Vector2Int>();
        float halfAngle = Mathf.PI / 2;
        for (int i = 0; i < 4; i++) {
            Vector2Int unitDisplacementVector = new Vector2Int(
                Mathf.RoundToInt(Mathf.Cos(halfAngle * i)),
                Mathf.RoundToInt(Mathf.Sin(halfAngle * i))
                );

            Vector2Int destination = positionOnBoard + unitDisplacementVector;
            MovementChange undoneMovement = new MovementChange(destination, positionOnBoard);

            for (int j = 1; j <= 2 && SatisfyConditionsToContinue(destination); j++)
            {
                if (!undoneMovement.Equals(lastMove))
                {
                    possibleLocations.Add(destination);
                }
                destination = positionOnBoard + unitDisplacementVector * (j + 1);
                undoneMovement = new MovementChange(destination, positionOnBoard);
            }

            if (IsWithinBoard(destination) && isPromotionCriteriaMet(destination))
            {
                possibleLocations.Add(destination);
            }
        }
        return possibleLocations.ToArray();
    }

    private bool SatisfyConditionsToContinue(Vector2Int destination)
    {
        return IsWithinBoard(destination) 
            && (!chessboard.HasPieceAtPosition(destination)
                || !(positionOnBoard.y < 0 ^ destination.y >= 0));
    }

    private bool isPromotionCriteriaMet(Vector2Int destination)
    {
        Player player = positionOnBoard.y >= 0 ? Player.TWO : Player.ONE;
        int numberOfThisPieces = chessboard.GetAllPieces(player)
                                        .Where(p => p.GetValue() == 7)
                                        .Count();
        int allNumberOfThisPieces = chessboard.GetAllPieces(Player.ALL)
                                        .Where(p => p.GetValue() == 7)
                                        .Count();
        Piece destinationPiece = chessboard.GetPieceAtGridPosition(destination);
        return allNumberOfThisPieces < 6
                && numberOfThisPieces <= 0
                && (positionOnBoard.y < 0 ^ destination.y >= 0)
                && destinationPiece != null
                && destinationPiece.GetValue() == PAWN_VALUE;
    }

    public override int GetValue() {
        return DRONE_VALUE;
    }

    public override Piece Clone(IChessboardActions chessboard) {
        return new DronePiece(positionOnBoard, chessboard);
    }
}
