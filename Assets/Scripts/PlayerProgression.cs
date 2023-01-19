using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
    public static void UpdatePlayerProgression(int level) {
        if (!SceneLoader.IsSceneALevel(level)) return;

        int currentHighestUnlockedLevel = PlayerPrefs.GetInt(
            Utils.HIGHEST_UNLOCKED_LEVEL);

        if (currentHighestUnlockedLevel >= level) return;

        PlayerPrefs.SetInt(Utils.HIGHEST_UNLOCKED_LEVEL, level);
        PlayerPrefs.Save();
    }

    public static int GetPlayerProgression() {
        int highestLevel = 1;

        if (PlayerPrefs.HasKey(Utils.HIGHEST_UNLOCKED_LEVEL))
            highestLevel = PlayerPrefs.GetInt(Utils.HIGHEST_UNLOCKED_LEVEL);

        return highestLevel;
    }
}
