using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {
    [SerializeField] protected Stats stats;

    protected Canvas unitCanvas;
    protected Healthbar healthbar;
    protected Movement movement;
    protected Attack attack;

    protected BoardGrid boardGrid;

    protected void Awake() {
        unitCanvas = GetComponentInChildren<Canvas>();
        healthbar  = GetComponentInChildren<Healthbar>();

        movement = GetComponent<Movement>();
        attack = GetComponent<Attack>();

        boardGrid = FindObjectOfType<BoardGrid>();
    }

    protected void Start() {
        if(healthbar != null) { healthbar.SetMaxHealth(stats.maxHealth); }
        Deselect();
    }

    public virtual void Select() { }

    public bool CanMoveTo(Vector2Int worldPosition) {
        bool result = false;

        if((movement != null) && (! movement.hasMoved)) {
            result = movement.CanMoveTo(worldPosition);
        }

        return result;
    }

    public bool CanMoveTo(Vector2 worldPosition) {
        return CanMoveTo(Vector2Int.FloorToInt(worldPosition));
    }

    public bool CanAttackOn(Vector2Int worldPosition) {
        bool result = false;

        if((attack != null) && (! attack.hasAttacked)) {
            result = attack.CanAttackOn(worldPosition);
        }

        return result;
    }

    public bool CanAttackOn(Vector2 worldPosition) {
        return CanAttackOn(Vector2Int.FloorToInt(worldPosition));
    }

    public virtual void MoveTo(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        movement.Move(worldPositionInt);
    }

    public Vector2 GetPositionClosestTo(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        return movement.GetPositionClosestTo(worldPositionInt);
    }

    public List<Vector2> GetMovementPositions() {
        List<Vector2> result = new List<Vector2>();
        if(movement == null) { return result; }

        return movement.GetAllPossiblePositions();
    }

    public void Attack(Vector2 worldPosition) {
        Vector2Int worldPositionInt = Vector2Int.FloorToInt(worldPosition);
        attack.Execute(worldPositionInt);
    }

    public void TakeDamage(int damage) {
        if(healthbar == null) { return; }

        healthbar.TakeDamage(damage);

        if((unitCanvas != null) && (unitCanvas.enabled != true)) {
            StartCoroutine(ShowHealthbarBriefly());
        }
    }

    public void Deselect() {
        if(unitCanvas != null) { unitCanvas.enabled = false; }
        if(movement != null) { movement.HideIndicator(); }
        if(attack != null) { attack.HideIndicator(); }
    }

    public bool HasMoved()
        => (movement != null)
        && (! movement.hasMoved)
        && movement.HasMovementOptions();

    public void SetHasMoved(bool state) {
        if(movement == null) { return; }

        movement.hasMoved = state;
    }

    public void SetHasAttacked(bool state) {
        if(attack == null) { return; }

        attack.hasAttacked = state;
    }

    public bool HasOptions() {
        bool result = false;

        if(movement != null) {
            movement.GetAvailablePositions();

            result = (! movement.hasMoved) && movement.HasMovementOptions();
        }

        if(attack != null) {
            attack.GetAvailablePositions();

            result = result ||
                ((! attack.hasAttacked) && attack.HasAttackOptions());

        }

        return result;
    }

    public void Reset() {
        if(movement != null) { movement.hasMoved = false; }
        if(attack != null) { attack.hasAttacked = false; }
    }

    private IEnumerator ShowHealthbarBriefly() {
        float decreaseWaitPeriod = 0.1f;
        float waitPeriod = 0.5f;

        unitCanvas.enabled = false;
        for(int i = 0; i < 4; ++i) {
            unitCanvas.enabled = (! unitCanvas.enabled);
            yield return new WaitForSeconds(waitPeriod);
            waitPeriod -= decreaseWaitPeriod;
        }
    }
}
