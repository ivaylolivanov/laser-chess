using System.Collections.Generic;
using UnityEngine;

public class PlayerTurn : ITurn
{
    private List<Unit> units;

    private Unit selectedUnit;

    public PlayerTurn(List<Unit> playerUnits) {
        units = playerUnits;
        selectedUnit = null;
    }

    public Turn Actions() {
        if (units.Count <= 0 ) return Turn.LOST;

        if (! AtLeast1UnitHasOptions()) {
            ResetUnitsActions();

            return Turn.ENEMY;
        }

        if (Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorldPosition = Utils.GetMouseWorldPoint();

            Collider2D[] pointedColliders = Physics2D.OverlapCircleAll(
                mouseWorldPosition,
                0.1f // TODO: Define as constant
            );

            if (selectedUnit == null) {
                foreach(var collider in pointedColliders) {
                    selectedUnit = collider.GetComponent<Unit>();
                    if (selectedUnit != null) {
                        selectedUnit.Select();
                        return Turn.PLAYER;
                    }
                }
            }
            else {
                if (selectedUnit.CanMoveTo(mouseWorldPosition))
                    selectedUnit.MoveTo(mouseWorldPosition);

                if (! selectedUnit.HasMoved()
                   && selectedUnit.CanAttackOn(mouseWorldPosition)
                )
                    selectedUnit.Attack(mouseWorldPosition);

                selectedUnit.Deselect();
                selectedUnit = null;
            }
        }

        return Turn.PLAYER;
    }

    private bool AtLeast1UnitHasOptions() {
        bool result = false;

        foreach (Unit unit in units) {
            if (unit.HasOptions()) {
                result = true;
                break;
            }
        }

        return result;
    }

    private void ResetUnitsActions()
    {
        foreach (Unit unit in units)
            unit.Reset();
    }
}
