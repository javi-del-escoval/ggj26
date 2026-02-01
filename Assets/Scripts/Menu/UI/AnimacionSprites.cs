using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimacionSprites : MonoBehaviour
{
    public Image target;
    public Sprite frameA;
    public Sprite frameB;
    public Sprite[] frames;
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
        if (target == null) yield break;

        var sequence = GetFrameSequence();
        if (sequence == null || sequence.Length == 0) yield break;

        while (true)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                var frame = sequence[i];
                if (frame == null) continue;

                target.sprite = frame;
                yield return Wait(secondsPerFrame);
            }
        }
    }

    private Sprite[] GetFrameSequence()
    {
        if (frames != null && frames.Length > 0)
        {
            return frames;
        }

        if (frameA == null && frameB == null) return null;
        if (frameA != null && frameB != null) return new[] { frameA, frameB };
        if (frameA != null) return new[] { frameA };
        return new[] { frameB };
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
