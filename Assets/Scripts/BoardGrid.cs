using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardGrid : MonoBehaviour {
    [Header("Grid")]
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float tileSize;
    [SerializeField] private LayerMask obstaclesMask;

    [Space] [Header("Debug")]
    [SerializeField] private bool debug;
    [SerializeField] private int  debugTextSize;

    private Grid boardGrid;
    private TextMesh[, ] boardGridDebug;

    void Awake() {
        boardGrid = new Grid(width, height, tileSize, obstaclesMask);

        if(debug) {
            boardGridDebug = new TextMesh[width, height];
            DebugVisualize();
        }
    }

    public List<Vector2> GetFreeLeftPositions(Vector2 origin) {
        List<Vector2> result = new List<Vector2>();
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        for(int x = originInt.x - 1; x >= 0; --x) {
            Vector2 position = new Vector2(x, originInt.y);
            int tileValue = boardGrid.GetTileValue(position);

            if(tileValue > 0) { break; }

            result.Add(position);
        }

        return result;
    }

    public List<Vector2> GetFreeRightPositions(Vector2 origin) {
        List<Vector2> result = new List<Vector2>();
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        for(int x = originInt.x + 1; x < width; ++x) {
            Vector2 position = new Vector2(x, originInt.y);
            int tileValue = boardGrid.GetTileValue(position);

            if(tileValue > 0) { break; }

            result.Add(position);
        }

        return result;
    }

    public List<Vector2> GetFreeDownPositions(Vector2 origin) {
        List<Vector2> result = new List<Vector2>();
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        for(int y = originInt.y - 1; y >= 0; --y) {
            Vector2 position = new Vector2(originInt.x, y);
            int tileValue = boardGrid.GetTileValue(position);

            if(tileValue > 0) { break; }

            result.Add(position);
        }

        return result;
    }

    public List<Vector2> GetFreeUpPositions(Vector2 origin) {
        List<Vector2> result = new List<Vector2>();
        Vector2Int originInt = Vector2Int.FloorToInt(origin);

        for(int y = originInt.y + 1; y < height; ++y) {
            Vector2 position = new Vector2(originInt.x, y);
            int tileValue = boardGrid.GetTileValue(position);

            if(tileValue > 0) { break; }

            result.Add(position);
        }

        return result;
    }

    public void UpdateTile(Vector3 position) {
        Vector3Int positionInt = Vector3Int.FloorToInt(position);
        boardGrid.UpdateTile(positionInt);

        int newValue = boardGrid.GetTileValue(positionInt);
        boardGridDebug[positionInt.x, positionInt.y].text = newValue.ToString();
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

    public void GetFreeQueenPositions(
        Vector2 origin,
        out List<Vector2> left,
        out List<Vector2> leftDown,
        out List<Vector2> leftUp,
        out List<Vector2> right,
        out List<Vector2> rightDown,
        out List<Vector2> rightUp,
        out List<Vector2> down,
        out List<Vector2> up,
        int limit
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

            if((valueLeft == 0) && (! blockLeft)) {
                left.Add(positionLeft);
            }
            else { blockLeft = true; }

            if((valueLeftDown == 0) && (! blockLeftDown)) {
                leftDown.Add(positionLeftDown);
            }
            else { blockLeftDown = true; }

            if((valueLeftUp == 0) && (! blockLeftUp)) {
                leftUp.Add(positionLeftUp);
            }
            else { blockLeftUp = true; }

            if((valueRight == 0) && ((! blockRight))) {
                right.Add(positionRight);
            }
            else { blockRight = true; }

            if((valueRightDown == 0) && (! blockRightDown)) {
                rightDown.Add(positionRightDown);
            }
            else { blockRightDown = true; }

            if((valueRightUp == 0) && (! blockRightUp)) {
                rightUp.Add(positionRightUp);
            }
            else { blockRightUp = true; }

            if((valueDown == 0) && (! blockDown)) {
                down.Add(positionDown);
            }
            else { blockDown = true; }

            if((valueUp == 0) && (! blockUp)) {
                up.Add(positionUp);
            }
            else { blockUp = true; }
        }
    }

    public void GetFreeDiagonalPositions(
        Vector2 origin,
        out List<Vector2> leftDown,
        out List<Vector2> leftUp,
        out List<Vector2> rightDown,
        out List<Vector2> rightUp
    ){
        Vector2Int originInt = Vector2Int.FloorToInt(origin);
        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();

        for(int delta = 1; delta < width; ++delta) {
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

            if(valueLeftDown  == 0) { leftDown.Add(positionLeftDown);   }
            if(valueLeftUp    == 0) { leftUp.Add(positionLeftUp);       }
            if(valueRightDown == 0) { rightDown.Add(positionRightDown); }
            if(valueRightUp   == 0) { rightUp.Add(positionRightUp);     }
        }
    }

    public void GetFreeOrthogonalPositions(
        Vector2 origin,
        out List<Vector2> left,
        out List<Vector2> right,
        out List<Vector2> down,
        out List<Vector2> up,
        int limit
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

            if((valueLeft == 0) && (! blockLeft)) { left.Add(positionLeft); }
            else { blockLeft = true; }

            if((valueRight == 0) && (! blockRight)) { right.Add(positionRight); }
            else { blockRight = true; }

            if((valueDown == 0) && (! blockDown)) { down.Add(positionDown); }
            else { blockDown = true; }

            if((valueUp == 0) && (! blockUp)) { up.Add(positionUp); }
            else { blockUp = true; }
        }
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
}
