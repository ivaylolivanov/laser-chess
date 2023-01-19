using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour {
    [Header("Grid")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float gridLineWidth;
    [SerializeField] private float tileSize;
    [SerializeField] private LayerMask obstaclesMask;

    [Space] [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private int  debugTextSize;

    private Grid boardGrid;
    private TextMesh[, ] boardGridDebug;

    private MeshFilter   gridMeshFilter;
    private MeshRenderer gridRenderer;

    void Awake() {
        boardGrid = new Grid(width, height, tileSize, obstaclesMask);

        if(debug) {
            boardGridDebug = new TextMesh[width, height];
            DebugVisualize();
        }

        gridMeshFilter = GetComponent<MeshFilter>();
        gridRenderer   = GetComponent<MeshRenderer>();

        gridRenderer.sortingOrder = 0;
        gridRenderer.sortingLayerName = Utils.INDICATOR_SORTING_LAYER;

        CreateGridMesh();
    }

    public int GetTileValue(Vector2Int position) {
        Vector3Int position3Int = new Vector3Int(position.x, position.y, 0);
        int result = boardGrid.GetTileValue(position3Int);

        return result;
    }

    public int GetTileValue(Vector3 position) {
        int result = boardGrid.GetTileValue(position);

        return result;
    }

    public int GetTileValue(Vector3Int position) {
        int result = boardGrid.GetTileValue(position);

        return result;
    }

    public void SetTile(Vector2Int position, int value) {
        Vector3Int position3Int = new Vector3Int(position.x, position.y, 0);
        boardGrid.SetTile(position3Int, value);
    }

    public void SetTile(Vector3Int position, int value) {
        boardGrid.SetTile(position, value);
    }

    public void SetTile(Vector3 position, int value) {
        boardGrid.SetTile(position, value);
    }

    public void UpdateTile(Vector3 position) {
        Vector3Int positionInt = Vector3Int.FloorToInt(position);
        boardGrid.UpdateTile(positionInt);

        int newValue = boardGrid.GetTileValue(positionInt);

        if(debug && boardGridDebug[positionInt.x, positionInt.y] != null) {
            boardGridDebug[positionInt.x, positionInt.y].text
                = newValue.ToString();
        }
    }

    public void GetFreeKnightPositions(
        Vector2 origin,
        out List<Vector2> positions
    ) {
        Vector2Int originInt = Vector2Int.FloorToInt(origin);
        positions = new List<Vector2>();

        int[] xPoints = new int[] { -1, -2, -2, -1, 1, 2,  2,  1 };
        int[] yPoints = new int[] { -2, -1,  1,  2, 2, 1, -1, -2 };

        for(int i = 0; i < Utils.KNIGHT_POSSIBLE_POSITIONS; ++i) {
            Vector2 knightPosition = originInt
                + new Vector2(xPoints[i], yPoints[i]);
            int tileValue = boardGrid.GetTileValue(knightPosition);
            if(tileValue == 0) {
                positions.Add(knightPosition);
            }
        }
    }

    public void GetQueenPositions(
        Vector2 origin,
        out List<Vector2> left,
        out List<Vector2> leftDown,
        out List<Vector2> leftUp,
        out List<Vector2> right,
        out List<Vector2> rightDown,
        out List<Vector2> rightUp,
        out List<Vector2> down,
        out List<Vector2> up,
        int limit,
        int allowedOnceValue
    ){
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        left      = new List<Vector2>();
        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        right     = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();
        down      = new List<Vector2>();
        up        = new List<Vector2>();

        bool blockLeft      = false;
        bool blockLeftDown  = false;
        bool blockLeftUp    = false;
        bool blockRight     = false;
        bool blockRightDown = false;
        bool blockRightUp   = false;
        bool blockDown      = false;
        bool blockUp        = false;

        for(int delta = 1; delta <= limit; ++delta) {
            int xLeft  = originInt.x - delta;
            int xRight = originInt.x + delta;
            int yDown  = originInt.y - delta;
            int yUp    = originInt.y + delta;

            Vector2 positionLeft      = new Vector2(xLeft,       originInt.y);
            Vector2 positionLeftDown  = new Vector2(xLeft,       yDown);
            Vector2 positionLeftUp    = new Vector2(xLeft,       yUp);
            Vector2 positionRight     = new Vector2(xRight,      originInt.y);
            Vector2 positionRightDown = new Vector2(xRight,      yDown);
            Vector2 positionRightUp   = new Vector2(xRight,      yUp);
            Vector2 positionDown      = new Vector2(originInt.x, yDown);
            Vector2 positionUp        = new Vector2(originInt.x, yUp);

            int valueLeft      = boardGrid.GetTileValue(positionLeft);
            int valueLeftDown  = boardGrid.GetTileValue(positionLeftDown);
            int valueLeftUp    = boardGrid.GetTileValue(positionLeftUp);
            int valueRight     = boardGrid.GetTileValue(positionRight);
            int valueRightDown = boardGrid.GetTileValue(positionRightDown);
            int valueRightUp   = boardGrid.GetTileValue(positionRightUp);
            int valueDown      = boardGrid.GetTileValue(positionDown);
            int valueUp        = boardGrid.GetTileValue(positionUp);

            AddPosition2Direction(
                valueLeft,
                ref left,
                positionLeft,
                ref blockLeft,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueLeftDown,
                ref leftDown,
                positionLeftDown,
                ref blockLeftDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueLeftUp,
                ref leftUp,
                positionLeftUp,
                ref blockLeftUp,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRight,
                ref right,
                positionRight,
                ref blockRight,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRightDown,
                ref rightDown,
                positionRightDown,
                ref blockRightDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRightUp,
                ref rightUp,
                positionRightUp,
                ref blockRightUp,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueDown,
                ref down,
                positionDown,
                ref blockDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueUp,
                ref up,
                positionUp,
                ref blockUp,
                allowedOnceValue
            );
        }
    }

    public void GetDiagonalPositions(
        Vector2 origin,
        out List<Vector2> leftDown,
        out List<Vector2> leftUp,
        out List<Vector2> rightDown,
        out List<Vector2> rightUp,
        int allowedOnceValue
    ) {
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();

        bool blockLeftDown  = false;
        bool blockLeftUp    = false;
        bool blockRightDown = false;
        bool blockRightUp   = false;

        for(int delta = 1; delta < width; ++delta) {
            bool allBlocked = blockLeftDown
                && blockLeftUp
                && blockRightDown
                && blockRightUp;

            if(allBlocked) { break; }

            int xLeft  = originInt.x - delta;
            int xRight = originInt.x + delta;
            int yDown  = originInt.y - delta;
            int yUp    = originInt.y + delta;

            Vector2 positionLeftDown  = new Vector2(xLeft,  yDown);
            Vector2 positionLeftUp    = new Vector2(xLeft,  yUp);
            Vector2 positionRightDown = new Vector2(xRight, yDown);
            Vector2 positionRightUp   = new Vector2(xRight, yUp);

            int valueLeftDown  = boardGrid.GetTileValue(positionLeftDown);
            int valueLeftUp    = boardGrid.GetTileValue(positionLeftUp);
            int valueRightDown = boardGrid.GetTileValue(positionRightDown);
            int valueRightUp   = boardGrid.GetTileValue(positionRightUp);

            AddPosition2Direction(
                valueLeftDown,
                ref leftDown,
                positionLeftDown,
                ref blockLeftDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueLeftUp,
                ref leftUp,
                positionLeftUp,
                ref blockLeftUp,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRightDown,
                ref rightDown,
                positionRightDown,
                ref blockRightDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRightUp,
                ref rightUp,
                positionRightUp,
                ref blockRightUp,
                allowedOnceValue
            );
        }
    }

    public void GetOrthogonalPositions(
        Vector2 origin,
        out List<Vector2> left,
        out List<Vector2> right,
        out List<Vector2> down,
        out List<Vector2> up,
        int limit,
        int allowedOnceValue = 0
    ) {
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        left  = new List<Vector2>();
        right = new List<Vector2>();
        down  = new List<Vector2>();
        up    = new List<Vector2>();

        bool blockLeft  = false;
        bool blockRight = false;
        bool blockDown  = false;
        bool blockUp    = false;

        for(int delta = 1; delta <= limit; ++delta) {
            int xLeft  = originInt.x - delta;
            int xRight = originInt.x + delta;
            int yDown  = originInt.y - delta;
            int yUp    = originInt.y + delta;

            Vector2 positionLeft  = new Vector2(xLeft,       originInt.y);
            Vector2 positionRight = new Vector2(xRight,      originInt.y);
            Vector2 positionDown  = new Vector2(originInt.x, yDown);
            Vector2 positionUp    = new Vector2(originInt.x, yUp);

            int valueLeft  = boardGrid.GetTileValue(positionLeft);
            int valueRight = boardGrid.GetTileValue(positionRight);
            int valueDown  = boardGrid.GetTileValue(positionDown);
            int valueUp    = boardGrid.GetTileValue(positionUp);

            AddPosition2Direction(
                valueLeft,
                ref left,
                positionLeft,
                ref blockLeft,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueRight,
                ref right,
                positionRight,
                ref blockRight,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueDown,
                ref down,
                positionDown,
                ref blockDown,
                allowedOnceValue
            );

            AddPosition2Direction(
                valueUp,
                ref up,
                positionUp,
                ref blockUp,
                allowedOnceValue
            );
        }
    }

    public void GetForwardPositions(
        Vector3 origin,
        out List<Vector2> forward,
        Vector3Int direction,
        int limit,
        int allowedOnceValue
    ) {
        Vector3Int originInt = Vector3Int.FloorToInt(origin);

        forward = new List<Vector2>();

        bool blocked = false;

        for(int i = 1; i <= limit; ++i) {
            Vector3 positionForward = originInt + direction;

            int tileValue = boardGrid.GetTileValue(positionForward);

            AddPosition2Direction(
                tileValue,
                ref forward,
                positionForward,
                ref blocked,
                allowedOnceValue
            );
        }
    }

    private void AddPosition2Direction(
        int tileValue,
        ref List<Vector2> direction,
        Vector2 position2Add,
        ref bool directionBlocked,
        int nonFreeAllowedValue
    ) {
        bool canAddTile = (! directionBlocked)
            && ((tileValue == Utils.TILE_VALUE_FREE)
                || (tileValue == nonFreeAllowedValue));

        if(canAddTile) {
            direction.Add(position2Add);

            if(tileValue != Utils.TILE_VALUE_FREE) {
                directionBlocked = true;
            }
        }
        else { directionBlocked = true; }
    }

    private void DebugVisualize() {
        Color freeColor = Color.green;
        Color takenColor = Color.red;

        Color lineColor = freeColor;
        for(int x = 0; x < width; ++x) {
            for(int y = 0; y < height; ++y) {

                int tileData = boardGrid.GetTileValue(x, y);
                Vector2 debugDataTextPosition = new Vector2(x, y)
                    + (Vector2.one * tileSize / 2);

                boardGridDebug[x, y] = Utils.CreateWorldText(
                    transform,
                    tileData.ToString(),
                    debugDataTextPosition,
                    debugTextSize,
                    Color.white,
                    "Terrain",
                    1
                );

                if(tileData > 0) { lineColor = takenColor; }
                else             { lineColor = freeColor;  }

                Vector3 bottomHorizontalA = new Vector3(x, y, 0f);
                Vector3 bottomHorizontalB = new Vector3(x + tileSize, y, 0f);
                Debug.DrawLine(
                    bottomHorizontalA,
                    bottomHorizontalB,
                    lineColor,
                    100f
                );

                Vector3 leftVerticalA     = new Vector3(x, y,0f);
                Vector3 leftVerticalB     = new Vector3(x, y + tileSize, 0f);
                Debug.DrawLine(leftVerticalA, leftVerticalB, lineColor, 100f);

                Vector3 topHorizontalA    = new Vector3(x, y + tileSize, 0f);
                Vector3 topHorizontalB    = new Vector3(
                    x + tileSize,
                    y + tileSize, 0f
                );
                Debug.DrawLine(topHorizontalA, topHorizontalB, lineColor, 100f);

                Vector3 rightVerticalA    = new Vector3(x + tileSize, y, 0f);
                Vector3 rightVerticalB    = new Vector3(
                    x + tileSize,
                    y+tileSize,
                    0f
                );
                Debug.DrawLine(rightVerticalA, rightVerticalB, lineColor, 100f);
            }
        }
    }

    private void CreateGridMesh() {
        Mesh gridMesh = new Mesh();

        gridMesh.name = "GridMesh";

        gridMesh.Clear();

        int rectanglesCount = width + height - 2;
        int verticesCount   = rectanglesCount * 4;

        gridMesh.vertices  = GenerateGridMeshVertices(verticesCount);
        gridMesh.triangles = GenerateGridMeshTriangles(rectanglesCount);

        Color[] vertexColors = new Color[verticesCount];
        for (int vertex = 0; vertex < verticesCount; ++vertex)
            vertexColors[vertex] = Color.white;

        gridMesh.colors = vertexColors;

        gridMesh.RecalculateNormals();

        gridMeshFilter.mesh = gridMesh;
    }

    private Vector3[] GenerateGridMeshVertices(int verticesCount)
    {
        Vector3[] vertices = new Vector3[verticesCount];

        // Vertical Lines
        int vertexIndex = 0;
        for (int x = 1; x < width; ++x)
        {
            Vector3[] lineVertices = Utils.GetRectangleVertices(
                new Vector2(x, 1),
                new Vector2(x + gridLineWidth, height - 1)
            );

            for (int i = 0; i < lineVertices.Length; ++i)
            {
                vertexIndex = (x - 1) * 4 + i;
                vertices[vertexIndex] = lineVertices[i];
            }
        }
        vertexIndex++;

        // Horizontal Lines
        for (int y = 1; y < height; ++y)
        {
            Vector3[] lineVertices = Utils.GetRectangleVertices(
                new Vector2(1, y),
                new Vector2(width - 1 + gridLineWidth, y + gridLineWidth)
            );

            for (int i = 0; i < lineVertices.Length; ++i)
                vertices[vertexIndex + (y - 1) * 4 + i] = lineVertices[i];
        }

        return vertices;
    }

    private int[] GenerateGridMeshTriangles(int rectanglesCount)
    {
        int trianglesCount  = rectanglesCount * 6;
        int[] triangles = new int[trianglesCount * 2];

        int triangleVertexIndex = 0;
        for (int rectangle = 0; rectangle < rectanglesCount; ++rectangle)
        {
            triangles[triangleVertexIndex++] =  rectangle * 4;
            triangles[triangleVertexIndex++] = (rectangle * 4) + 2;
            triangles[triangleVertexIndex++] = (rectangle * 4) + 1;

            triangles[triangleVertexIndex++] = (rectangle * 4) + 2;
            triangles[triangleVertexIndex++] = (rectangle * 4) + 3;
            triangles[triangleVertexIndex++] = (rectangle * 4) + 1;
        }

        return triangles;
    }
}
