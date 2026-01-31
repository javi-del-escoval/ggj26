using System.Collections;
using UnityEngine;

public class LoopMoveUI : MonoBehaviour
{
    public RectTransform target;
    public Vector2 from;
    public Vector2 to;
    public float moveDuration = 2f;
    public float pauseSeconds = 0f;
    public bool useUnscaledTime = true;

    private Coroutine routine;

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

        while (true)
        {
            yield return Move(from, to);
            if (pauseSeconds > 0f) yield return Wait(pauseSeconds);
            target.anchoredPosition = from;
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
}
