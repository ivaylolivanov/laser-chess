using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoader {

    public const int MAIN_MENU_BUILD_INDEX = 0;
    // Main scene, Level selection scene, End screen
    public const int NON_LEVEL_SCENES_COUNT = 1;

    public static int GetCurrentSceneBuildIndex()
        => SceneManager.GetActiveScene().buildIndex;

    public static bool IsSceneALevel(int index) {
        bool result = (index > 0) && (index < GetScenesCount());
        return result;
    }

    public static bool IsSceneValid(int index) {
        Scene scene = SceneManager.GetSceneByBuildIndex(index);

        bool result = scene != null;

        return result;
    }

    public static int GetLevelsCount()
        => GetScenesCount() - NON_LEVEL_SCENES_COUNT;

    public static int GetScenesCount()
        => SceneManager.sceneCountInBuildSettings;

    public static void LoadScene(int sceneIndex) {
        if (!IsSceneValid(sceneIndex)) return;

        SceneManager.LoadScene(sceneIndex);
    }

    public static void LoadMainMenu() {
        SceneLoader.LoadScene(MAIN_MENU_BUILD_INDEX);
    }

    public static void LoadNextScene() {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = sceneIndex + 1;

        if (!IsSceneValid(nextSceneIndex))
            nextSceneIndex = 0;

        SceneManager.LoadScene(nextSceneIndex);
    }

    public static void ReloadCurrentScene() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public static IEnumerator LoadNextSceneDelayed(float delay) {
        yield return new WaitForSeconds(delay);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(++currentSceneIndex);
    }

    public static IEnumerator ReloadCurrentSceneDelayed(float delay) {
        yield return new WaitForSeconds(delay);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public static void Quit() => Application.Quit();
}
