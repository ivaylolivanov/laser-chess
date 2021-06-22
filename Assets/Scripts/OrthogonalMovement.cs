using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthogonalMovement : MonoBehaviour, IMovement {
    [SerializeField] private UnitStats stats;

    [Header("Indicator")]
    [SerializeField] private Color indicatorColor = Color.green;

    private bool hasMoved = false;

    private BoardGrid boardGrid;

    private Mesh         indicatorMesh;
    private MeshRenderer indicatorRenderer;

    private List<Vector2> left;
    private List<Vector2> right;
    private List<Vector2> down;
    private List<Vector2> up;

    private void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();

        InitializeIndicator();

        left  = new List<Vector2>();
        right = new List<Vector2>();
        down  = new List<Vector2>();
        up    = new List<Vector2>();
    }

    public void ShowIndicator() {
        indicatorRenderer.enabled = true;
    }

    public void HideIndicator() {
        indicatorRenderer.enabled = false;
    }

    public bool HasMoved() => hasMoved;

    public bool CanMoveTo (Vector2Int position) {
        bool result = false;

        if((left     != null && left.Count  > 0 &&  left.Contains(position))
           || (right != null && right.Count > 0 && right.Contains(position))
           || (down  != null && down.Count  > 0 &&  down.Contains(position))
           || (up    != null && up.Count    > 0 &&    up.Contains(position))
        ) {
            result = true;
        }

        return result;
    }

    public void GetAvailablePositions() {
        if(left  != null) {  left.Clear(); left  = null; }
        if(right != null) { right.Clear(); right = null; }
        if(down  != null) {  down.Clear(); down  = null; }
        if(up    != null) {    up.Clear(); up    = null; }

        boardGrid.GetFreeOrthogonalPositions(
            transform.position,
            out left,
            out right,
            out down,
            out up,
            stats.stepsLimit
        );
    }

    public void CreateIndicatorMesh() {
        int squaresCount = CalculateNumberOfRectangles();

        int allVertices = squaresCount * 4;
        int trianglesCount = squaresCount * 2;
        int trianglesVertices = trianglesCount * 3;

        int[] triangles = new int[trianglesVertices];
        Vector3[] vertices = new Vector3[allVertices];
        Color[] verticesColors = new Color[allVertices];

        indicatorMesh.Clear();

        int verticesIndex = 0;
        AddMeshDirection(ref verticesIndex, ref vertices, left, true);
        AddMeshDirection(ref verticesIndex, ref vertices, right, false);
        AddMeshDirection(ref verticesIndex, ref vertices, down, true);
        AddMeshDirection(ref verticesIndex, ref vertices, up, false);

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
        indicatorMesh.triangles = triangles;
        indicatorMesh.colors    = verticesColors;
        indicatorMesh.RecalculateNormals();
    }

    public IEnumerator Move(Vector2 destination) {
        List<Vector2> direction = new List<Vector2>();
        if(destination.y == transform.position.y) {
            if(destination.x < transform.position.x) { direction = left; }
            if(destination.x > transform.position.x) { direction = right; }
        }

        if(destination.x == transform.position.x) {
            if(destination.y < transform.position.y) { direction = down; }
            if(destination.y > transform.position.y) { direction = up; }
        }

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

    private int CalculateNumberOfRectangles() {
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

    private void InitializeIndicator() {
        indicatorMesh = new Mesh();

        indicatorRenderer = GetComponent<MeshRenderer>();
        indicatorRenderer.sortingOrder     = 0;
        indicatorRenderer.sortingLayerName = Utils.UNITS_SORTING_LAYER;

        GetComponent<MeshFilter>().mesh = indicatorMesh;

        HideIndicator();
    }
}
