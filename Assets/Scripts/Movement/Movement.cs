using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    [SerializeField] protected Stats stats;

    [Header("Indicator")]
    [SerializeField] protected Color indicatorColor = Color.green;

    public bool hasMoved = false;

    protected BoardGrid boardGrid;

    protected MeshFilter indicatorMeshFilter;
    protected MeshRenderer indicatorRenderer;

    protected List<Vector2> allDirections;

    protected void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();

        allDirections = new List<Vector2>();

        InitializeIndicator();
    }

    public void ShowIndicator() { indicatorRenderer.enabled = true; }
    public void HideIndicator() { indicatorRenderer.enabled = false; }

    public bool HasMovementOptions() {
        bool result = (allDirections != null) && (allDirections.Count > 0);

        return result;
    }

    public bool CanMoveTo (Vector2Int position) {
        bool result = allDirections.Contains(position);

        return result;
    }

    public virtual void GetAvailablePositions() {}
    protected virtual List<Vector2> DecideMoveDirection(Vector2 destination)
        => allDirections;
    protected virtual void FillMeshVertices(ref Vector3[] vertices) {}
    protected virtual int CalculateNumberOfRectangles() => 0;

    public virtual void Move(Vector2 destination) {
        Vector2 originalPosition = transform.position;
        transform.position = destination;

        StartCoroutine(
            DelayedGridUpdate(
                originalPosition,
                destination
            )
        );

        hasMoved = true;
    }

    public List<Vector2> GetAllPossiblePositions() {
        return allDirections;
    }

    public Vector2 GetPositionClosestTo(Vector2 targetPosition) {
        Vector2 result = Vector2.zero;

        float minDistance = float.MaxValue;
        foreach(Vector2 position in allDirections) {
            float sqrDistance = (targetPosition - position).sqrMagnitude;

            if(minDistance > sqrDistance) {
                minDistance = sqrDistance;
                result = position;
            }
        }

        return result;
    }

    public void CreateIndicatorMesh() {
        Mesh indicatorMesh = new Mesh();
        GetComponent<MeshFilter>().mesh = indicatorMesh;

        int squaresCount = CalculateNumberOfRectangles();

        int allVertices = squaresCount * 4;
        int trianglesCount = squaresCount * 2;
        int trianglesVertices = trianglesCount * 3;

        int[] triangles = new int[trianglesVertices];
        Vector3[] vertices = new Vector3[allVertices];
        Color[] verticesColors = new Color[allVertices];

        indicatorMesh.Clear();

        FillMeshVertices(ref vertices);

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

    private IEnumerator DelayedGridUpdate(
        Vector3 lastPosition,
        Vector3 currentPosition
    ) {
        yield return new WaitForSeconds(0.2f);

        boardGrid.UpdateTile(lastPosition);
        boardGrid.UpdateTile(currentPosition);
    }

    protected void InitializeIndicator() {
        indicatorMeshFilter = GetComponent<MeshFilter>();
        indicatorRenderer = GetComponent<MeshRenderer>();

        indicatorRenderer.sortingLayerName = Utils.UNITS_SORTING_LAYER;
        indicatorRenderer.sortingOrder     = 0;

        HideIndicator();
    }
}
