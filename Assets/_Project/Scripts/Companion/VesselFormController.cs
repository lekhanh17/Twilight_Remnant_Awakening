using System.Collections;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Khoác Tàn — Biến Hình Tạm Thời. Chi phí -10 CON/lần,
    /// thời lượng tăng theo RES hiện tại (Phần 5 + 7.4 GDD).
    /// </summary>
    public class VesselFormController : MonoBehaviour
    {
        public float durationBase = 15f;
        public bool IsActive { get; private set; }

        [Header("Hiệu ứng hình ảnh (không cần vẽ hình thú thể mới)")]
        public GameObject auraObject;           // Effect_VesselForm_aura, đặt Sorting Order thấp hơn Ren
        public SpriteRenderer playerSpriteRenderer;
        public Color activeTint = new Color(0.85f, 0.7f, 1f); // tím nhẹ khi biến hình

        private Color originalColor = Color.white;

        public void Activate(StatBlock stats)
        {
            if (IsActive) return;
            stats.ModifyCON(-10f);
            float duration = durationBase + stats.RES * 0.1f;
            StartCoroutine(RevertAfter(duration));
        }

        private IEnumerator RevertAfter(float t)
        {
            IsActive = true;
            if (auraObject != null) auraObject.SetActive(true);
            if (playerSpriteRenderer != null)
            {
                originalColor = playerSpriteRenderer.color;
                playerSpriteRenderer.color = activeTint;
            }

            yield return new WaitForSeconds(t);

            IsActive = false;
            if (auraObject != null) auraObject.SetActive(false);
            if (playerSpriteRenderer != null)
                playerSpriteRenderer.color = originalColor;
        }
    }
}
