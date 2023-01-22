using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerUnit : Unit {

    // private PlayerController owner;

    // public override void MoveTo(Vector2 worldPosition) {
    //     Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
    //     movement.Move(worldPositionInt);

    //     boardGrid.SetTile(worldPosition, Utils.TILE_VALUE_ENEMY);
    // }

    // public override void Select() {
    //     if(unitCanvas != null) { unitCanvas.enabled = true; }

    //     if((movement != null) && (! movement.hasMoved)) {
    //         movement.GetAvailablePositions();

    //         if(movement.HasMovementOptions()) {
    //             movement.CreateIndicatorMesh();
    //             movement.ShowIndicator();
    //         }
    //         else {
    //             movement.hasMoved = true;
    //         }

    //     }
    //     else if((attack != null) && (! attack.hasAttacked)) {
    //         attack.GetAvailablePositions();

    //         if(attack.HasAttackOptions()) {
    //             attack.CreateIndicatorMesh();
    //             attack.ShowIndicator();
    //         }
    //         else {
    //             attack.hasAttacked = true;
    //         }
    //     }
    // }

    // private void OnDestroy() { owner.RemoveUnit(this); }
}
