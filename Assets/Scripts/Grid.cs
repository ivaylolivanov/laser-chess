using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
    private int       width;
    private int       height;
    private float     tileSize;
    private LayerMask obstaclesMask;
    private float     updateTileAngel;

    private float  tileHalfSize;
    private int[,] grid;

    public Grid(
        int       width,
        int       height,
        float     tileSize,
        LayerMask obstaclesMask,
        float     updateTileAngel = 0f
    ) {
        this.width           = width;
        this.height          = height;
        this.tileSize        = tileSize;
        this.obstaclesMask   = obstaclesMask;
        this.updateTileAngel = updateTileAngel;

        tileHalfSize = tileSize / 2;
        grid = new int [width, height];

        Initialize();
    }

    public int GetTileValue(int x, int y) {
        int result = int.MaxValue;

        if(AreCoordinatesValid(x, y)) { result = grid[x, y]; }

        return result;
    }

    public int GetTileValue(Vector3Int position) {
        return GetTileValue(position.x, position.y);
    }

    public int GetTileValue(Vector3 position) {
        return GetTileValue(Vector3Int.FloorToInt(position));
    }

    public int GetTileValueLoose(int x, int y) {
        x = Mathf.Clamp(x, 0, width  - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return grid[x, y];
    }

    public int GetTileValueLoose(Vector3Int position) {
        return GetTileValueLoose(position.x, position.y);
    }

    public int GetTileValueLoose(Vector3 position) {
        return GetTileValueLoose(Vector3Int.FloorToInt(position));
    }

    public void UpdateTile(int x, int y) {
        if(! AreCoordinatesValid(x, y)) { return; }

        Vector2 offset = Vector2.one * tileHalfSize;
        Vector2 center = new Vector2(x, y) + offset;

        Collider2D[] colliders = Physics2D.OverlapBoxAll(
            center,
            offset,
            updateTileAngel,
            obstaclesMask
        );

        if(colliders.Length > 0) {
            grid[x, y] = colliders[0].gameObject.layer;
        }
        else {
            grid[x, y] = 0;
        }
    }

    public void UpdateTile(Vector3Int position) {
        UpdateTile(position.x, position.y);
    }

    public void UpdateTile(Vector3 position) {
        UpdateTile(Vector3Int.FloorToInt(position));
    }

    public void SetTile(int x, int y, int value) {
        if(! AreCoordinatesValid(x, y)) { return; }

        grid[x, y] = value;
    }

    public void SetTile(Vector3Int position, int value) {
        SetTile(position.x, position.y, value);
    }

    public void SetTile(Vector3 position, int value) {
        SetTile(Vector3Int.FloorToInt(position), value);
    }

    private bool AreCoordinatesValid(int x, int y) {
        bool isXValid = (x >= 0) && (x < width);
        bool isYValid = (y >= 0) && (y < height);
        bool result = isXValid && isYValid;

        return result;
    }

    private void Initialize() {
        for(int x = 0; x < width; ++x) {
            for(int y = 0; y < height; ++y) { UpdateTile(x, y); }
        }
    }
}
