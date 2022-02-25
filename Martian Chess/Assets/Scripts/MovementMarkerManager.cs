using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementMarkerManager : MonoBehaviour
{
    private Vector2Int[] markerPositions;
    private List<GameObject> markers = new List<GameObject>();
    [SerializeField]
    private GameObject markerPrefab;
    private Vector2 offset = new Vector2(0.5f, 0.5f);

    public void UpdateMarkers(Piece piece, MovementChange movementChange) {
        RemoveAllMarkers();
        markerPositions = piece.GetPossibleMovementPlaces(movementChange);
        foreach (Vector2Int position in markerPositions) {
            markers.Add(Instantiate(markerPrefab, position + offset, Quaternion.identity));
        }
    }

    public void RemoveAllMarkers() {
        foreach (GameObject marker in markers) {
            Destroy(marker);
        }
        markers.Clear();
    }
}
