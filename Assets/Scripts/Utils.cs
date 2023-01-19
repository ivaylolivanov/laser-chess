using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {

    #region Constants

    // Board
    public const int    BOARD_WIDTH  = 8;
    public const int    BOARD_HEIGHT = 8;

    // Unit movement
    public const int    KNIGHT_POSSIBLE_POSITIONS = 8;
    public const int    MAX_STEPS = 7;

    // Sorting layers
    public const string UNITS_SORTING_LAYER = "Units";
    public const string INDICATOR_SORTING_LAYER = "Indicator";

    // Tiles
    public const int TILE_VALUE_FREE   = 0;
    public const int TILE_VALUE_PLAYER = 8;
    public const int TILE_VALUE_ENEMY  = 9;
    public const int TILE_VALUE_WALL   = 10;

    public const string HIGHEST_UNLOCKED_LEVEL = "HighestUnlockedLevel";

    #endregion

    public static Vector3[] GetRectangleVertices(
        Vector2 leftBottom,
        Vector2 rightTop
    ) {
        Vector2 rightBottom = new Vector2(rightTop.x,   leftBottom.y);
        Vector2 leftTop     = new Vector2(leftBottom.x, rightTop.y);

        Vector3[] result = new Vector3[] {
            leftBottom,
            rightBottom,
            leftTop,
            rightTop
        };

        return result;
    }

    public static TextMesh CreateWorldText(
        Transform parent,
        string text,
        Vector3 localPosition,
        int fontSize,
        Color color,
        string sortingLayerName = "Default",
        int sortingOrder = 0,
        TextAnchor textAnchor = TextAnchor.MiddleCenter,
        TextAlignment textAlignment = TextAlignment.Center
    ) {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;
        transform.SetParent(parent, false);
        transform.localPosition = localPosition;
        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;

        MeshRenderer textRenderer = textMesh.GetComponent<MeshRenderer>();
        textRenderer.sortingLayerName = sortingLayerName;
        textRenderer.sortingOrder = sortingOrder;

        return textMesh;
    }

    public static bool CanMoveInDirection(List<Vector2> direction, Vector2 position) {
        Vector2Int positionInt = Vector2Int.FloorToInt(position);
        return CanMoveInDirection(direction, positionInt);
    }

    public static bool CanMoveInDirection(
        List<Vector2> direction,
        Vector2Int position
    ) {
        bool result = (direction != null)
           && (direction.Count > 0)
           && direction.Contains(position);

        return result;
    }

    public static void ArrayRightTrim(ref List<Vector2> arr, int fromIndex) {
        if(arr == null || arr.Count < fromIndex) { return; }

        int count = Mathf.Max(0, arr.Count - 1 - fromIndex);
        arr.RemoveRange(fromIndex, count);
    }

    public static void ShowObject(Transform transform)
        => transform.localScale = Vector3.one;

    public static void HideObject(Transform transform)
        => transform.localScale = Vector3.zero;

    public static bool IsObjectShown(Transform transform) {
        bool result = transform.localScale == Vector3.one;

        return result;
    }
}
