using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthogonalMovement : Movement {
    private List<Vector2> left;
    private List<Vector2> right;
    private List<Vector2> down;
    private List<Vector2> up;

    private void Awake() {
        base.Awake();

        left  = new List<Vector2>();
        right = new List<Vector2>();
        down  = new List<Vector2>();
        up    = new List<Vector2>();
    }

    public override void GetAvailablePositions() {
        boardGrid.GetOrthogonalPositions(
            transform.position,
            out left,
            out right,
            out down,
            out up,
            stats.stepsLimit
        );

        if(allDirections != null) { allDirections.Clear(); }

        allDirections.AddRange(left);
        allDirections.AddRange(right);
        allDirections.AddRange(down);
        allDirections.AddRange(up);
    }

    protected override List<Vector2> DecideMoveDirection(Vector2 destination) {
        List<Vector2> direction = new List<Vector2>();
        if(destination.y == transform.position.y) {
            if(destination.x < transform.position.x) { direction = left; }
            if(destination.x > transform.position.x) { direction = right; }
        }

        if(destination.x == transform.position.x) {
            if(destination.y < transform.position.y) { direction = down; }
            if(destination.y > transform.position.y) { direction = up; }
        }

        return direction;
    }

    protected override void FillMeshVertices(ref Vector3[] vertices) {
        int verticesIndex = 0;

        AddMeshDirection(ref verticesIndex, ref vertices, left, true);
        AddMeshDirection(ref verticesIndex, ref vertices, right);
        AddMeshDirection(ref verticesIndex, ref vertices, down, true);
        AddMeshDirection(ref verticesIndex, ref vertices, up);
    }

    protected override int CalculateNumberOfRectangles() {
        int vertices = ((left.Count > 0) ? 1 : 0)
            + ((right.Count > 0) ? 1 : 0)
            + ((down.Count > 0) ? 1 : 0)
            + ((up.Count > 0) ? 1 : 0);

        return vertices;
    }

    private void AddMeshDirection(
        ref int index,
        ref Vector3[] meshVertices,
        List<Vector2> direction,
        bool reverse = false
    ) {
        if(direction.Count <= 0) { return; }

        Vector2 bottomLeft = direction[0];
        Vector2 topRight   = direction[direction.Count - 1] + Vector2.one;

        if(reverse) {
            bottomLeft = direction[direction.Count - 1];
            topRight   = direction[0] + Vector2.one;
        }

        Vector3[] rectVertices = Utils.GetRectangleVertices(
            bottomLeft,
            topRight
        );

        foreach(Vector3 vertex in rectVertices) {
            meshVertices[index] = vertex - transform.position;
            ++index;
        }
    }
}
