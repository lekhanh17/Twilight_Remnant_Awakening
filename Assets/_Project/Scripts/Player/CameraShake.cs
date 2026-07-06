using System.Collections;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào Main Camera. Hàm Shake() dùng chung cho mọi tình huống cần rung màn hình
    /// (CON nguy hiểm, bị đánh trúng, nổ năng lượng Vọng Thú...). Không tự nghe EventBus —
    /// nơi khác (như CONDangerScreenEffect) gọi trực tiếp Shake() khi cần.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        private Vector3 originalLocalPos;
        private Coroutine currentShake;

        private void Awake()
        {
            originalLocalPos = transform.localPosition;
        }

        public void Shake(float duration, float magnitude)
        {
            if (currentShake != null) StopCoroutine(currentShake);
            currentShake = StartCoroutine(DoShake(duration, magnitude));
        }

        private IEnumerator DoShake(float duration, float magnitude)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                transform.localPosition = originalLocalPos + new Vector3(x, y, 0f);
                elapsed += Time.deltaTime;
                yield return null;
            }
            transform.localPosition = originalLocalPos;
            currentShake = null;
        }
    }
}
