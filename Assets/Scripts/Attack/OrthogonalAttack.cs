using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthogonalAttack : Attack {
    protected List<Vector2> left;
    protected List<Vector2> right;
    protected List<Vector2> down;
    protected List<Vector2> up;

    void Awake() {
        base.Awake();

        left  = new List<Vector2>();
        right = new List<Vector2>();
        down  = new List<Vector2>();
        up    = new List<Vector2>();
    }

    public override void GetAvailablePositions() {
        boardGrid.GetOrthogonalPositions(
            transform.position,
            out left,
            out right,
            out down,
            out up,
            stats.attackLimit,
            targetTileValue
        );

        if(allDirections != null) { allDirections.Clear(); }

        allDirections.UnionWith(left);
        allDirections.UnionWith(right);
        allDirections.UnionWith(down);
        allDirections.UnionWith(up);
    }
}
