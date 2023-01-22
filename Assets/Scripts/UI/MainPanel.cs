using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button missionSelectionButton;
    [SerializeField] private Button quitButton;

    [Space]
    [Header("Popups")]
    [SerializeField] private GameObject missionSelectionPopup;

    void OnEnable()
    {
        playButton.onClick.AddListener(LoadFirstLevel);
        missionSelectionButton.onClick.AddListener(OpenMissionSelectionPopup);
        quitButton.onClick.AddListener(QuitGame);
    }

    void OnDisable()
    {
        playButton.onClick.RemoveListener(LoadFirstLevel);
        missionSelectionButton.onClick.RemoveListener(OpenMissionSelectionPopup);
        quitButton.onClick.RemoveListener(QuitGame);
    }

    private void OpenMissionSelectionPopup()
        => missionSelectionPopup.transform.localScale = Vector3.one;
    private void LoadFirstLevel() => SceneLoader.LoadNextScene();
    private void QuitGame() => SceneLoader.Quit();
}
