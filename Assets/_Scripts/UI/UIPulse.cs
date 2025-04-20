using UnityEngine;
using UnityEngine.UI;

public class UIPulse : MonoBehaviour
{
    [SerializeField] private float pulseScale = 1.2f;
    [SerializeField] private float pulseSpeed = 5f;

    private Vector3 defaultScale;
    private Vector3 targetScale;
    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        defaultScale = rectTransform.localScale;
        targetScale = defaultScale;

        BeatManager.OnBeat += TriggerPulse;
    }

    private void OnDestroy()
    {
        BeatManager.OnBeat -= TriggerPulse;
    }

    private void TriggerPulse()
    {
        // Pulse slightly bigger
        rectTransform.localScale = defaultScale * pulseScale;
        targetScale = defaultScale;
    }

    private void Update()
    {
        // Smoothly shrink back to default scale
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * pulseSpeed);
    }
}
