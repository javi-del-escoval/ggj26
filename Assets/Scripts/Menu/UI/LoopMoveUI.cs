using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoopMoveUI : MonoBehaviour
{
    [Header("Target")]
    public RectTransform target;
    public Vector2 from;
    public Vector2 to;

    [Header("Timing")]
    public float startDelaySeconds = 0f;
    public float moveDuration = 2f;
    public float pauseSeconds = 0f;
    public bool useUnscaledTime = true;

    [Header("Behavior")]
    public bool restartOnEnable = true;
    public bool stopOnDisable = true;
    public bool snapToFromOnEnable = true;
    public bool yieldOneFrameBeforeDelay = true;
    public bool useLocalPositionInsteadOfAnchored = false;
    public bool flipXOnReverse = true;
    public bool invertInitialScaleX = false;

    [Header("Debug")]
    public bool verboseLogs = false;

    private Coroutine routine;
    private bool isRunning;
    private int enableFrame = -1;
    private int runToken;
    private Vector3 baseScale = Vector3.one;
    private float baseScaleXSign = 1f;

    private void OnEnable()
    {
        if (verboseLogs) Log("OnEnable");

        if (!restartOnEnable && isRunning)
        {
            if (verboseLogs) Log("OnEnable ignored (already running)");
            return;
        }

        if (isRunning && routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        StartRoutineOnce();
    }

    private void OnDisable()
    {
        if (verboseLogs) Log("OnDisable");

        if (!stopOnDisable) return;

        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }

        isRunning = false;
    }

    private void StartRoutineOnce()
    {
        if (isRunning && Time.frameCount == enableFrame)
        {
            if (verboseLogs) Log("StartRoutineOnce blocked (same frame)");
            return;
        }

        enableFrame = Time.frameCount;
        isRunning = true;
        runToken++;
        routine = StartCoroutine(MoveLoop(runToken));
    }

    private IEnumerator MoveLoop(int token)
    {
        EnsureTarget();
        if (target == null)
        {
            if (verboseLogs) Log("MoveLoop abort: target is null");
            isRunning = false;
            yield break;
        }

        DebugEnvironment();

        if (yieldOneFrameBeforeDelay)
        {
            if (verboseLogs) Log("Yield 1 frame before delay (layout settle)");
            yield return null;
        }

        if (verboseLogs && Time.unscaledDeltaTime > 0.5f)
        {
            Log("Large unscaledDeltaTime detected (" + Time.unscaledDeltaTime + "). Yielding extra frame.");
            yield return null;
        }

        if (snapToFromOnEnable)
        {
            SetPosition(from);
            if (verboseLogs) Log("Snap to from on enable");
        }

        CacheBaseScale();
        SetScaleFlip(false);

        if (startDelaySeconds > 0f)
        {
            if (verboseLogs) LogStageStart("StartDelay", startDelaySeconds);
            yield return WaitAbsolute(startDelaySeconds, token);
            if (verboseLogs) LogStageEnd("StartDelay");
        }

        while (token == runToken)
        {
            if (verboseLogs) LogStageStart("Move A->B", moveDuration);
            yield return MoveAbsolute(from, to, token);
            if (verboseLogs) LogStageEnd("Move A->B");
            SetScaleFlip(true);

            if (pauseSeconds > 0f)
            {
                if (verboseLogs) LogStageStart("Pause", pauseSeconds);
                yield return WaitAbsolute(pauseSeconds, token);
                if (verboseLogs) LogStageEnd("Pause");
            }

            if (verboseLogs) LogStageStart("Move B->A", moveDuration);
            yield return MoveAbsolute(to, from, token);
            if (verboseLogs) LogStageEnd("Move B->A");
            SetScaleFlip(false);
        }

        isRunning = false;
    }

    private IEnumerator MoveAbsolute(Vector2 a, Vector2 b, int token)
    {
        float duration = SanitizeDuration(moveDuration);
        float start = Now();

        SetPosition(a);

        while (token == runToken)
        {
            float now = Now();
            float p = Mathf.Clamp01((now - start) / duration);
            SetPosition(Vector2.LerpUnclamped(a, b, p));
            if (p >= 1f) break;
            yield return null;
        }

        SetPosition(b);
    }

    private IEnumerator WaitAbsolute(float seconds, int token)
    {
        float duration = SanitizeDuration(seconds);
        float start = Now();

        while (token == runToken)
        {
            float now = Now();
            if (now - start >= duration) break;
            yield return null;
        }
    }

    public void SelfTest(float toleranceSeconds = 0.05f)
    {
        StartCoroutine(SelfTestRoutine(toleranceSeconds));
    }

    private IEnumerator SelfTestRoutine(float toleranceSeconds)
    {
        EnsureTarget();
        if (target == null)
        {
            Log("SelfTest abort: target null");
            yield break;
        }

        float startDelay = Mathf.Max(0f, startDelaySeconds);
        float moveDur = SanitizeDuration(moveDuration);
        float pauseDur = Mathf.Max(0f, pauseSeconds);

        float t0 = Now();
        if (startDelay > 0f) yield return WaitAbsolute(startDelay, runToken);
        float t1 = Now();
        yield return MoveAbsolute(from, to, runToken);
        float t2 = Now();
        if (pauseDur > 0f) yield return WaitAbsolute(pauseDur, runToken);
        float t3 = Now();

        float dDelay = t1 - t0;
        float dMove = t2 - t1;
        float dPause = t3 - t2;

        Log("SelfTest Summary");
        Log("Expected Delay: " + startDelay + " | Actual: " + dDelay);
        Log("Expected Move: " + moveDur + " | Actual: " + dMove);
        Log("Expected Pause: " + pauseDur + " | Actual: " + dPause);

        if (startDelay > 0f && dDelay < Mathf.Max(0.01f, startDelay - toleranceSeconds))
        {
            Log("Delay too short. Possible causes: OnEnable re-run, coroutine restarted, timescale mismatch, layout override.");
        }
    }

    private void EnsureTarget()
    {
        if (target == null) target = GetComponent<RectTransform>();
    }

    private void CacheBaseScale()
    {
        if (target == null) return;
        baseScale = target.localScale;
        baseScaleXSign = baseScale.x >= 0f ? 1f : -1f;
        if (invertInitialScaleX) baseScaleXSign *= -1f;
        baseScale.x = Mathf.Abs(baseScale.x) * baseScaleXSign;
        target.localScale = baseScale;
    }

    private void SetScaleFlip(bool reverse)
    {
        if (!flipXOnReverse || target == null) return;
        var scale = baseScale;
        scale.x = Mathf.Abs(scale.x) * baseScaleXSign * (reverse ? -1f : 1f);
        target.localScale = scale;
    }

    private void SetPosition(Vector2 pos)
    {
        if (target == null) return;
        if (useLocalPositionInsteadOfAnchored)
        {
            target.localPosition = new Vector3(pos.x, pos.y, target.localPosition.z);
        }
        else
        {
            target.anchoredPosition = pos;
        }
    }

    private float Now()
    {
        return useUnscaledTime ? Time.unscaledTime : Time.time;
    }

    private float SanitizeDuration(float value)
    {
        if (float.IsNaN(value) || float.IsInfinity(value)) return 0.01f;
        return Mathf.Max(0.01f, value);
    }

    private void DebugEnvironment()
    {
        Log("Debug Start");
        Log("Scene: " + gameObject.scene.name);
        Log("ActiveInHierarchy: " + gameObject.activeInHierarchy + " | Enabled: " + enabled);
        Log("InstanceID: " + GetInstanceID());
        Log("Frame: " + Time.frameCount + " | Time: " + Time.time + " | Unscaled: " + Time.unscaledTime + " | TimeScale: " + Time.timeScale);
        Log("startDelaySeconds: " + startDelaySeconds + " | pauseSeconds: " + pauseSeconds + " | moveDuration: " + moveDuration + " | useUnscaledTime: " + useUnscaledTime);
        Log("useLocalPositionInsteadOfAnchored: " + useLocalPositionInsteadOfAnchored);

        var siblings = GetComponents<LoopMoveUI>();
        if (siblings.Length > 1)
        {
            Log("Multiple LoopMoveUI on same GameObject: " + siblings.Length);
        }

        var layoutGroup = GetComponentInParent<LayoutGroup>();
        var fitter = GetComponentInParent<ContentSizeFitter>();
        var animator = GetComponentInParent<Animator>();

        if (layoutGroup != null) Log("LayoutGroup found on parent: " + layoutGroup.GetType().Name);
        if (fitter != null) Log("ContentSizeFitter found on parent");
        if (animator != null) Log("Animator found on parent");

        Log("Debug End");
    }

    private void Log(string msg)
    {
        if (!verboseLogs) return;
        Debug.Log("[LoopMoveUI] " + msg, this);
    }

    private float stageStartTime;
    private void LogStageStart(string label, float expected)
    {
        stageStartTime = Now();
        Log(label + " begin | expected " + expected + "s | time: " + Time.time + " | unscaled: " + Time.unscaledTime + " | dt: " + Time.deltaTime + " | udt: " + Time.unscaledDeltaTime);
    }

    private void LogStageEnd(string label)
    {
        float endTime = Now();
        float elapsed = endTime - stageStartTime;
        Log(label + " end | elapsed: " + elapsed + "s | time: " + Time.time + " | unscaled: " + Time.unscaledTime);
    }
}
