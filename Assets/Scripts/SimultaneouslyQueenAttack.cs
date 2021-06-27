using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneouslyQueenAttack : Attack {
    private List<Vector2> left;
    private List<Vector2> leftDown;
    private List<Vector2> leftUp;
    private List<Vector2> right;
    private List<Vector2> rightDown;
    private List<Vector2> rightUp;
    private List<Vector2> down;
    private List<Vector2> up;

    void Awake() {
        base.Awake();

        left      = new List<Vector2>();
        leftDown  = new List<Vector2>();
        leftUp    = new List<Vector2>();
        right     = new List<Vector2>();
        rightDown = new List<Vector2>();
        rightUp   = new List<Vector2>();
        down      = new List<Vector2>();
        up        = new List<Vector2>();
    }

    public override void GetAvailablePositions() {
        boardGrid.GetQueenPositions(
            transform.position,
            out left,
            out leftDown,
            out leftUp,
            out right,
            out rightDown,
            out rightUp,
            out down,
            out up,
            stats.stepsLimit,
            targetTileValue
        );

        if(allDirections != null) { allDirections.Clear(); }

        allDirections.UnionWith(left);
        allDirections.UnionWith(leftDown);
        allDirections.UnionWith(leftUp);
        allDirections.UnionWith(right);
        allDirections.UnionWith(rightDown);
        allDirections.UnionWith(rightUp);
        allDirections.UnionWith(down);
        allDirections.UnionWith(up);
    }

    public override void Execute(Vector2 destination) {
        foreach(Vector2 position in allDirections) {
            int tileValue  = boardGrid.GetTileValue(position);

            bool tileIsEnemy  = tileValue == Utils.TILE_VALUE_ENEMY;
            bool tileIsPlayer = tileValue == Utils.TILE_VALUE_PLAYER;
            if((! tileIsPlayer) || (! tileIsPlayer) ) { continue; }

            Collider2D enemyCollider = Physics2D.OverlapCircle(
                position + (Vector2.one * 0.5f),
                0.1f
                );

            Unit enemyUnit = enemyCollider.GetComponent<Unit>();
            if(enemyUnit == null) { continue; }

            GameObject explosion = Instantiate(
                explosionPrefab,
                position,
                Quaternion.identity
                );

            enemyUnit.TakeDamage(stats.attackPower);

            Destroy(explosion, 2f);
        }

        hasAttacked = true;
    }
}
