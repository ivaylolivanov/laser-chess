using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyUnitTypes { DRONE, DREADNOUGHT, COMMAND }

public class EnemyUnit : Unit {
    [SerializeField] private EnemyUnitTypes unitType;

    private EnemyController owner;

    void Awake() {
        base.Awake();

        owner = FindObjectOfType<EnemyController>();
    }

    void Start() {
        base.Start();

        owner.AddUnit(this, unitType);
    }

    void OnDestroy() { owner.RemoveUnit(this, unitType); }

    public override void MoveTo(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        movement.Move(worldPositionInt);

        boardGrid.SetTile(worldPosition, Utils.TILE_VALUE_ENEMY);
    }
}
