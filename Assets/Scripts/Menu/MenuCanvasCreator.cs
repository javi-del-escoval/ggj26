#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public static class MenuCanvasCreator
{
    [MenuItem("Tools/Create Menu Canvas")]
    public static void CreateMenuCanvas()
    {
        RepairMenuMissingScripts();

        var canvasGO = new GameObject("MenuCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        EnsureEventSystem();

        var controller = canvasGO.AddComponent<MenuController>();

        var panel = CreateUIObject("Panel", canvasGO.transform);
        var panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.6f);
        StretchToParent(panel.GetComponent<RectTransform>());

        var logo = CreateUIObject("LogoPlaceholder", panel.transform);
        var logoImage = logo.AddComponent<Image>();
        logoImage.preserveAspect = true;
        var logoRT = logo.GetComponent<RectTransform>();
        logoRT.anchorMin = new Vector2(0.5f, 1f);
        logoRT.anchorMax = new Vector2(0.5f, 1f);
        logoRT.pivot = new Vector2(0.5f, 1f);
        logoRT.anchoredPosition = new Vector2(0f, -40f);
        logoRT.sizeDelta = new Vector2(400f, 120f);

        var buttonContainer = CreateUIObject("Buttons", panel.transform);
        var containerRT = buttonContainer.GetComponent<RectTransform>();
        containerRT.anchorMin = new Vector2(0.5f, 0.5f);
        containerRT.anchorMax = new Vector2(0.5f, 0.5f);
        containerRT.pivot = new Vector2(0.5f, 0.5f);
        containerRT.anchoredPosition = new Vector2(0f, -40f);
        containerRT.sizeDelta = new Vector2(300f, 240f);

        var playBtn = CreateButton("PlayButton", buttonContainer.transform, "Play");
        var quitBtn = CreateButton("QuitButton", buttonContainer.transform, "Quit");

        PositionButton(playBtn.GetComponent<RectTransform>(), 0f);
        PositionButton(quitBtn.GetComponent<RectTransform>(), -80f);

        var highscoreText = CreateHighscoreText(panel.transform);

        var playMenuBtn = EnsureMenuButton(playBtn, controller, MenuAction.Play);
        var quitMenuBtn = EnsureMenuButton(quitBtn, controller, MenuAction.Quit);

        AssignControllerFields(controller, playMenuBtn, quitMenuBtn, highscoreText, panelImage);

        EditorUtility.SetDirty(controller);
        EditorSceneManager.MarkSceneDirty(canvasGO.scene);

        Selection.activeGameObject = canvasGO;
    }

    [MenuItem("Tools/Auto-Assign Menu Buttons")]
    public static void AutoAssignMenuButtons()
    {
        var controller = GetTargetController();
        if (controller == null)
        {
            Debug.LogWarning("No MenuController found. Select a MenuCanvas (or a child) or ensure one exists in the scene.");
            return;
        }

        var buttons = controller.GetComponentsInChildren<MenuButton>(true);
        foreach (var btn in buttons)
        {
            EnsureMenuButton(btn.gameObject, controller, btn.action);
        }

        EnsureHighscoreText(controller);

        EditorUtility.SetDirty(controller);
        Debug.Log("Menu buttons auto-assigned.");
    }

    [MenuItem("Tools/Replace EventSystem Input Module")]
    public static void ReplaceEventSystemInputModule()
    {
        var systems = Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (systems.Length == 0)
        {
            Debug.LogWarning("No EventSystem found in the scene.");
            return;
        }

        foreach (var es in systems)
        {
#if ENABLE_INPUT_SYSTEM
            var legacy = es.GetComponent<StandaloneInputModule>();
            if (legacy != null) Object.DestroyImmediate(legacy);

            if (es.GetComponent<InputSystemUIInputModule>() == null)
            {
                es.gameObject.AddComponent<InputSystemUIInputModule>();
            }
#else
            var newInput = es.GetComponent<InputSystemUIInputModule>();
            if (newInput != null) Object.DestroyImmediate(newInput);

            if (es.GetComponent<StandaloneInputModule>() == null)
            {
                es.gameObject.AddComponent<StandaloneInputModule>();
            }
#endif
            EditorUtility.SetDirty(es.gameObject);
        }

        Debug.Log("EventSystem input module replaced.");
    }

    [MenuItem("Tools/Repair Menu Missing Scripts")]
    public static void RepairMenuMissingScripts()
    {
        var menuCanvas = GameObject.Find("MenuCanvas");
        if (menuCanvas != null)
        {
            RemoveMissingScriptsInHierarchy(menuCanvas.gameObject);
        }

        var controllers = Object.FindObjectsByType<MenuController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (controllers.Length == 0) return;

        foreach (var controller in controllers)
        {
            EnsureMenuButtons(controller);
            EnsureHighscoreText(controller);
        }
    }

    private static MenuController GetTargetController()
    {
        var selected = Selection.activeGameObject;
        if (selected != null)
        {
            var inSelection = selected.GetComponentInParent<MenuController>();
            if (inSelection != null) return inSelection;
        }

        var controllers = Object.FindObjectsByType<MenuController>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (controllers.Length == 1) return controllers[0];

        return null;
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindAnyObjectByType<EventSystem>() != null) return;

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
        es.AddComponent<InputSystemUIInputModule>();
#else
        es.AddComponent<StandaloneInputModule>();
#endif
    }

    private static GameObject CreateButton(string name, Transform parent, string label)
    {
        var btnGO = CreateUIObject(name, parent);
        var image = btnGO.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.9f);
        btnGO.AddComponent<Button>();

        var rt = btnGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(260f, 60f);

        var textGO = CreateUIObject("Label", btnGO.transform);
        var text = textGO.AddComponent<Text>();
        text.text = label;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        StretchToParent(textGO.GetComponent<RectTransform>());

        return btnGO;
    }

    private static Text CreateHighscoreText(Transform parent)
    {
        var go = CreateUIObject("HighscoreText", parent);
        var text = go.AddComponent<Text>();
        text.text = "Highscore: 0";
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, 120f);
        rt.sizeDelta = new Vector2(400f, 40f);

        return text;
    }

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void StretchToParent(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static void AssignControllerFields(MenuController controller, MenuButton play, MenuButton quit, Text highscoreText, Image backgroundImage)
    {
        if (controller == null) return;

        controller.playButton = play;
        controller.quitButton = quit;
        controller.highscoreText = highscoreText;
        controller.backgroundImage = backgroundImage;

        EditorUtility.SetDirty(controller);
    }

    private static void PositionButton(RectTransform rt, float y)
    {
        if (rt == null) return;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, y);
    }

    private static void RemoveMissingScriptsInHierarchy(GameObject root)
    {
        if (root == null) return;

        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(root);

        var t = root.transform;
        for (var i = 0; i < t.childCount; i++)
        {
            RemoveMissingScriptsInHierarchy(t.GetChild(i).gameObject);
        }
    }

    private static void EnsureMenuButtons(MenuController controller)
    {
        if (controller == null) return;

        var root = controller.transform;
        var buttonsContainerGO = FindDeepChild(root, "Buttons");
        if (buttonsContainerGO == null)
        {
            var panelGO = FindDeepChild(root, "Panel");
            var parentTransform = panelGO != null ? panelGO.transform : root;
            var containerGO = CreateUIObject("Buttons", parentTransform);
            buttonsContainerGO = containerGO;
        }

        var playGO = FindDeepChild(root, "PlayButton") ?? CreateButton("PlayButton", buttonsContainerGO.transform, "Play");
        var quitGO = FindDeepChild(root, "QuitButton") ?? CreateButton("QuitButton", buttonsContainerGO.transform, "Quit");

        var playMenuBtn = EnsureMenuButton(playGO, controller, MenuAction.Play);
        var quitMenuBtn = EnsureMenuButton(quitGO, controller, MenuAction.Quit);

        AssignControllerFields(controller, playMenuBtn, quitMenuBtn, controller.highscoreText, controller.backgroundImage);
        EditorUtility.SetDirty(controller);
    }

    private static void EnsureHighscoreText(MenuController controller)
    {
        if (controller == null) return;

        if (controller.highscoreText == null)
        {
            var panelGO = FindDeepChild(controller.transform, "Panel");
            var parentTransform = panelGO != null ? panelGO.transform : controller.transform;
            controller.highscoreText = CreateHighscoreText(parentTransform);
            EditorUtility.SetDirty(controller);
        }
    }

    private static MenuButton EnsureMenuButton(GameObject go, MenuController controller, MenuAction action)
    {
        if (go == null) return null;

        var image = go.GetComponent<Image>();
        if (image == null) image = go.AddComponent<Image>();

        var button = go.GetComponent<Button>();
        if (button == null) button = go.AddComponent<Button>();

        var menuButton = go.GetComponent<MenuButton>();
        if (menuButton == null) menuButton = go.AddComponent<MenuButton>();

        menuButton.controller = controller;
        menuButton.action = action;
        menuButton.targetImage = image;

        FixOnClick(button, menuButton);

        EditorUtility.SetDirty(go);
        return menuButton;
    }

    private static void FixOnClick(Button button, MenuButton menuButton)
    {
        if (button == null || menuButton == null) return;

        button.onClick.RemoveAllListeners();

        var evt = button.onClick;
        for (var i = evt.GetPersistentEventCount() - 1; i >= 0; i--)
        {
            UnityEventTools.RemovePersistentListener(evt, i);
        }

        UnityEventTools.AddPersistentListener(evt, menuButton.Invoke);

        EditorUtility.SetDirty(button);
    }

    private static GameObject FindDeepChild(Transform parent, string name)
    {
        if (parent == null) return null;

        for (var i = 0; i < parent.childCount; i++)
        {
            var child = parent.GetChild(i);
            if (child.name == name) return child.gameObject;

            var found = FindDeepChild(child, name);
            if (found != null) return found;
        }

        return null;
    }
}
#endif
