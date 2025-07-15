using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class PeriodicSinkRise : MonoBehaviour
{
    [Header("Timing (seconds)")]
    [Tooltip("UpStayTime")]
    public float stayDuration = 4f;
    [Tooltip("SinkingDuration")]
    public float sinkDuration = 3f;
    [Tooltip("BottomStayTime")]
    public float bottomPauseTime = 3f;
    [Tooltip("RiseDuration")]
    public float riseDuration = 2f;

    [Header("Motion")]
    [Tooltip("SinkingDepth")]
    public float sinkDepth = 2.7f;

    [Header("Audio (可选：局部 AudioSource 播放)")]
    [SerializeField] private AudioSource localSource;       // 在 Inspector 拖入
    [SerializeField] private AudioClip deliverySuccessClip; // 在 Inspector 拖入 delivery_success.wav

    private Vector3 _startLocalPos;

    void Awake()
    {
        // 记录初始位置
        _startLocalPos = transform.localPosition;
    }

    void OnEnable()
    {
        StartCoroutine(SinkPauseRiseLoop());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SinkPauseRiseLoop()
    {
        while (true)
        {
            // 1）顶部停留
            yield return new WaitForSeconds(stayDuration);

            // —— 在开始下沉前播放“delivery success”音效 —— 
            if (localSource != null && deliverySuccessClip != null)
            {
                localSource.PlayOneShot(deliverySuccessClip);
            }

            // 2）下沉
            yield return LerpPosition(
                _startLocalPos,
                _startLocalPos + Vector3.down * sinkDepth,
                sinkDuration
            );

            // 3）底部再停留
            yield return new WaitForSeconds(bottomPauseTime);

            // 4）上浮
            yield return LerpPosition(
                _startLocalPos + Vector3.down * sinkDepth,
                _startLocalPos,
                riseDuration
            );
        }
    }

    private IEnumerator LerpPosition(Vector3 from, Vector3 to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localPosition = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // 确保最终到位
        transform.localPosition = to;
    }
}
