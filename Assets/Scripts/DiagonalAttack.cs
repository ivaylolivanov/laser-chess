using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiagonalAttack : Attack {
    private List<Vector2> leftDown;
    private List<Vector2> leftUp;
    private List<Vector2> rightDown;
    private List<Vector2> rightUp;

    private void Awake() {
        base.Awake();

        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();
    }

    public override void GetAvailablePositions() {
        boardGrid.GetDiagonalPositions(
            transform.position,
            out leftDown,
            out leftUp,
            out rightDown,
            out rightUp,
            targetTileValue
        );

        if(allDirections != null) { allDirections.Clear(); }

        allDirections.UnionWith(leftDown);
        allDirections.UnionWith(leftUp);
        allDirections.UnionWith(rightDown);
        allDirections.UnionWith(rightUp);
    }
}
