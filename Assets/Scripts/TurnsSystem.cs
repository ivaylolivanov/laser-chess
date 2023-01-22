using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct UnitLevelData
{
    [SerializeField] public Vector2 position;
    [SerializeField] public GameObject unitTemplate;
}


public class TurnsSystem : MonoBehaviour {
    [SerializeField] private UnitLevelData[] unitLevelData;

    private PlayerController player;
    private EnemyController enemy;

    public static UnityEvent LevelWon;
    public static UnityEvent LevelLost;

    public Turn currentTurn { get; private set; }

    private List<Unit> playerUnits;
    private List<Unit> enemyUnits;

    private ITurn[] possibleTurns;

    void OnEnable() {
        SetupBoard();

        possibleTurns = new ITurn[3];
        possibleTurns[(uint) Turn.PLAYER] = new PlayerTurn(playerUnits);
        possibleTurns[(uint) Turn.ENEMY]  = new EnemyTurn(enemyUnits, this);

        currentTurn = Turn.PLAYER;
    }

    private void Update() {
        if (currentTurn == Turn.LOST || currentTurn == Turn.WON)
        {
            Debug.Log($"Level {currentTurn}");
            return;
        }

        if (currentTurn == Turn.PLAYER || currentTurn == Turn.ENEMY)
        {
            Debug.Log($"Level {currentTurn}");
            currentTurn = possibleTurns[(uint)currentTurn].Actions();
        }
    }

    public void Won() {
        currentTurn = Turn.WON;

        int currentSceneIndex = SceneLoader.GetCurrentSceneBuildIndex();
        PlayerProgression.UpdatePlayerProgression(currentSceneIndex);

        LevelWon?.Invoke();
    }

    public void Lost() {
        currentTurn = Turn.LOST;

        LevelLost?.Invoke();
    }

    private void SetupBoard() {
        playerUnits = new List<Unit>();
        enemyUnits  = new List<Unit>();

        Transform unitsParent = new GameObject("AllUnits")?.transform;
        unitsParent.SetParent(transform);

        Transform playerUnitsParent = new GameObject("PlayerUnits")?.transform;
        playerUnitsParent.SetParent(unitsParent);

        Transform enemyUnitsParent = new GameObject("EnemyUnits")?.transform;
        enemyUnitsParent.SetParent(unitsParent);

        foreach (var data in unitLevelData) {
            GameObject unitGO = Instantiate(data.unitTemplate);
            Unit unit = unitGO.GetComponent<Unit>();

            if ((unit.GetUnitType() & UnitType.IS_PLAYER_UNIT) != 0) {
                playerUnits.Add(unit);

                unit.transform.SetParent(playerUnitsParent);
                unit.transform.position = data.position;
            }
            else if ((unit.GetUnitType() & UnitType.IS_ENEMY_UNIT) != 0) {
                enemyUnits.Add(unit);

                unit.transform.SetParent(enemyUnitsParent);
                unit.transform.position = data.position;
            }
        }
    }
}
