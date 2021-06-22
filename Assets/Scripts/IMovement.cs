using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMovement {
    void ShowIndicator();
    void HideIndicator();

    void GetAvailablePositions();
    void CreateIndicatorMesh();

    bool CanMoveTo(Vector2Int position);
    IEnumerator Move(Vector2 destination);
    bool HasMoved();
}
