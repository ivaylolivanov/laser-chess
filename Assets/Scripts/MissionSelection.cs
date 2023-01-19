using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionSelection : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;

    [Space]
    [Header("Level cards")]
    [SerializeField] private Transform levelCardsParent;
    [SerializeField] private Sprite levelCardUnlockedSprite;
    [SerializeField] private Sprite levelCardLockedSprite;
    [SerializeField] private TMP_FontAsset levelCardFont;
    [SerializeField] private Color lockedLevelTextColor;

    private void OnEnable() {
        CreateLevelCards();

        closeButton.onClick.AddListener(CloseMenu);
        CloseMenu();
    }

    private void OnDisable() {
        closeButton.onClick.RemoveListener(CloseMenu);
    }

    private void CreateLevelCards() {
        int levelsProgress = PlayerProgression.GetPlayerProgression();
        int levelsCount = SceneLoader.GetLevelsCount();

        for (int i = 0; i < levelsCount; ++i) {
            int level = i + 1;

            CreateLevelCard(level, levelsProgress);
        }
    }

    private void CreateLevelCard(int level, int levelsProgress) {
        Sprite background = levelCardUnlockedSprite;

        Transform root = CreateLevelCardRoot(level);

        bool levelIsLocked = level > levelsProgress;
        if (levelIsLocked) background = levelCardLockedSprite;

        Image backgroundImage = CreateLevelCardBackground(root, background);

        if (!levelIsLocked)
            AddButton(root, level, backgroundImage);

        CreateLevelCardText(root, level, levelIsLocked);
    }

    private Transform CreateLevelCardRoot(int level) {
        GameObject levelCard = new GameObject($"Level{level}");
        levelCard.transform.SetParent(levelCardsParent);
        levelCard.AddComponent<RectTransform>();

        return levelCard.transform;
    }

    private Button AddButton(Transform root, int level, Image background) {
        Button button = root.gameObject.AddComponent<Button>();
        button.onClick.AddListener(delegate { SceneLoader.LoadScene(level); });
        button.targetGraphic = background;

        return button;
    }

    private Image CreateLevelCardBackground(Transform parent, Sprite background) {
        GameObject levelCardBackground = new GameObject("Background");
        levelCardBackground.transform.SetParent(parent);
        RectTransform rectTransform = levelCardBackground
            .AddComponent<RectTransform>();

        StretchRectTransform(rectTransform);

        Image cardImage = levelCardBackground.AddComponent<Image>();
        cardImage.sprite = background;

        return cardImage;
    }

    private TextMeshProUGUI CreateLevelCardText(Transform parent, int level,
                                                bool locked) {
        GameObject levelCardText = new GameObject("LevelText");
        levelCardText.transform.SetParent(parent);

        RectTransform textRectTransform = levelCardText
            .AddComponent<RectTransform>();

        StretchRectTransform(textRectTransform);

        TextMeshProUGUI levelText = levelCardText
            .AddComponent<TextMeshProUGUI>();
        levelText.font = levelCardFont;
        levelText.fontSize = 100f;
        levelText.text = $"{level}";
        levelText.alignment = TextAlignmentOptions.MidlineGeoAligned;

        if (locked)
            levelText.color = lockedLevelTextColor;

        return levelText;
    }

    private void StretchRectTransform(RectTransform rectTransform) {
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private void CloseMenu() {
        gameObject.transform.localScale = Vector3.zero;
    }
}
