using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenMovement : Movement {
    private List<Vector2> left;
    private List<Vector2> leftDown;
    private List<Vector2> leftUp;
    private List<Vector2> right;
    private List<Vector2> rightDown;
    private List<Vector2> rightUp;
    private List<Vector2> down;
    private List<Vector2> up;

    private void Awake() {
        base.Awake();

        left      = new List<Vector2>();
        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        right     = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();
        down      = new List<Vector2>();
        up        = new List<Vector2>();
    }

    public override void GetAvailablePositions() {
        boardGrid.GetQueenPositions(
            transform.position,
            out left,
            out leftDown,
            out leftUp,
            out right,
            out rightDown,
            out rightUp,
            out down,
            out up,
            stats.stepsLimit,
            Utils.TILE_VALUE_FREE
        );

        if(allDirections != null) { allDirections.Clear(); }

        allDirections.AddRange(left);
        allDirections.AddRange(leftDown);
        allDirections.AddRange(leftUp);
        allDirections.AddRange(right);
        allDirections.AddRange(rightDown);
        allDirections.AddRange(rightUp);
        allDirections.AddRange(down);
        allDirections.AddRange(up);
    }

    protected override List<Vector2> DecideMoveDirection(Vector2 destination) {
        List<Vector2> direction = new List<Vector2>();

        if(destination.x < transform.position.x) {
            if(destination.y == transform.position.y) {
                direction = left;
            }
            else if(destination.y < transform.position.y) {
                direction = leftDown;
            }
            else { direction = leftUp; }
        }
        else if(destination.x > transform.position.x) {
            if(destination.y == transform.position.y) {
                direction = right;
            }
            else if(destination.y < transform.position.y) {
                direction = rightDown;
            }
            else { direction = rightUp; }
        }
        else {
            if(destination.y < transform.position.y) { direction = down; }
            if(destination.y > transform.position.y) { direction = up; }
        }

        return direction;
    }

    protected override void FillMeshVertices(ref Vector3[] vertices) {
        int verticesIndex = 0;

        AddMeshOrthogonal(ref verticesIndex, ref vertices, left, true);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, leftDown);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, leftUp);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, right, false);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, rightDown);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, rightUp);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, down, true);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, up, false);

    }

    protected override int CalculateNumberOfRectangles() {
        int vertices = ((left.Count > 0) ? 1 : 0)
            + leftDown.Count
            + leftUp.Count
            + ((right.Count > 0) ? 1 : 0)
            + rightDown.Count
            + rightUp.Count
            + ((down.Count > 0) ? 1 : 0)
            + ((up.Count > 0) ? 1 : 0);

        return vertices;
    }

    private void AddMeshOrthogonal(
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

    private void AddMeshDiagonal(
        ref int index,
        ref Vector3[] meshVertices,
        List<Vector2> direction
    ) {
        if(direction == null || direction.Count <= 0) { return; }

        foreach(Vector2 point in direction) {
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
}
