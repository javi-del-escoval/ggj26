using UnityEngine;

public class RotacionLoopUI : MonoBehaviour
{
    [Header("Target")]
    public RectTransform target;

    [Header("Loop")]
    public float degreesPerLoop = 360f;
    public float secondsPerLoop = 2f;
    public bool useUnscaledTime = true;

    private float t;

    private void OnEnable()
    {
        t = 0f;
        EnsureTarget();
    }

    private void Update()
    {
        if (target == null) return;

        float duration = Mathf.Max(0.01f, secondsPerLoop);
        float dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        t += dt;
        float p = Mathf.PingPong(t / duration, 1f);
        float angle = degreesPerLoop * p;
        target.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void EnsureTarget()
    {
        if (target == null) target = GetComponent<RectTransform>();
    }
}
