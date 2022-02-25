using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class OpponentAI
{
    private int depth = 7;

    private bool isMaximisingPlayer;
    private Player player;
    private CancellationTokenSource tokenSource;

    public OpponentAI(Player player, CancellationTokenSource tokenSource)
    {
        this.player = player;
        this.tokenSource = tokenSource;
        isMaximisingPlayer = player == Player.ONE ? true : false;
    }

    public (Vector2Int, Vector2Int) MakeDecision(IChessboardActions board)
    {
        Node node;
        if (board.GetAllPieces(Player.ALL).Length >= 7)
        {
            node = Minimax(new HypotheticalBoard(board), 4, isMaximisingPlayer, int.MinValue, int.MaxValue);
        } else if (board.GetAllPieces(Player.ALL).Length >= 4)
        {
            node = Minimax(new HypotheticalBoard(board), 6, isMaximisingPlayer, int.MinValue, int.MaxValue);
        } else
        {
            node = Minimax(new HypotheticalBoard(board), 8, isMaximisingPlayer, int.MinValue, int.MaxValue);
        }
        if (node.GetScore() == int.MinValue)
        {
            Debug.Log("I can see how I can win");
        }
        Debug.Log("My best predicated was " + node.GetScore());
        return (node.GetPieceOrigin(), node.GetPieceDestination());
    }

    private Node Minimax(IChessboardActions board, int depth, bool isMaximisingPlayer, int alpha, int beta)
    {
        if (depth == 0 || board.isGameOver() || tokenSource.IsCancellationRequested)
        {
            MovementChange lastMove = board.GetLastMove();
            return new Node(board.EvaluateScore(), lastMove.getOrigin(), lastMove.getDestination());
        }
        else
        {
            MovementChange lastMove = board.GetLastMove();
            int extremeValue = isMaximisingPlayer ? int.MinValue : int.MaxValue;
            Player currentPlayer = isMaximisingPlayer ? Player.ONE : Player.TWO;

            Node extremeNode = new Node(extremeValue, lastMove.getOrigin(), lastMove.getDestination());

            List<TransitionalBoard> choiceBoards = new List<TransitionalBoard>();

            foreach (Piece piece in board.GetAllPieces(currentPlayer))
            {
                foreach (Vector2Int position in piece.GetPossibleMovementPlaces(lastMove))
                {
                    HypotheticalBoard newBoard = new HypotheticalBoard(board);

                    Piece pieceToReplace = newBoard.GetPieceAtGridPosition(position);
                    if (pieceToReplace != null)
                    {
                        newBoard.RemovePieceAtPosition(position, player);
                    }

                    newBoard.MovePiecePosition(piece.GetPosition(), position, player);

                    choiceBoards.Add(new TransitionalBoard(board, newBoard));
                }
            }

            choiceBoards.Sort((x, y) => x.GetChangeValue().CompareTo(y.GetChangeValue()));

            foreach (TransitionalBoard movement in choiceBoards)
            {
                IChessboardActions newBoard = movement.GetBoard();
                Piece piece = movement.GetPieceMoved();
                Vector2Int position = newBoard.GetLastMove().getDestination();

                Node child = Minimax(newBoard, depth - 1, !isMaximisingPlayer, alpha, beta);

                if (isMaximisingPlayer)
                {
                    if (child.GetScore() > extremeNode.GetScore() || extremeNode.GetScore() == extremeValue)
                    {
                        extremeNode = new Node(child.GetScore(), piece.GetPosition(), position);
                    }

                    alpha = Mathf.Max(alpha, child.GetScore());
                }
                else
                {
                    if (child.GetScore() < extremeNode.GetScore() || extremeNode.GetScore() == extremeValue)
                    {
                        extremeNode = new Node(child.GetScore(), piece.GetPosition(), position);
                    }

                    beta = Mathf.Min(beta, child.GetScore());
                }

                if (beta <= alpha)
                {
                    break;
                }
            }
            return extremeNode;
        }
    }


    private bool isValidMove(Vector2Int origin, Vector2Int destination, IChessboardActions board)
    {
        return origin != board.GetLastMove().getDestination() && destination != board.GetLastMove().getOrigin();
    }
}
