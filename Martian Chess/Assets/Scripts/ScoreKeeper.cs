using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper {
    private int score;

    private int playerOneScore = 0;
    private int playerTwoScore = 0;

    public void UpdateScore(IChessboardActions board) {
        score = 0;
        bool piecesExistOne = false;
        bool piecesExistTwo = false;
        
        for (int x = -2; x < 2; x++) {
            for (int y = -4; y < 4; y++) {
                Piece piece = board.GetPieceAtGridPosition(new Vector2Int(x, y));
                if (piece == null) {
                    if (y < 0) {
                        score += 10;
                    } else {
                        score -= 10;
                    }
                } else {
                    if (y < 0) {
                        piecesExistOne = true;
                        score += piece.GetValue() - 10;
                    } else {
                        piecesExistTwo = true;
                        score -= piece.GetValue() - 10;
                    }
                }
            }
        }

        if (!piecesExistOne) {
            score = int.MaxValue;
            return;
        }

        if (!piecesExistTwo) {
            score = int.MinValue;
            return;
        }
        score += playerOneScore + playerTwoScore;
    }

    public void StoreCapturedPiece(Piece piece, Player player) {
        if (player == Player.ONE) {
            playerOneScore += piece.GetValue();
        } else {
            playerTwoScore -= piece.GetValue();
        }
    }

    public int GetCapturedScore() {
        return playerOneScore + playerTwoScore;
    }

    public int GetScore() {
        return score;
    }

    public ScoreKeeper Clone() {
        ScoreKeeper scoreKeeper = new ScoreKeeper();
        scoreKeeper.playerOneScore = playerOneScore;
        scoreKeeper.playerTwoScore = playerTwoScore;
        return scoreKeeper;
    }
}
public enum Player { 
    ONE, TWO, ALL
}
