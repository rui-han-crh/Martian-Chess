using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChessBoardBehaviour : MonoBehaviour {
    private static readonly int xSize = 4;
    private static readonly int xSizeOffset = xSize / 2;
    private static readonly int ySize = 8;
    private static readonly int ySizeOffSet = ySize / 2;
    private static readonly float LINE_WIDTH = 0.05f;

    private InputBindings bindings;

    private Vector3 offset = new Vector3(0.5f, 0.5f, 0);

    [SerializeField]
    private Tilemap tileMap;

    [SerializeField]
    private Piece selectedPiece;
    private MovementMarkerManager markerManager;

    private LineRenderer lineRenderer;

    [SerializeField]
    private GameObject winFlag;
    [SerializeField]
    private GameObject loseFlag;
    [SerializeField]
    private GameObject thinkingIndicator;

    [SerializeField]
    private TextMeshProUGUI playerOneScoreObject, playerTwoScoreObject;


    [SerializeField]
    private GameObject[] pieceGameObjects;

    [SerializeField]
    private GameObject ghostPiece;
    [SerializeField]
    private GameObject PawnPrefab, DronePrefab, QueenPrefab;

    private IChessboardActions chessboard;

    private OpponentAI opponentAI;
    private OpponentAI opponentAI2;

    private bool onesTurn = true;

    private bool adversaryThinking = false;

    public bool startFirst;

    private CancellationTokenSource tokenSource;


    public void Awake() {
        bindings = new InputBindings();
        tokenSource = new CancellationTokenSource();
        opponentAI = new OpponentAI(Player.TWO, tokenSource);
        opponentAI2 = new OpponentAI(Player.ONE, tokenSource);
        ghostPiece = Instantiate(ghostPiece);
        ghostPiece.transform.position = int.MaxValue * Vector2.one;
    }

    private void OnEnable() {
        bindings.Enable();
    }

    private void OnDisable() {
        tokenSource.Cancel();
        bindings.Disable();
    }

    public void Start() {
        bindings.Keyboard.MouseClick.performed += ctx => MouseClick();

        markerManager = GetComponent<MovementMarkerManager>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = LINE_WIDTH;
        lineRenderer.endWidth = LINE_WIDTH;
        lineRenderer.startColor = lineRenderer.endColor = Color.red;

        chessboard = new HypotheticalBoard(pieceGameObjects, tileMap);

        foreach (GameObject pieceObject in pieceGameObjects)
        {
            UpdatePieceObjectPositionsByBoard(chessboard);
        }

        if (!startFirst) {
            AdversaryDecision();
        }
    }

    //private void MouseClickTwo() {
    //    if (!isGameOver()) {
    //        Vector2Int origin;
    //        Vector2Int destination;
    //        if (onesTurn) {
    //            (origin, destination) = opponentAI2.MakeDecision(this);
    //            onesTurn = false;
    //        } else {
    //            (origin, destination) = opponentAI.MakeDecision(this);
    //            onesTurn = true;
    //        }
    //        print("Move this " + origin);
    //        Piece pieceToMove = GetPieceAtGridPosition(origin);
    //        Piece pieceToReplace = GetPieceAtGridPosition(destination);
    //        if (pieceToReplace != null) {
    //            RemovePieceAtPosition(destination, Player.TWO);
    //        }
    //        MovePiecePosition(pieceToMove, destination);
    //    } else {
    //        print("Game already over");
    //    }
    //}

    private void MouseClick() {
        Vector2 worldMousePosition = bindings.Keyboard.MousePosition.ReadValue<Vector2>();
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(worldMousePosition);
        Vector3Int gridPosition = tileMap.WorldToCell(mousePosition);
        Vector2Int gridPositionVector2Int = (Vector2Int)gridPosition;

        if (!chessboard.isGameOver())
        {
            ControlPieces(gridPosition, gridPositionVector2Int);
        }

        //print("The score now is " + EvaluateScore());
        //print("Moves left -> " + movesUntilEnd);
    }

    private async void AdversaryDecision() {
        Vector2Int origin;
        Vector2Int destination;

        adversaryThinking = true;
        thinkingIndicator.SetActive(adversaryThinking);

        (origin, destination) = await Task.Run(() => opponentAI.MakeDecision(chessboard));

        if (tokenSource.IsCancellationRequested)
        {
            return;
        }

        Piece pieceToMove = chessboard.GetPieceAtGridPosition(origin);
        Piece pieceToReplace = chessboard.GetPieceAtGridPosition(destination);

        if (pieceToReplace != null && (pieceToMove.GetPosition().y < 0 ^ pieceToReplace.GetPosition().y < 0)) {
            chessboard.RemovePieceAtPosition(destination, Player.TWO);
            print("Adversary capture piece");
        }


        SetLineRendererPath(pieceToMove.GetPosition(), destination);

        DisplayGhostPiece(pieceToMove.GetPosition());

        chessboard.MovePiecePosition(pieceToMove, destination);


        UpdatePieceObjectPositionsByBoard(chessboard);

        adversaryThinking = false;
        thinkingIndicator.SetActive(adversaryThinking);
        UpdateScoreBoard();

        if (chessboard.isGameOver())
        {
            ActivateGameOver();
        }
    }

    private void SetLineRendererPath(Vector2Int origin, Vector2Int destination)
    {
        Vector3 originalPiecePosition = tileMap.CellToWorld((Vector3Int)origin) + offset + Vector3.back;
        Vector3 destinationPosition = tileMap.CellToWorld((Vector3Int)destination) + offset + Vector3.back;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, originalPiecePosition);
        lineRenderer.SetPosition(1, destinationPosition);
    }

    private void UnsetLineRendererPath()
    {
        lineRenderer.positionCount = 0;
    }

    private void ControlPieces(Vector3Int gridPosition, Vector2Int gridPositionVector2Int) {
        if (tileMap.HasTile(gridPosition)) {
            if (selectedPiece != null 
                    && selectedPiece.CanMoveToPosition(gridPositionVector2Int, chessboard.GetLastMove()) 
                    && !adversaryThinking) {
                HideGhostPiece();
                if (gridPosition.y >= 0) {
                    
                    if (chessboard.HasPieceAtPosition(gridPositionVector2Int)) {
                        chessboard.RemovePieceAtPosition(gridPositionVector2Int, Player.ONE);
                    }
                    chessboard.MovePiecePosition(selectedPiece, gridPositionVector2Int);
                    UnsetSelectedPiece();
                } else {
                    chessboard.MovePiecePosition(selectedPiece, gridPositionVector2Int);
                    UnsetSelectedPiece();
                }

                UpdatePieceObjectPositionsByBoard(chessboard);
                UnsetLineRendererPath();
                UpdateScoreBoard();

                print("Moves left " + chessboard.GetMovesLeft());

                print(chessboard.GetLastMove());
                //---------------Adversary-----------------
                if (!chessboard.isGameOver()) {
                    AdversaryDecision();
                } else {
                    ActivateGameOver();
                }
                //---------------Adversary-----------------
            } else {
                if (gridPosition.y < 0 && chessboard.HasPieceAtPosition(gridPositionVector2Int)) {
                    SelectPiece(gridPositionVector2Int);
                    markerManager.UpdateMarkers(selectedPiece, chessboard.GetLastMove());
                } else {
                    UnsetSelectedPiece();
                }
            }
        } else {
            UnsetSelectedPiece();
        }
    }

    private void UpdatePieceObjectPositionsByBoard(IChessboardActions chessboard)
    {
        for (int i = 0; i < pieceGameObjects.Length; i++)
        {
            Destroy(pieceGameObjects[i]);
            pieceGameObjects[i] = null;
        }

        Piece[] newPieces = chessboard.GetAllPieces(Player.ALL);
        for (int i = 0; i < newPieces.Length; i++)
        {
            Piece piece = newPieces[i];
            Vector3 position = tileMap.CellToWorld((Vector3Int)piece.GetPosition()) + offset;
            if (newPieces[i] is DronePiece)
            {
                pieceGameObjects[i] = Instantiate(DronePrefab, position, Quaternion.identity);
            }
            else if (newPieces[i] is PawnPiece)
            {
                pieceGameObjects[i] = Instantiate(PawnPrefab, position, Quaternion.identity);
            }
            else
            {
                pieceGameObjects[i] = Instantiate(QueenPrefab, position, Quaternion.identity);
            }
        }
    }

    private void UpdateScoreBoard()
    {
        ScoreKeeper scoreKeeper = chessboard.GetScoreKeeper();
        print(scoreKeeper.GetPlayerOneCapturedScore());
        playerOneScoreObject.text = scoreKeeper.GetPlayerOneCapturedScore().ToString();
        playerTwoScoreObject.text = scoreKeeper.GetPlayerTwoCapturedScore().ToString();
        Debug.Log(chessboard.GetAllPieces(Player.ALL).Count());
    }

    //private void PromotePiece(Piece selectedPiece, Vector2Int gridPositionVector2Int)
    //{
    //    Piece pieceAtDestination = GetPieceAtGridPosition(gridPositionVector2Int);

    //    RemovePieceAtPosition(selectedPiece.GetPosition());
    //    RemovePieceAtPosition(gridPositionVector2Int);

    //    Piece promotedPiece = null;

    //    for (int i = 0; i < pieceGameObjects.Length && promotedPiece == null; i++)
    //    {
    //        GameObject gamePiece = pieceGameObjects[i];
    //        if (!gamePiece.activeInHierarchy 
    //            && gamePiece.name.StartsWith(GetPromotedPieceString(selectedPiece, pieceAtDestination)))
    //        {
    //            gamePiece.SetActive(true);
    //            promotedPiece = GetPieceFromGameObject(gamePiece, gridPositionVector2Int);
    //            Vector2Int piecePosition = promotedPiece.GetPosition();
    //            gamePiece.transform.position = tileMap.CellToWorld((Vector3Int)piecePosition) + offset;
    //            pieceLocations[piecePosition.x + xSizeOffset, piecePosition.y + ySizeOffSet]
    //            = (gamePiece, promotedPiece);
    //        }
    //    }
    //    MovePiecePosition(promotedPiece, gridPositionVector2Int);
    //}

    private void ActivateGameOver() {
        if (chessboard.EvaluateScore() > 0) {
            winFlag.SetActive(true);
        } else {
            loseFlag.SetActive(true);
        }
    }

    private void HideGhostPiece()
    {
        ghostPiece.SetActive(false);
    }

    private void DisplayGhostPiece(Vector2Int gridPosition)
    {
        ghostPiece.SetActive(true);
        ghostPiece.transform.position = tileMap.CellToWorld((Vector3Int)gridPosition) + offset;
    }

    public void UnsetSelectedPiece() {
        selectedPiece = null;
        markerManager.RemoveAllMarkers();
    }

    private void SelectPiece(Vector2Int gridPosition) {
        selectedPiece = chessboard.GetPieceAtGridPosition(gridPosition);
    }


    public void UpdatePieceGameObjectPosition(GameObject pieceObject) {
        pieceObject.transform.position = tileMap.CellToWorld(
                                                tileMap.WorldToCell(pieceObject.transform.position)
                                                ) + offset;
    }

}

// script getting a bit long - line 201
