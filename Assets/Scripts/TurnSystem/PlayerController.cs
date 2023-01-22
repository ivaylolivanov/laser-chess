using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Camera mainCamera;
    private BoardGrid boardGrid;
    private TurnsSystem turnsSystem;

    private List<Unit> units;

    private Unit selectedUnit;

    void Awake() {
        mainCamera = Camera.main;

        boardGrid = FindObjectOfType<BoardGrid>();
        turnsSystem = FindObjectOfType<TurnsSystem>();

        units = new List<Unit>();
    }

    void Update() {
        if(turnsSystem.currentTurn != Turns.PLAYER_TURN) { return; }

        if(units.Count <= 0 ) {
            turnsSystem.Lost();
            return;
        }

        if(! AtLeast1UnitHasOptions()) {
            return;
        }

        if(Input.GetMouseButtonDown(0)) {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );
            mouseWorldPosition.z = 0;

            Collider2D[] pointedColliders = Physics2D.OverlapCircleAll(
                mouseWorldPosition,
                0.1f
            );

            if(selectedUnit == null) {
                foreach(var collider in pointedColliders) {
                    selectedUnit = collider.GetComponent<Unit>();
                    if(selectedUnit != null) {
                        selectedUnit.Select();
                        break;
                    }
                }
            }
            else {
                if(selectedUnit.CanMoveTo(mouseWorldPosition)) {
                    selectedUnit.MoveTo(mouseWorldPosition);
                }

                if(! selectedUnit.HasMoved()
                   && selectedUnit.CanAttackOn(mouseWorldPosition)
                ) {
                    selectedUnit.Attack(mouseWorldPosition);
                }

                selectedUnit.Deselect();
                selectedUnit = null;
            }
        }
    }

    public void AddUnit(Unit newUnit) {
        if(units != null) { units.Add(newUnit); }
    }

    public void RemoveUnit(Unit unit) {
        units.Remove(unit);

        boardGrid.UpdateTile(unit.transform.position);
    }

    public void ResetUnits() {
        foreach(Unit unit in units) { unit.Reset(); }
    }

    private bool AtLeast1UnitHasOptions() {
        bool result = false;

        foreach(Unit unit in units) {
            if(unit.HasOptions()) {
                result = true;
                break;
            }
        }

        return result;
    }
}
