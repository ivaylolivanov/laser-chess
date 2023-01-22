using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour {
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    void OnEnable() {
        closeMenuButton.onClick.AddListener(SelfShow);
        mainMenuButton.onClick.AddListener(SceneLoader.LoadMainMenu);
        quitButton.onClick.AddListener(SceneLoader.Quit);

        SelfHide();
    }

    void OnDisable() {
        closeMenuButton.onClick.RemoveListener(SelfShow);
        mainMenuButton.onClick.RemoveListener(SceneLoader.LoadMainMenu);
        quitButton.onClick.RemoveListener(SceneLoader.Quit);
    }

    private void Update() {
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        if (Utils.IsObjectShown(transform)) SelfHide();
        else                                SelfShow();
    }

    private void SelfShow() => Utils.ShowObject(transform);
    private void SelfHide() => Utils.HideObject(transform);
}
