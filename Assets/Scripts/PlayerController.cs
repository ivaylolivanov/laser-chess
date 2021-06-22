using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    private Camera mainCamera;
    private BoardGrid boardGrid;

    private Unit selectedUnit;

    void Awake() {
        mainCamera = Camera.main;

        boardGrid = FindObjectOfType<BoardGrid>();
    }

    void Update() {
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
                if(pointedColliders.Length <= 0) {
                    if(selectedUnit.CanMoveTo(mouseWorldPosition)) {
                        selectedUnit.MoveTo(mouseWorldPosition);
                        selectedUnit.Deselect();
                        selectedUnit = null;
                    }
                    // else if(selectedUnit.CanAttackOn(mouseWorldPosition)) {
                    //     selectedUnit.AttackOn(mouseWorldPosition);
                    // }
                    else {
                        selectedUnit.Deselect();
                        selectedUnit = null;
                    }
                }
            }
        }
    }
}
