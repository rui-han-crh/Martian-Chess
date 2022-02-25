using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[RequireComponent(typeof(PieceManager))]
public class MovePiece : MonoBehaviour {
    private InputBindings bindings;
    [SerializeField]
    private Tilemap tileMap;

    MovementMarkerManager markerManager;
    [SerializeField]
    private Piece selectedPiece;

    private bool isPlayerOneTurn = true;

    private PieceManager pieceManager;

    private void Awake() {
        bindings = new InputBindings();
        pieceManager = GetComponent<PieceManager>();
        markerManager = GetComponent<MovementMarkerManager>();
    }

    private void OnEnable() {
        bindings.Enable();
    }

    private void OnDisable() {
        bindings.Disable();
    }

    // Start is called before the first frame update
    private void Start() {
        
    }

    //private void MouseClick() {
    //    Vector2 worldMousePosition = bindings.Keyboard.MousePosition.ReadValue<Vector2>();
    //    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(worldMousePosition);
    //    Vector3Int gridPosition = tileMap.WorldToCell(mousePosition);
    //    Vector2Int gridPositionVector2Int = (Vector2Int)gridPosition;

    //    if (isPlayerOneTurn) {
    //        ControlPieces(gridPosition, gridPositionVector2Int);
    //    }
    //    print(pieceManager.EvaluateScore());
    //    if (pieceManager.isGameOver()) {
    //        print("gameover");
    //    }
    //}

    //private void ControlPieces(Vector3Int gridPosition, Vector2Int gridPositionVector2Int) {
    //    if (tileMap.HasTile(gridPosition)) {
    //        if (selectedPiece != null && selectedPiece.CanMoveToPosition(gridPositionVector2Int)) {
    //            if (true || gridPosition.y >= 0) {
    //                if (pieceManager.HasPieceAtPosition(gridPositionVector2Int)) {
    //                    pieceManager.RemovePieceAtPosition(gridPositionVector2Int, Player.ONE);
    //                }
    //                pieceManager.MovePiecePosition(selectedPiece, gridPositionVector2Int);
    //                UnsetSelectedPiece();
    //            } else {
    //                if (!pieceManager.HasPieceAtPosition(gridPositionVector2Int)) {
    //                    pieceManager.MovePiecePosition(selectedPiece, gridPositionVector2Int);
    //                    UnsetSelectedPiece();
    //                } else {
    //                    SelectPiece(gridPositionVector2Int);
    //                    markerManager.UpdateMarkers(selectedPiece);
    //                }
    //            }


    //        } else {
    //            if (true || gridPosition.y < 0 && pieceManager.HasPieceAtPosition(gridPositionVector2Int)) {
    //                SelectPiece(gridPositionVector2Int);
    //                markerManager.UpdateMarkers(selectedPiece);
    //            } else {
    //                UnsetSelectedPiece();
    //            }
    //        }
    //    } else {
    //        UnsetSelectedPiece();
    //    }
    //}

    

    /// <summary>
    /// Uses the minimax algorithm to make a decision
    /// </summary>
    

    private bool IsValidMove(IChessboardActions board, IChessboardActions newBoard) {
        MovementChange boardLast = board.GetLastMove();
        MovementChange newBoardLast = newBoard.GetLastMove();
        return boardLast.getOrigin() != newBoardLast.getDestination() 
            && boardLast.getDestination() != newBoardLast.getOrigin();
    }

    public void UnsetSelectedPiece() {
        selectedPiece = null;
        markerManager.RemoveAllMarkers();
    }

    private void SelectPiece(Vector2Int position) {
        selectedPiece = pieceManager.GetPieceAtGridPosition(position);
    }
}
