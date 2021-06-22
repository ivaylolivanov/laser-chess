using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightMovement : MonoBehaviour, IMovement {
    [SerializeField] private UnitStats stats;

    [Header("Indicator")]
    [SerializeField] private Color indicatorColor = Color. green;

    private bool hasMoved = false;

    private BoardGrid boardGrid;

    private Mesh         indicatorMesh;
    private MeshRenderer indicatorRenderer;

    private List<Vector2> availablePositions;

    private void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();

        InitializeIndicator();

        availablePositions = new List<Vector2>();
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

        if((availablePositions != null)
           && (availablePositions.Count > 0)
           &&  availablePositions.Contains(position)
        ) {
            result = true;
        }

        return result;
    }

    public void GetAvailablePositions() {
        boardGrid.GetFreeKnightPositions(
            transform.position,
            out availablePositions
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

        AddSquaresToMeshMesh(ref vertices, availablePositions);

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
        Vector2 originalPosition = transform.position;
        transform.position = destination;
        boardGrid.UpdateTile(transform.position);

        yield return new WaitForSeconds(stats.intervalBetweenSteps);

        boardGrid.UpdateTile(originalPosition);
    }

    private void AddSquaresToMeshMesh(
        ref Vector3[] meshVertices,
        List<Vector2> squaresOrigins
    ) {
        if((squaresOrigins == null) || (squaresOrigins.Count <= 0)) { return; }

        int index = 0;
        foreach(Vector2 point in squaresOrigins) {
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

    private int CalculateNumberOfSquares() {
        int vertices = availablePositions.Count;

        return vertices;
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
