using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneouslyOrthogonalAttack : OrthogonalAttack {
    void Awake() { base.Awake(); }

    public override void Execute(Vector2 destination) {
        foreach(Vector2 position in allDirections) {
            int tileValue  = boardGrid.GetTileValue(position);

            bool tileIsEnemy  = tileValue == Utils.TILE_VALUE_ENEMY;
            bool tileIsPlayer = tileValue == Utils.TILE_VALUE_PLAYER;
            if((! tileIsPlayer) && (! tileIsEnemy)) { continue; }

            Collider2D targetCollider = Physics2D.OverlapCircle(
                position + (Vector2.one * 0.5f),
                0.1f
            );

            Unit targetUnit = targetCollider.GetComponent<Unit>();
            if(targetUnit == null) { continue; }

            GameObject explosion = Instantiate(
                explosionPrefab,
                position,
                Quaternion.identity
            );

            targetUnit.TakeDamage(stats.attackPower);

            Destroy(explosion, 2f);
        }

        hasAttacked = true;
    }
}
