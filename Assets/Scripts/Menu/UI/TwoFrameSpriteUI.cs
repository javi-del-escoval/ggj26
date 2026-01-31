using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TwoFrameSpriteUI : MonoBehaviour
{
    public Image target;
    public Sprite frameA;
    public Sprite frameB;
    public float secondsPerFrame = 0.2f;
    public bool useUnscaledTime = true;

    private Coroutine routine;

    private void OnEnable()
    {
        if (routine == null)
        {
            routine = StartCoroutine(Loop());
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

    private IEnumerator Loop()
    {
        if (target == null) target = GetComponent<Image>();
        if (target == null || frameA == null || frameB == null) yield break;

        while (true)
        {
            target.sprite = frameA;
            yield return Wait(secondsPerFrame);
            target.sprite = frameB;
            yield return Wait(secondsPerFrame);
        }
    }

    private IEnumerator Wait(float seconds)
    {
        float duration = Mathf.Max(0.01f, seconds);
        float t = 0f;
        while (t < duration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            yield return null;
        }
    }
}
