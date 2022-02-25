using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private int score;
    private Vector2Int pieceMovedOrigin;
    private Vector2Int pieceMovedDestination;
    private bool cannotBeRetrieved;

    public Node(int score, Vector2Int pieceMovedOrigin, Vector2Int pieceMovedDestination) {
        this.score = score;
        this.pieceMovedOrigin = pieceMovedOrigin;
        this.pieceMovedDestination = pieceMovedDestination;
        this.cannotBeRetrieved = false;
    }

    private Node(int score) {
        this.cannotBeRetrieved = true;
        this.score = score;
    }

    public static Node MinimumNode() {
        return new Node(int.MinValue);
    }

    public static Node MaximumNode() {
        return new Node(int.MaxValue);
    }

    public int GetScore() {
        return score;
    }

    public Vector2Int GetPieceOrigin() {
        if (cannotBeRetrieved) {
            throw new Exception("Node cannot have movements be retrieved");
        }
        return pieceMovedOrigin;
    }

    public Vector2Int GetPieceDestination() {
        if (cannotBeRetrieved) {
            throw new Exception("Node cannot have movements be retrieved");
        }
        return pieceMovedDestination;
    }
}
