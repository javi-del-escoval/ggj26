using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Button References")]
    public MenuButton playButton;
    public MenuButton quitButton;

    [Header("Button Enabled")]
    public bool playEnabled = true;
    public bool quitEnabled = true;

    [Header("Sprite Placeholders")]
    public Sprite playSprite;
    public Sprite quitSprite;

    [Header("Scene Names")]
    public string playSceneName = "Game";

    [Header("Background")]
    public Image backgroundImage;
    public Sprite backgroundSprite;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.6f);

    [Header("UI Font")]
    public Font uiFont;
    public bool applyFontOnStart = true;
    public int buttonFontSize = 32;
    public int highscoreFontSize = 28;

    [Header("Highscore Display")]
    public int highscoreValue = 0;
    public string highscoreLabel = "Highscore: ";
    public Text highscoreText;

    private void Start()
    {
        ApplyButtonSprites();
        ApplyButtonEnabled();
        ApplyBackground();
        UpdateHighscoreDisplay();
        if (applyFontOnStart) ApplyFonts();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyBackground();
            ApplyFonts();
            UpdateHighscoreDisplay();
        }
    }

    public void OnButtonPressed(MenuAction action)
    {
        switch (action)
        {
            case MenuAction.Play:
                LoadScene(playSceneName, "Play");
                break;
            case MenuAction.Quit:
                QuitGame();
                break;
        }
    }

    public void ApplyButtonSprites()
    {
        if (playButton != null) playButton.ApplySprite(playSprite);
        if (quitButton != null) quitButton.ApplySprite(quitSprite);
    }

    public void ApplyButtonEnabled()
    {
        SetButtonEnabled(playButton, playEnabled);
        SetButtonEnabled(quitButton, quitEnabled);
    }

    public void ApplyBackground()
    {
        if (backgroundImage == null) return;
        backgroundImage.color = backgroundColor;
        if (backgroundSprite != null) backgroundImage.sprite = backgroundSprite;
    }

    public void UpdateHighscoreDisplay()
    {
        if (highscoreText == null) return;
        highscoreText.text = highscoreLabel + highscoreValue;
    }

    public void ApplyFonts()
    {
        ApplyFontToButtonLabel(playButton, uiFont, buttonFontSize);
        ApplyFontToButtonLabel(quitButton, uiFont, buttonFontSize);

        if (highscoreText != null)
        {
            if (uiFont != null) highscoreText.font = uiFont;
            if (highscoreFontSize > 0) highscoreText.fontSize = highscoreFontSize;
        }
    }

    private static void ApplyFontToButtonLabel(MenuButton menuButton, Font font, int size)
    {
        if (menuButton == null) return;

        var text = menuButton.GetComponentInChildren<Text>(true);
        if (text != null)
        {
            if (font != null) text.font = font;
            if (size > 0) text.fontSize = size;
        }
    }

    private static void SetButtonEnabled(MenuButton menuButton, bool enabled)
    {
        if (menuButton == null) return;

        var btn = menuButton.GetComponent<Button>();
        if (btn != null) btn.interactable = enabled;

        var cg = menuButton.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = enabled ? 1f : 0f;
            cg.interactable = enabled;
            cg.blocksRaycasts = enabled;
        }
    }

    private void LoadScene(string sceneName, string label)
    {
        Debug.Log(label + " pressed");
        if (!string.IsNullOrWhiteSpace(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning(label + " scene name is empty.");
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quit pressed");
        Application.Quit();
    }
}
