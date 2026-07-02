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
            // TODO: đổi sprite/animator sang hình dạng Vọng Thú tại đây
            yield return new WaitForSeconds(t);
            IsActive = false;
            // TODO: trở lại hình dạng gốc tại đây
        }
    }
}
