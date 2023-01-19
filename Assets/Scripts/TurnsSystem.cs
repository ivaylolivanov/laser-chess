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

        // StartCoroutine(SceneLoader.LoadNextSceneDelayed(3f));

        LevelWon?.Invoke();
    }
    public void Lost() {
        currentTurn = Turns.LOST;
        // StartCoroutine(SceneLoader.ReloadCurrentSceneDelayed(3f));

        LevelLost?.Invoke();
    }
}
