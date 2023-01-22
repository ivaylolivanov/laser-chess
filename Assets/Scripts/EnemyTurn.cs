using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurn : ITurn {
    private List<Unit> drones;
    private List<Unit> dreadnoughts;
    private List<Unit> commanders;

    private Turn nextTurn;
    private MonoBehaviour caller;

    private bool allDronesOrdered       = false;
    private bool allDreadnoughtsOrdered = false;
    private bool allCommandersOrdered   = false;

    private float actionsInterval = 2f;
    private float nextAction = 0f;

    public EnemyTurn(List<Unit> enemyUnits, MonoBehaviour turnsSystem) {
        SeparateUnits(enemyUnits);

        caller = turnsSystem;

        nextTurn = Turn.ENEMY;
        nextAction = 0f;
    }

    public Turn Actions() {
        if (commanders.Count <= 0) return Turn.WON;

        caller.StartCoroutine(OrderDrones());

        if (allDronesOrdered)
            caller.StartCoroutine(OrderDreadnoughts());

        if (allDronesOrdered && allDreadnoughtsOrdered)
            caller.StartCoroutine(OrderCommanders());

        if (nextTurn == Turn.LOST || nextTurn == Turn.WON) return nextTurn;

        bool allUnitsOrdered = allDronesOrdered
            && allDreadnoughtsOrdered
            && allCommandersOrdered;

        nextTurn = Turn.ENEMY;
        if (allUnitsOrdered) {
            nextTurn = Turn.PLAYER;

            ResetUnitsActions();
        }

        return nextTurn;
    }

    private IEnumerator OrderDrones()
    {
        foreach (Unit drone in drones)
        {
            Vector2Int currentDronePosition = Vector2Int.FloorToInt(
                drone.transform.position
            );
            Vector2Int nextDronePosition = currentDronePosition
                - new Vector2Int(0, 1);

            if (drone.CanMoveTo(nextDronePosition))
                drone.MoveTo(nextDronePosition);
            else
                drone.SetHasMoved(true);

            List<Vector2> allDiagonals = new List<Vector2>();
            // boardGrid.GetDiagonalPositions(
            //     nextDronePosition,
            //     out allDiagonals,
            //     out allDiagonals,
            //     out allDiagonals,
            //     out allDiagonals,
            //     Utils.TILE_VALUE_PLAYER
            // );

            // foreach (Vector2 position in allDiagonals) {
            //     int tileValue = boardGrid.GetTileValue(position);
            //     if(tileValue != Utils.TILE_VALUE_PLAYER) continue;

            //     drone.Attack(position);
            //     break;
            // }
            drone.SetHasAttacked(true);

            if (nextDronePosition.y == 1)
            {
                Debug.Log($"Drone won");
                nextTurn = Turn.LOST;
                break;
            }

            yield return new WaitForSeconds(actionsInterval);
        }

        allDronesOrdered = true;
    }

    private IEnumerator OrderDreadnoughts() {
        foreach(Unit unit in dreadnoughts) {
            // if (! unit.HasOptions()) continue;

            Collider2D[] allColliders = Physics2D.OverlapAreaAll(
                new Vector2(1,1),
                new Vector2(9,9)
            );

            float closestSqrDistance = float.MaxValue;
            PlayerUnit closestTarget = null;

            foreach(Collider2D collider in allColliders) {
                PlayerUnit playerUnit = collider.GetComponent<PlayerUnit>();
                if (playerUnit == null) continue;

                float sqrDistance = (
                    playerUnit.transform.position
                    - unit.transform.position
                ).sqrMagnitude;

                if (sqrDistance < closestSqrDistance) {
                    closestSqrDistance = sqrDistance;
                    closestTarget = playerUnit;
                }
            }

            if (closestTarget == null) continue;

            Vector2 newPosition = unit.GetPositionClosestTo(
                closestTarget.transform.position
            );

            if (unit.CanMoveTo(newPosition))
                unit.MoveTo(newPosition);
            else
                unit.SetHasMoved(true);

            if (unit.CanAttackOn(closestTarget.transform.position))
                unit.Attack(newPosition);
            else
                unit.SetHasAttacked(true);

            yield return new WaitForSeconds(actionsInterval);
        }

        allDreadnoughtsOrdered = true;
    }

    private IEnumerator OrderCommanders() {
        foreach (Unit unit in commanders) {
            // if (! unit.HasOptions()) continue;

            Vector2 current = (Vector2) unit.transform.position;
            Vector2 left  = current + Vector2.left;
            Vector2 right = current + Vector2.right;

            int[] minHits = new int[] { 0, 0, 0 };
            Vector2[] positionsToGo = new Vector2[] {
                left,
                current,
                right
            };

            List<Vector2> attackPoints = new List<Vector2>();

            Collider2D[] allColliders = Physics2D.OverlapAreaAll(
                new Vector2(1,1),
                new Vector2(9,9)
            );

            foreach (Collider2D collider in allColliders) {
                Unit playerUnit = collider.GetComponent<Unit>();
                if (playerUnit == null)        continue;

                bool IsPlayerUnit = (playerUnit.GetUnitType()
                                     & UnitType.IS_PLAYER_UNIT) != 0;
                if (IsPlayerUnit)              continue;
                if (! playerUnit.HasOptions()) continue;

                List<Vector2> nextPossiblePositions
                    = playerUnit.GetMovementPositions();

                UnitType unitType = playerUnit.GetUnitType();
                switch (unitType) {
                case UnitType.GRUNT:
                    foreach (Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDown  = new List<Vector2>();
                        List<Vector2> leftUp    = new List<Vector2>();
                        List<Vector2> rightDown = new List<Vector2>();
                        List<Vector2> rightUp   = new List<Vector2>();

                        // boardGrid.GetDiagonalPositions(
                        //     position,
                        //     out leftDown,
                        //     out leftUp,
                        //     out rightDown,
                        //     out rightUp,
                        //     Utils.TILE_VALUE_ENEMY
                        // );

                        attackPoints.AddRange(leftDown);
                        attackPoints.AddRange(leftUp);
                        attackPoints.AddRange(rightDown);
                        attackPoints.AddRange(rightUp);
                    }

                    break;

                case UnitType.JUMPSHIP:
                    foreach(Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDir  = new List<Vector2>();
                        List<Vector2> rightDir = new List<Vector2>();
                        List<Vector2> down  = new List<Vector2>();
                        List<Vector2> up    = new List<Vector2>();

                        // boardGrid.GetOrthogonalPositions(
                        //     position,
                        //     out leftDir,
                        //     out rightDir,
                        //     out down,
                        //     out up,
                        //     1,
                        //     Utils.TILE_VALUE_ENEMY
                        // );

                        attackPoints.AddRange(leftDir);
                        attackPoints.AddRange(rightDir);
                        attackPoints.AddRange(down);
                        attackPoints.AddRange(up);
                    }

                    break;

                case UnitType.TANK:
                    foreach(Vector2 position in nextPossiblePositions) {
                        List<Vector2> leftDir  = new List<Vector2>();
                        List<Vector2> rightDir = new List<Vector2>();
                        List<Vector2> down  = new List<Vector2>();
                        List<Vector2> up    = new List<Vector2>();
                        // boardGrid.GetOrthogonalPositions(
                        //     position,
                        //     out leftDir,
                        //     out rightDir,
                        //     out down,
                        //     out up,
                        //     Utils.MAX_STEPS,
                        //     Utils.TILE_VALUE_ENEMY
                        // );

                        attackPoints.AddRange(leftDir);
                        attackPoints.AddRange(rightDir);
                        attackPoints.AddRange(down);
                        attackPoints.AddRange(up);
                    }

                    break;
                }
            }

            foreach (Vector2 point in attackPoints) {
                if (point == left)    ++minHits[0];
                if (point == current) ++minHits[1];
                if (point == right)   ++minHits[2];
            }

            int min1 = Mathf.Min(minHits[0], minHits[1]);
            int min = Mathf.Min(min1, minHits[2]);

            int safestPositionIndex = 1;

            for (int i = 0; i < 3; ++i) {
                if (minHits[i] == min) {
                    safestPositionIndex = i;
                    break;
                }
            }

            if (unit.CanMoveTo(positionsToGo[safestPositionIndex]))
                unit.MoveTo(positionsToGo[safestPositionIndex]);

            yield return new WaitForSeconds(actionsInterval);
        }

        allCommandersOrdered = true;
    }

    private void AddUnit(Unit newUnit) {
        Unit unit = newUnit;
        UnitType unitType = unit.GetUnitType();

        switch (unitType)
        {
            case UnitType.DRONE:
                drones.Add(unit);
                break;
            case UnitType.DREADNOUGHT:
                dreadnoughts.Add(unit);
                break;
            case UnitType.COMMANDER:
                commanders.Add(unit);
                break;
            default:
                Debug.LogError(
                    $"Trying to add unrecognized enemy unit type: {unitType}");
                break;
        }
    }

    private void SeparateUnits(List<Unit> enemyUnits) {
        drones       = new List<Unit>();
        dreadnoughts = new List<Unit>();
        commanders   = new List<Unit>();

        foreach (var unit in enemyUnits)
            AddUnit(unit);
    }

    private void ResetUnitsActions()
    {
        allDronesOrdered       = false;
        allDreadnoughtsOrdered = false;
        allCommandersOrdered   = false;

        foreach (Unit drone in drones)
            drone.Reset();

        foreach (Unit dreadnought in dreadnoughts)
            dreadnought.Reset();

        foreach (Unit commander in commanders)
            commander.Reset();
    }
}
