using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenMovement : MonoBehaviour, IMovement {
    [SerializeField] private UnitStats stats;

    [Header("Indicator")]
    [SerializeField] private Color indicatorColor = Color. green;

    private bool hasMoved = false;

    private BoardGrid boardGrid;

    private Mesh         indicatorMesh;
    private MeshRenderer indicatorRenderer;

    private List<Vector2> left;
    private List<Vector2> leftDown;
    private List<Vector2> leftUp;
    private List<Vector2> right;
    private List<Vector2> rightDown;
    private List<Vector2> rightUp;
    private List<Vector2> down;
    private List<Vector2> up;

    private void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();

        InitializeIndicator();

        left      = new List<Vector2>();
        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        right     = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();
        down      = new List<Vector2>();
        up        = new List<Vector2>();
    }

    public void ShowIndicator() {
        indicatorRenderer.enabled = true;
    }

    public void HideIndicator() {
        indicatorRenderer.enabled = false;
    }

    public bool HasMoved() => hasMoved;

    public bool CanMoveTo (Vector2Int position) {
        bool result = Utils.CanMoveInDirection(left, position)
            || Utils.CanMoveInDirection(leftDown,    position)
            || Utils.CanMoveInDirection(leftUp,      position)
            || Utils.CanMoveInDirection(right,       position)
            || Utils.CanMoveInDirection(rightDown,   position)
            || Utils.CanMoveInDirection(rightUp,     position)
            || Utils.CanMoveInDirection(down,        position)
            || Utils.CanMoveInDirection(up,          position);

        return result;
    }

    public void GetAvailablePositions() {
        boardGrid.GetFreeQueenPositions(
            transform.position,
            out left,
            out leftDown,
            out leftUp,
            out right,
            out rightDown,
            out rightUp,
            out down,
            out up,
            stats.stepsLimit
        );
    }

    public void CreateIndicatorMesh() {
        int squaresCount = CalculateNumberOfSquares();

        int allVertices = squaresCount * 4;
        int trianglesCount = squaresCount * 2;
        int trianglesVertices = trianglesCount * 3;

        int[] triangles = new int[trianglesVertices];
        Vector3[] vertices = new Vector3[allVertices];
        Color[] verticesColors = new Color[allVertices];

        indicatorMesh.Clear();

        int verticesIndex = 0;
        AddMeshOrthogonal(ref verticesIndex, ref vertices, left, true);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, leftDown);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, leftUp);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, right, false);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, rightDown);
        AddMeshDiagonal(  ref verticesIndex, ref vertices, rightUp);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, down, true);
        AddMeshOrthogonal(ref verticesIndex, ref vertices, up, false);

        int triangleVertexIndex = 0;
        for(int i = 0; i < squaresCount; ++i) {
            triangles[triangleVertexIndex] = i * 4;       ++triangleVertexIndex;
            triangles[triangleVertexIndex] = (i * 4) + 2; ++triangleVertexIndex;
            triangles[triangleVertexIndex] = (i * 4) + 1; ++triangleVertexIndex;
            triangles[triangleVertexIndex] = (i * 4) + 2; ++triangleVertexIndex;
            triangles[triangleVertexIndex] = (i * 4) + 3; ++triangleVertexIndex;
            triangles[triangleVertexIndex] = (i * 4) + 1; ++triangleVertexIndex;

            verticesColors[i * 4] = indicatorColor;
            verticesColors[(i * 4) + 1] = indicatorColor;
            verticesColors[(i * 4) + 2] = indicatorColor;
            verticesColors[(i * 4) + 3] = indicatorColor;
        }

        indicatorMesh.vertices  = vertices;
        indicatorMesh.colors    = verticesColors;
        indicatorMesh.triangles = triangles;
        indicatorMesh.RecalculateNormals();
    }

    public IEnumerator Move(Vector2 destination) {
        List<Vector2> direction = DecideMoveDirection(destination);

        Vector2 originalPosition = transform.position;
        for(int i = 0; i < direction.Count; ++i) {
            boardGrid.UpdateTile(transform.position);

            if((Vector2)transform.position == destination) { break; }
            transform.position = direction[i];

            yield return new WaitForSeconds(stats.intervalBetweenSteps);

            if((i - 1) < 0) { boardGrid.UpdateTile(originalPosition); }
            else { boardGrid.UpdateTile(direction[i-1]); }
        }

        hasMoved = true;
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

    private int CalculateNumberOfSquares() {
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

    private List<Vector2> DecideMoveDirection(Vector2 destination) {
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

    private void InitializeIndicator() {
        indicatorRenderer = GetComponent<MeshRenderer>();
        indicatorRenderer.sortingLayerName = Utils.UNITS_SORTING_LAYER;
        indicatorRenderer.sortingOrder     = 0;

        indicatorMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = indicatorMesh;

        HideIndicator();
    }
}
