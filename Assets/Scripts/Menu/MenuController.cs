using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
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

    [Header("Button Labels")]
    public bool showButtonLabels = false;
    public string playLabelText = "Play";
    public string quitLabelText = "Quit";
    public bool disableLabelWhenHidden = true;

    [Header("Play Hover Auto-Load")]
    public bool autoLoadPlayHoverFrames = true;
    public string playHoverResourcesPath = "Menu/PlayHover";

    [Header("Play Hover Animation")]
    public Sprite[] playHoverFrames;
    public float playHoverFPS = 12f;
    public Coroutine playHoverRoutine;
    public bool isPlayHovered;
    public Sprite playBaseSprite;
    public Image playButtonImage;

    [Header("Play Hover Debug")]
    public bool playHoverDebugLogs = false;

    [Header("Loading Overlay")]
    public GameObject loadingOverlay;
    public float loadingDuration = 2f;

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

    private Coroutine loadingRoutine;
    private bool isLoading;

    private void Awake()
    {
        AutoLoadPlayHoverFrames();
        InitializePlayHover();
    }

    private void Start()
    {
        ApplyButtonSprites();
        ApplyButtonLabels();
        RestorePlayBaseSprite();
        ApplyButtonEnabled();
        ApplyBackground();
        UpdateHighscoreDisplay();
        SetLoadingOverlayActive(false);
        if (applyFontOnStart) ApplyFonts();
    }

    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            ApplyBackground();
            ApplyFonts();
            UpdateHighscoreDisplay();
            AutoLoadPlayHoverFrames();
            ApplyButtonLabels();
        }
    }

    public void OnButtonPressed(MenuAction action)
    {
        switch (action)
        {
            case MenuAction.Play:
                BeginPlayLoading();
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
        highscoreValue = PlayerPrefs.GetInt("HighScore", 0);
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

    public void ApplyButtonLabels()
    {
        ApplyButtonLabel(playButton, showButtonLabels ? playLabelText : string.Empty, showButtonLabels);
        ApplyButtonLabel(quitButton, showButtonLabels ? quitLabelText : string.Empty, showButtonLabels);
    }

    private void ApplyButtonLabel(MenuButton menuButton, string label, bool show)
    {
        if (menuButton == null) return;

        var text = menuButton.GetComponentInChildren<Text>(true);
        if (text == null) return;

        text.text = label ?? string.Empty;

        if (disableLabelWhenHidden)
        {
            text.gameObject.SetActive(show);
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

    private static void SetButtonInteractable(MenuButton menuButton, bool interactable)
    {
        if (menuButton == null) return;

        var btn = menuButton.GetComponent<Button>();
        if (btn != null) btn.interactable = interactable;

        var cg = menuButton.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }
    }

    private void InitializePlayHover()
    {
        if (playButton == null) return;

        EnsurePlayButtonImage();
        if (playBaseSprite == null)
        {
            if (playSprite != null)
            {
                playBaseSprite = playSprite;
            }
            else if (playButtonImage != null)
            {
                playBaseSprite = playButtonImage.sprite;
            }
        }

        RestorePlayBaseSprite();
        EnsurePlayHoverTrigger();
    }

    public void AutoLoadPlayHoverFrames()
    {
        if (!autoLoadPlayHoverFrames) return;
        if (string.IsNullOrWhiteSpace(playHoverResourcesPath)) return;

        var loaded = Resources.LoadAll<Sprite>(playHoverResourcesPath);
        if (loaded == null || loaded.Length == 0)
        {
            Debug.LogWarning("No Play hover sprites found at Resources path: " + playHoverResourcesPath);
            return;
        }

        var filtered = new System.Collections.Generic.List<Sprite>(loaded.Length);
        for (int i = 0; i < loaded.Length; i++)
        {
            if (loaded[i] != null)
            {
                filtered.Add(loaded[i]);
            }
        }

        filtered.Sort(ComparePlayHoverSprites);
        playHoverFrames = filtered.ToArray();

        if (playBaseSprite == null && playHoverFrames.Length > 0)
        {
            playBaseSprite = playHoverFrames[0];
        }
    }

    public void RefreshPlayHoverFromEditor()
    {
        AutoLoadPlayHoverFrames();
        InitializePlayHover();
        RestorePlayBaseSprite();
    }

    public void DebugPlayHoverSetup()
    {
        Debug.Log("Play Hover Debug ----------------");
        Debug.Log("PlayButton assigned: " + (playButton != null));
        Debug.Log("Play Enabled flag: " + playEnabled);
        Debug.Log("Hover Frames count: " + (playHoverFrames != null ? playHoverFrames.Length : 0));
        Debug.Log("Hover FPS: " + playHoverFPS);
        Debug.Log("Play Button Image assigned: " + (playButtonImage != null));

        var btn = playButton != null ? playButton.GetComponent<Button>() : null;
        Debug.Log("Button.interactable: " + (btn != null && btn.interactable));

        var cg = playButton != null ? playButton.GetComponent<CanvasGroup>() : null;
        Debug.Log("CanvasGroup ok: " + (cg == null || (cg.interactable && cg.blocksRaycasts && cg.alpha > 0f)));

        var trigger = playButton != null ? playButton.GetComponent<EventTrigger>() : null;
        Debug.Log("EventTrigger present: " + (trigger != null));
        if (trigger != null)
        {
            bool hasEnter = false;
            bool hasExit = false;
            for (int i = 0; i < trigger.triggers.Count; i++)
            {
                if (trigger.triggers[i].eventID == EventTriggerType.PointerEnter) hasEnter = true;
                if (trigger.triggers[i].eventID == EventTriggerType.PointerExit) hasExit = true;
            }
            Debug.Log("EventTrigger PointerEnter: " + hasEnter);
            Debug.Log("EventTrigger PointerExit: " + hasExit);
        }

        var canvas = GetComponentInParent<Canvas>();
        Debug.Log("Canvas found: " + (canvas != null));
        Debug.Log("GraphicRaycaster present: " + (canvas != null && canvas.GetComponent<GraphicRaycaster>() != null));

        var currentEventSystem = UnityEngine.EventSystems.EventSystem.current;
        bool hasEventSystem = currentEventSystem != null;
        if (!hasEventSystem)
        {
            currentEventSystem = FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>();
            hasEventSystem = currentEventSystem != null;
        }
        Debug.Log("EventSystem present: " + hasEventSystem);

        Debug.Log("Play Hover Debug ----------------");
    }

    private static int ComparePlayHoverSprites(Sprite a, Sprite b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return 1;
        if (b == null) return -1;

        int aNum = ExtractTrailingNumber(a.name);
        int bNum = ExtractTrailingNumber(b.name);
        if (aNum >= 0 && bNum >= 0)
        {
            return aNum.CompareTo(bNum);
        }

        return string.CompareOrdinal(a.name, b.name);
    }

    private static int ExtractTrailingNumber(string name)
    {
        if (string.IsNullOrEmpty(name)) return -1;

        int lastStart;
        int lastEnd;
        if (!TryFindLastNumberGroup(name, name.Length - 1, out lastStart, out lastEnd))
        {
            return -1;
        }

        if (lastStart > 0 && name[lastStart - 1] == '_')
        {
            int prevStart;
            int prevEnd;
            if (TryFindLastNumberGroup(name, lastStart - 2, out prevStart, out prevEnd))
            {
                return ParseNumber(name, prevStart, prevEnd);
            }
        }

        return ParseNumber(name, lastStart, lastEnd);
    }

    private static bool TryFindLastNumberGroup(string name, int endIndex, out int start, out int end)
    {
        start = -1;
        end = -1;
        if (endIndex < 0) return false;

        int i = endIndex;
        while (i >= 0 && (name[i] < '0' || name[i] > '9'))
        {
            i--;
        }

        if (i < 0) return false;

        end = i;
        while (i >= 0 && name[i] >= '0' && name[i] <= '9')
        {
            i--;
        }

        start = i + 1;
        return start <= end;
    }

    private static int ParseNumber(string name, int start, int end)
    {
        int value = 0;
        for (int i = start; i <= end; i++)
        {
            value = (value * 10) + (name[i] - '0');
        }

        return value;
    }

    private void EnsurePlayHoverTrigger()
    {
        if (playButton == null) return;

        var trigger = playButton.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = playButton.gameObject.AddComponent<EventTrigger>();
        }

        if (trigger.triggers == null)
        {
            trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
        }

        AddTriggerIfMissing(trigger, EventTriggerType.PointerEnter, StartPlayHoverAnimation);
        AddTriggerIfMissing(trigger, EventTriggerType.PointerExit, StopPlayHoverAnimation);
    }

    private static void AddTriggerIfMissing(EventTrigger trigger, EventTriggerType eventType, UnityEngine.Events.UnityAction action)
    {
        for (int i = 0; i < trigger.triggers.Count; i++)
        {
            if (trigger.triggers[i].eventID == eventType)
            {
                if (trigger.triggers[i].callback == null)
                {
                    trigger.triggers[i].callback = new EventTrigger.TriggerEvent();
                }

                trigger.triggers[i].callback.AddListener(_ => action());
                return;
            }
        }

        var entry = new EventTrigger.Entry { eventID = eventType };
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    public void StartPlayHoverAnimation()
    {
        if (isLoading || !CanPlayHover()) return;

        isPlayHovered = true;
        if (playHoverDebugLogs) Debug.Log("Play hover enter");
        if (playHoverRoutine != null)
        {
            StopCoroutine(playHoverRoutine);
            playHoverRoutine = null;
        }

        playHoverRoutine = StartCoroutine(PlayHoverAnimation());
    }

    public void StopPlayHoverAnimation()
    {
        isPlayHovered = false;
        if (playHoverDebugLogs) Debug.Log("Play hover exit");
        if (playHoverRoutine != null)
        {
            StopCoroutine(playHoverRoutine);
            playHoverRoutine = null;
        }

        RestorePlayBaseSprite();
    }

    private void BeginPlayLoading()
    {
        if (isLoading) return;
        if (!playEnabled) return;

        isLoading = true;
        StopPlayHoverAnimation();
        SetButtonInteractable(playButton, false);
        SetButtonInteractable(quitButton, false);
        SetLoadingOverlayActive(true);

        if (loadingRoutine != null)
        {
            StopCoroutine(loadingRoutine);
        }

        loadingRoutine = StartCoroutine(LoadAfterDelay());
    }

    private IEnumerator LoadAfterDelay()
    {
        float waitTime = Mathf.Max(0f, loadingDuration);
        if (waitTime > 0f)
        {
            yield return new WaitForSeconds(waitTime);
        }

        LoadScene(playSceneName, "Play");
    }

    private void SetLoadingOverlayActive(bool active)
    {
        if (loadingOverlay != null && loadingOverlay.activeSelf != active)
        {
            loadingOverlay.SetActive(active);
        }
    }

    private IEnumerator PlayHoverAnimation()
    {
        for (int i = 0; i < playHoverFrames.Length; i++)
        {
            if (!isPlayHovered || !IsPlayInteractable()) break;

            EnsurePlayButtonImage();
            var frame = playHoverFrames[i];
            if (frame != null && playButtonImage != null)
            {
                playButtonImage.sprite = frame;
            }

            yield return new WaitForSeconds(1f / playHoverFPS);
        }

        if (isPlayHovered && IsPlayInteractable())
        {
            EnsurePlayButtonImage();
            var lastFrame = playHoverFrames[playHoverFrames.Length - 1];
            if (lastFrame != null && playButtonImage != null)
            {
                playButtonImage.sprite = lastFrame;
            }

            while (isPlayHovered && IsPlayInteractable())
            {
                yield return null;
            }
        }

        playHoverRoutine = null;
        if (!isPlayHovered)
        {
            RestorePlayBaseSprite();
        }
    }

    private bool CanPlayHover()
    {
        if (!playEnabled) return false;
        EnsurePlayButtonImage();
        if (playButtonImage == null) return false;
        if (playHoverFrames == null || playHoverFrames.Length == 0) return false;
        if (playHoverFPS <= 0f) return false;
        return IsPlayInteractable();
    }

    private bool IsPlayInteractable()
    {
        if (playButton == null) return false;

        var btn = playButton.GetComponent<Button>();
        if (btn != null && !btn.interactable) return false;

        var cg = playButton.GetComponent<CanvasGroup>();
        if (cg != null && (!cg.interactable || !cg.blocksRaycasts || cg.alpha <= 0f)) return false;

        return true;
    }

    private void RestorePlayBaseSprite()
    {
        EnsurePlayButtonImage();
        if (playButtonImage != null && playBaseSprite != null)
        {
            playButtonImage.sprite = playBaseSprite;
        }
    }

    // Hover zoom removed; animation-only hover remains.

    private void EnsurePlayButtonImage()
    {
        if (playButtonImage != null) return;
        if (playButton == null) return;

        playButtonImage = playButton.GetComponent<Image>();
        if (playButtonImage == null)
        {
            playButtonImage = playButton.GetComponentInChildren<Image>(true);
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
