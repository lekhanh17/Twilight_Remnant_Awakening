using UnityEngine;
using UnityEngine.UI;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào HUDCanvas (hoặc GameObject riêng dưới HUDCanvas). Điều khiển viền đỏ
    /// (Effect_CONDanger_vignette) mờ dần rõ dần theo CON hiện tại, và gọi CameraShake
    /// khi CON chạm ngưỡng nguy hiểm/CON=0. CHỈ nghe EventBus, không tự chứa logic gameplay
    /// (đúng nguyên tắc UI đã chốt từ Giai đoạn F).
    /// </summary>
    public class CONDangerScreenEffect : MonoBehaviour
    {
        [Header("Tham chiếu")]
        public Image vignetteImage;       // Effect_CONDanger_vignette, Image full-screen (stretch anchor)
        public CameraShake cameraShake;   // component trên Main Camera

        [Header("Cấu hình")]
        public float dangerThreshold = 20f;   // khớp đúng ngưỡng trong CONThresholds.cs
        [Range(0f, 1f)] public float maxVignetteAlpha = 0.85f;
        public float dangerShakeDuration = 0.3f;
        public float dangerShakeMagnitude = 0.08f;
        public float zeroShakeDuration = 0.6f;
        public float zeroShakeMagnitude = 0.15f;

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnCONChanged, OnCONChanged);
            EventBus.On(GameEvents.OnCONDanger, OnCONDanger);
            EventBus.On(GameEvents.OnCONZero, OnCONZero);
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnCONChanged, OnCONChanged);
            EventBus.Off(GameEvents.OnCONDanger, OnCONDanger);
            EventBus.Off(GameEvents.OnCONZero, OnCONZero);
        }

        private void Start()
        {
            if (PlayerStats.Instance != null)
                UpdateVignette(PlayerStats.Instance.stats.CON);
        }

        private void OnCONChanged(object data)
        {
            UpdateVignette((float)data);
        }

        private void UpdateVignette(float con)
        {
            if (vignetteImage == null) return;
            float t = con >= dangerThreshold ? 0f : 1f - (con / dangerThreshold);
            var c = vignetteImage.color;
            c.a = Mathf.Clamp01(t) * maxVignetteAlpha;
            vignetteImage.color = c;
        }

        private void OnCONDanger(object data)
        {
            if (cameraShake != null)
                cameraShake.Shake(dangerShakeDuration, dangerShakeMagnitude);
        }

        private void OnCONZero(object data)
        {
            if (cameraShake != null)
                cameraShake.Shake(zeroShakeDuration, zeroShakeMagnitude);
        }
    }
}
