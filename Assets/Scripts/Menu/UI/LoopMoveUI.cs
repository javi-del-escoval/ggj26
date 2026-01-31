using System.Collections;
using UnityEngine;

public class LoopMoveUI : MonoBehaviour
{
    public RectTransform target;
    public Vector2 from;
    public Vector2 to;
    public float moveDuration = 2f;
    public float pauseSeconds = 0f;
    public float pauseSecondsOnReturn = 0f;
    public bool useUnscaledTime = true;
    public bool flipXOnReverse = true;

    private Coroutine routine;
    private Vector3 baseScale = Vector3.one;

    private void OnEnable()
    {
        if (routine == null)
        {
            routine = StartCoroutine(MoveLoop());
        }
    }

    private void OnDisable()
    {
        if (routine != null)
        {
            StopCoroutine(routine);
            routine = null;
        }
    }

    private IEnumerator MoveLoop()
    {
        if (target == null) target = GetComponent<RectTransform>();
        if (target == null) yield break;
        baseScale = target.localScale;

        while (true)
        {
            SetScaleFlip(false);
            yield return Move(from, to);
            if (pauseSeconds > 0f) yield return Wait(pauseSeconds);
            SetScaleFlip(true);
            yield return Move(to, from);
            if (pauseSecondsOnReturn > 0f) yield return Wait(pauseSecondsOnReturn);
        }
    }

    private IEnumerator Move(Vector2 a, Vector2 b)
    {
        float t = 0f;
        float duration = Mathf.Max(0.01f, moveDuration);
        target.anchoredPosition = a;

        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float p = Mathf.Clamp01(t / duration);
            target.anchoredPosition = Vector2.LerpUnclamped(a, b, p);
            yield return null;
        }

        target.anchoredPosition = b;
    }

    private IEnumerator Wait(float seconds)
    {
        if (seconds <= 0f) yield break;

        float t = 0f;
        while (t < seconds)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }

    private void SetScaleFlip(bool reverse)
    {
        if (!flipXOnReverse || target == null) return;

        var scale = baseScale;
        scale.x = Mathf.Abs(scale.x) * (reverse ? -1f : 1f);
        target.localScale = scale;
    }
}
