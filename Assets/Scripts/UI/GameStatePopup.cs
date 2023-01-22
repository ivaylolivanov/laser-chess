using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameStatePopup : MonoBehaviour {
    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI gameStateButtonText;
    [Space]
    [Header("Buttons")]
    [SerializeField] private Button gameStateButton;
    [SerializeField] private Button mainMenuButton;

    #region Constants

    private const string LEVEL_WON_TITLE  = "Congratulations! No aliens around!";
    private const string LEVEL_LOST_TITLE = "You failed to push back the aliens!";

    private const string LEVEL_WON_BUTTON_TEXT  = "Next level";
    private const string LEVEL_LOST_BUTTON_TEXT = "Try again";

    #endregion

    public void OnEnable() {
        Utils.HideObject(transform);
        mainMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);

        TurnsSystem.LevelWon  += OnLevelWon;
        TurnsSystem.LevelLost += OnLevelLost;
    }

    public void OnDisable() {
        mainMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);

        TurnsSystem.LevelWon  -= OnLevelWon;
        TurnsSystem.LevelLost -= OnLevelLost;
    }

    private void OnLevelWon() {
        title.text = LEVEL_WON_TITLE;
        gameStateButtonText.text = LEVEL_WON_BUTTON_TEXT;

        gameStateButton.onClick.AddListener(SceneLoader.LoadNextScene);

        Utils.ShowObject(transform);
    }

    private void OnLevelLost() {
        title.text = LEVEL_LOST_TITLE;
        gameStateButtonText.text = LEVEL_LOST_BUTTON_TEXT;

        gameStateButton.onClick.AddListener(SceneLoader.ReloadCurrentScene);

        Utils.ShowObject(transform);
    }

    // private void Show() {
    //     transform.localScale = Vector3.one;
    // }

    // private void Hide() {
    //     transform.localScale = Vector3.zero;
    // }

}
