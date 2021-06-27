using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    [SerializeField] protected Stats stats;
    [SerializeField] protected GameObject explosionPrefab;

    [Header("Indicator")]
    [SerializeField] protected Color indicatorColor = Color.red;

    public bool hasAttacked = false;

    protected BoardGrid boardGrid;

    protected MeshFilter   indicatorMeshFilter;
    protected MeshRenderer indicatorRenderer;

    protected HashSet<Vector2> allDirections;

    protected int targetTileValue = 0;

    protected void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();

        allDirections = new HashSet<Vector2>();

        Unit myUnit = GetComponent<PlayerUnit>();
        if(myUnit != null) { targetTileValue = Utils.TILE_VALUE_ENEMY; }
        else { targetTileValue = Utils.TILE_VALUE_PLAYER; }

        InitializeIndicator();
    }

    public virtual void GetAvailablePositions() { }

    public virtual void Execute(Vector2 position) {
        int tileValue = boardGrid.GetTileValue(position);

        bool tileIsTarget = tileValue == targetTileValue;
        if(! tileIsTarget) { return; }

        Collider2D targetCollider = Physics2D.OverlapCircle(
            position + (Vector2.one * 0.5f),
            0.1f
        );
        if(targetCollider == null) { return; }

        Unit targetUnit = targetCollider.GetComponent<Unit>();
        if(targetUnit == null) { return; }

        GameObject explosion = Instantiate(
            explosionPrefab,
            position + (Vector2.one * 0.5f),
            Quaternion.identity
        );

        targetUnit.TakeDamage(stats.attackPower);

        Destroy(explosion, 2f);

        hasAttacked = true;
    }

    public void ShowIndicator() { indicatorRenderer.enabled = true; }
    public void HideIndicator() { indicatorRenderer.enabled = false; }

    public bool HasAttackOptions() {
        bool result = false;

        foreach(Vector2 position in allDirections) {
            int tileValue = boardGrid.GetTileValue(position);
            if(tileValue == Utils.TILE_VALUE_ENEMY) {
                result = true;
                break;
            }
        }

        return result;
    }

    public bool CanAttackOn(Vector2Int position) {
        bool result = allDirections.Contains(position);

        if(! result) { return result; }

        int tileValue = boardGrid.GetTileValue(position);
        result = (tileValue == targetTileValue);

        return result;
    }

    public void CreateIndicatorMesh() {
        Mesh indicatorMesh = new Mesh();
        indicatorMeshFilter.mesh = indicatorMesh;

        int squaresCount = CalculateNumberOfSquares();

        int allVertices = squaresCount * 4;
        int trianglesCount = squaresCount * 2;
        int trianglesVertices = trianglesCount * 3;

        int[] triangles = new int[trianglesVertices];
        Vector3[] vertices = new Vector3[allVertices];
        Color[] verticesColors = new Color[allVertices];

        indicatorMesh.Clear();

        AddMeshSquares(ref vertices);

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

    protected void AddMeshSquares(ref Vector3[] meshVertices) {
        int index = 0;

        foreach(Vector2 point in allDirections) {
            int tileValue = boardGrid.GetTileValue(point);
            if(tileValue != Utils.TILE_VALUE_ENEMY) { continue; }

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

    protected int CalculateNumberOfSquares() {
        int result = CountVisibleEnemies();

        return result;
    }

    protected int CountVisibleEnemies() {
        int result = 0;

        foreach(Vector2 position in allDirections) {
            int tileValue = boardGrid.GetTileValue(position);
            if(tileValue == targetTileValue) { result++; }
        }

        return result;
    }

    protected void InitializeIndicator() {
        indicatorMeshFilter = GetComponent<MeshFilter>();
        indicatorRenderer = GetComponent<MeshRenderer>();

        indicatorRenderer.sortingOrder     = 0;
        indicatorRenderer.sortingLayerName = Utils.UNITS_SORTING_LAYER;

        HideIndicator();
    }
}
