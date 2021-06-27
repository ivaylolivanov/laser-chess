using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {
    private BoardGrid boardGrid;
    private TurnsSystem turnsSystem;

    private List<EnemyUnit> droneUnits;
    private List<EnemyUnit> dreadnoughtUnits;
    private List<EnemyUnit> commandUnits;

    void Awake() {
        boardGrid = FindObjectOfType<BoardGrid>();
        turnsSystem = FindObjectOfType<TurnsSystem>();

        droneUnits = new List<EnemyUnit>();
        dreadnoughtUnits = new List<EnemyUnit>();
        commandUnits = new List<EnemyUnit>();
    }

    void Update() {
        if(commandUnits.Count <= 0 ) {
            turnsSystem.Won();
            return;
        }

        if(turnsSystem.currentTurn != Turns.ENEMY_TURN) { return; }

        DroneOrders();
        DreadnoughtOrders();
        CommandOrders();

        turnsSystem.PlayerTurn();
    }

    public void AddUnit(EnemyUnit newUnit, EnemyUnitTypes unitType) {
        switch(unitType) {
        case EnemyUnitTypes.DRONE:
            droneUnits.Add(newUnit);
            break;

        case EnemyUnitTypes.DREADNOUGHT:
            dreadnoughtUnits.Add(newUnit);
            break;

        case EnemyUnitTypes.COMMAND:
            commandUnits.Add(newUnit);
            break;
        }
    }

    public void RemoveUnit(EnemyUnit unit, EnemyUnitTypes unitType) {
        switch(unitType) {
        case EnemyUnitTypes.DRONE:
            droneUnits.Remove(unit);
            break;

        case EnemyUnitTypes.DREADNOUGHT:
            dreadnoughtUnits.Remove(unit);
            break;

        case EnemyUnitTypes.COMMAND:
            commandUnits.Remove(unit);
            break;
        }

        boardGrid.UpdateTile(unit.transform.position);
    }

    public void ResetUnits() {
        ResetUnitsGroup(droneUnits);
        ResetUnitsGroup(dreadnoughtUnits);
        ResetUnitsGroup(commandUnits);
    }

    private void DroneOrders() {
        foreach(EnemyUnit drone in droneUnits) {
            if(! drone.HasOptions()) { continue; }

            Vector2Int currentDronePosition = Vector2Int.FloorToInt(
                drone.transform.position
            );
            Vector2Int nextDronePosition = currentDronePosition
                - new Vector2Int(0, 1);

            if(drone.CanMoveTo(nextDronePosition)) {
                drone.MoveTo(nextDronePosition);
            }
            else {
                drone.SetHasMoved(true);
            }

            List<Vector2> allDiagonals = new List<Vector2>();
            boardGrid.GetDiagonalPositions(
                nextDronePosition,
                out allDiagonals,
                out allDiagonals,
                out allDiagonals,
                out allDiagonals,
                Utils.TILE_VALUE_PLAYER
            );

            foreach(Vector2 position in allDiagonals) {
                int tileValue = boardGrid.GetTileValue(position);
                if(tileValue != Utils.TILE_VALUE_PLAYER) { continue; }

                drone.Attack(position);
                break;
            }
            drone.SetHasAttacked(true);

            if(nextDronePosition.y == 1) {
                turnsSystem.Lost();
                return;
            }
        }
    }

    private void DreadnoughtOrders() {
        foreach(EnemyUnit unit in dreadnoughtUnits) {
            if(! unit.HasOptions()) { continue; }

            Collider2D[] allColliders = Physics2D.OverlapAreaAll(
                new Vector2(1,1),
                new Vector2(9,9)
            );

            float closestSqrDistance = float.MaxValue;
            PlayerUnit closestTarget = null;

            foreach(Collider2D collider in allColliders) {
                PlayerUnit playerUnit = collider.GetComponent<PlayerUnit>();
                if(playerUnit == null) { continue; }

                float sqrDistance = (
                    playerUnit.transform.position
                    - unit.transform.position
                ).sqrMagnitude;

                if(sqrDistance < closestSqrDistance) {
                    closestSqrDistance = sqrDistance;
                    closestTarget = playerUnit;
                }
            }

            if(closestTarget == null) { continue; }

            Vector2 newPosition = unit.GetPositionClosestTo(
                closestTarget.transform.position
            );

            if(unit.CanMoveTo(newPosition)) {
                unit.MoveTo(newPosition);
            }
            else {
                unit.SetHasMoved(true);
            }

            if(unit.CanAttackOn(closestTarget.transform.position)) {
                unit.Attack(newPosition);
            }
            else {
                unit.SetHasAttacked(true);
            }
        }
    }

    private void CommandOrders() {
        foreach(EnemyUnit unit in commandUnits) {
            if(! unit.HasOptions()) { continue; }

            Vector2 current = (Vector2) unit.transform.position;
            Vector2 left  = current + Vector2.left;
            Vector2 right = current + Vector2.right;

            int[] minHits = new int[3];
            Vector2[] positionsToGo = new Vector2[3];

            minHits[0] = 0;
            minHits[1] = 0;
            minHits[2] = 0;

            positionsToGo[0] = left;
            positionsToGo[1] = current;
            positionsToGo[2] = right;

            List<Vector2> attackPoints = new List<Vector2>();

            Collider2D[] allColliders = Physics2D.OverlapAreaAll(
                new Vector2(1,1),
                new Vector2(9,9)
            );

            foreach(Collider2D collider in allColliders) {
                PlayerUnit playerUnit = collider.GetComponent<PlayerUnit>();
                if(playerUnit == null) { continue; }
                if(! playerUnit.HasOptions()) { continue; }

                List<Vector2> nextPossiblePositions
                    = playerUnit.GetMovementPositions();

                PlayerUnitTypes playerUnitType = playerUnit.GetUnitType();
                switch(playerUnitType) {
                case PlayerUnitTypes.GRUNT:
                    foreach(Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDown  = new List<Vector2>();
                        List<Vector2> leftUp    = new List<Vector2>();
                        List<Vector2> rightDown = new List<Vector2>();
                        List<Vector2> rightUp   = new List<Vector2>();

                        boardGrid.GetDiagonalPositions(
                            position,
                            out leftDown,
                            out leftUp,
                            out rightDown,
                            out rightUp,
                            Utils.TILE_VALUE_ENEMY
                        );

                        attackPoints.AddRange(leftDown);
                        attackPoints.AddRange(leftUp);
                        attackPoints.AddRange(rightDown);
                        attackPoints.AddRange(rightUp);
                    }

                    break;

                case PlayerUnitTypes.JUMPSHIP:
                    foreach(Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDir  = new List<Vector2>();
                        List<Vector2> rightDir = new List<Vector2>();
                        List<Vector2> down  = new List<Vector2>();
                        List<Vector2> up    = new List<Vector2>();

                        boardGrid.GetOrthogonalPositions(
                            position,
                            out leftDir,
                            out rightDir,
                            out down,
                            out up,
                            1,
                            Utils.TILE_VALUE_ENEMY
                        );

                        attackPoints.AddRange(leftDir);
                        attackPoints.AddRange(rightDir);
                        attackPoints.AddRange(down);
                        attackPoints.AddRange(up);
                    }

                    break;

                case PlayerUnitTypes.TANK:
                    foreach(Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDir  = new List<Vector2>();
                        List<Vector2> rightDir = new List<Vector2>();
                        List<Vector2> down  = new List<Vector2>();
                        List<Vector2> up    = new List<Vector2>();
                        boardGrid.GetOrthogonalPositions(
                            position,
                            out leftDir,
                            out rightDir,
                            out down,
                            out up,
                            Utils.MAX_STEPS,
                            Utils.TILE_VALUE_ENEMY
                        );

                        attackPoints.AddRange(leftDir);
                        attackPoints.AddRange(rightDir);
                        attackPoints.AddRange(down);
                        attackPoints.AddRange(up);
                    }

                    break;
                }
            }

            foreach(Vector2 point in attackPoints) {
                if(point == left) { minHits[0]++; }
                if(point == current) { minHits[1]++; }
                if(point == right) { minHits[2]++; }
            }

            int min1 = Mathf.Min(minHits[0], minHits[1]);
            int min = Mathf.Min(min1, minHits[2]);

            int safestPositionIndex = 1;

            for(int i = 0; i < 3; ++i) {
                if(minHits[i] == min) {
                    safestPositionIndex = i;
                    break;
                }
            }

            if(unit.CanMoveTo(positionsToGo[safestPositionIndex])) {
                unit.MoveTo(positionsToGo[safestPositionIndex]);
            }
        }
    }

    private void ResetUnitsGroup(List<EnemyUnit> unitsGroup) {
        foreach(EnemyUnit unit in unitsGroup) { unit.Reset(); }
    }
}
