using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : Movement {
    private void Awake() { base.Awake(); }

    public override void GetAvailablePositions() {
        boardGrid.GetFreeKnightPositions(
            transform.position,
            out allDirections
        );
    }

    protected override void FillMeshVertices(ref Vector3[] meshVertices) {
        int index = 0;
        foreach(Vector2 point in allDirections) {
            Vector3[] squareVertices = Utils.GetRectangleVertices(
                point,
                point + Vector2.one
            );

            foreach(Vector3 vertex in squareVertices) {
                meshVertices[index] = vertex - transform.position;
                ++index;
            }
        }
    }

    protected override int CalculateNumberOfRectangles() => allDirections.Count;
}
