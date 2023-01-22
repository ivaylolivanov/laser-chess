using UnityEngine;
using UnityEngine.Events;

public enum Turns {PLAYER_TURN, ENEMY_TURN, WON, LOST }

public class TurnsSystem : MonoBehaviour {
    private PlayerController player;
    private EnemyController enemy;

    public static UnityAction LevelWon;
    public static UnityAction LevelLost;

    public Turns currentTurn { get; private set; }

    void Awake() {
        player = FindObjectOfType<PlayerController>();
        enemy = FindObjectOfType<EnemyController>();

        EndPlayerTurn.OnEndPlayerTurnButtonPressed += EnemyTurn;
    }

    void Start() {
        player.ResetUnits();
        enemy.ResetUnits();

        currentTurn = Turns.PLAYER_TURN;
    }

    public void PlayerTurn() {
        player.ResetUnits();

        currentTurn = Turns.PLAYER_TURN;
    }

    public void EnemyTurn() {
        enemy.ResetUnits();

        currentTurn = Turns.ENEMY_TURN;
    }

    public void Won() {
        currentTurn = Turns.WON;

        int currentSceneIndex = SceneLoader.GetCurrentSceneBuildIndex();
        PlayerProgression.UpdatePlayerProgression(currentSceneIndex);

        LevelWon?.Invoke();
    }

    public void Lost() {
        currentTurn = Turns.LOST;

        LevelLost?.Invoke();
    }
}
