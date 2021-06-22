using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] private UnitStats stats;

    private Canvas unitCanvas;
    private Healthbar healthbar;
    private IMovement movement;

    void Awake() {
        unitCanvas = GetComponentInChildren<Canvas>();
        healthbar  = GetComponentInChildren<Healthbar>();

        movement = GetComponent<IMovement>();
    }

    void Start() {
        if(healthbar != null) { healthbar.SetMaxHealth(stats.maxHealth); };
        Deselect();
    }

    public bool CanMoveTo(Vector2Int worldPosition) {
        bool result = false;

        if(movement != null) {
            result = movement.CanMoveTo(worldPosition);
        }

        return result;
    }

    public bool CanMoveTo(Vector2 worldPosition) {
        return CanMoveTo(Vector2Int.FloorToInt(worldPosition));
    }

    public void MoveTo(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        StartCoroutine(movement.Move(worldPositionInt));
    }

    public void Select() {
        if(unitCanvas != null) {
            unitCanvas.enabled = true;
        }

        if(movement != null) {
            movement.ShowIndicator();
            movement.GetAvailablePositions();
            movement.CreateIndicatorMesh();
        }
    }

    public void Deselect() {
        if(unitCanvas != null) {
            unitCanvas.enabled = false;
        }

        if(movement != null) {
            movement.HideIndicator();
        }
    }
}
