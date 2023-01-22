using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerUnitTypes { GRUNT, JUMPSHIP, TANK }

public class PlayerUnit : Unit {
    [SerializeField] private PlayerUnitTypes unitType;

    private PlayerController owner;

    void Awake() {
        base.Awake();

        owner = FindObjectOfType<PlayerController>();
    }

    void Start() {
        base.Start();

        owner.AddUnit(this);
    }

    public override void MoveTo(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        movement.Move(worldPositionInt);

        boardGrid.SetTile(worldPosition, Utils.TILE_VALUE_ENEMY);
    }

    public override void Select() {
        if(unitCanvas != null) { unitCanvas.enabled = true; }

        if((movement != null) && (! movement.hasMoved)) {
            movement.GetAvailablePositions();

            if(movement.HasMovementOptions()) {
                movement.CreateIndicatorMesh();
                movement.ShowIndicator();
            }
            else {
                movement.hasMoved = true;
            }

        }
        else if((attack != null) && (! attack.hasAttacked)) {
            attack.GetAvailablePositions();

            if(attack.HasAttackOptions()) {
                attack.CreateIndicatorMesh();
                attack.ShowIndicator();
            }
            else {
                attack.hasAttacked = true;
            }
        }
    }

    public PlayerUnitTypes GetUnitType() => unitType;

    private void OnDestroy() { owner.RemoveUnit(this); }
}
