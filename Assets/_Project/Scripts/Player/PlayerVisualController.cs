using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào GameObject Player. Lắng nghe StoryFlagManager để tự động
    /// chuyển từ "công cụ dọn dẹp" (que gắp) sang "hiệu ứng Tàn Hưởng"
    /// đúng thời điểm Ren thức tỉnh — mốc đó CHÍNH LÀ lúc flag
    /// "Canh04_HoanThanh" được set (Cảnh 04 "Thức Tỉnh" trong cốt truyện),
    /// nên không cần thêm biến bool rời rạc, tái dùng đúng hệ thống flag
    /// đã có sẵn từ Giai đoạn D.
    /// </summary>
    public class PlayerVisualController : MonoBehaviour
    {
        [Header("Tham chiếu")]
        public GameObject handToolObject;      // que gắp Aftermath Corp
        public GameObject tanHuongMarkerObject; // (tuỳ chọn) icon/hiệu ứng tĩnh báo đã thức tỉnh, có thể để trống

        [Header("Cấu hình")]
        public string awakeningFlag = "Canh04_HoanThanh";

        private void OnEnable() => EventBus.On(GameEvents.OnFlagChanged, OnFlagChanged);
        private void OnDisable() => EventBus.Off(GameEvents.OnFlagChanged, OnFlagChanged);

        private void Start()
        {
            // Áp trạng thái đúng ngay khi vào scene — quan trọng cho trường hợp
            // Load save mà flag đã true từ trước (không phải mới set trong phiên này).
            ApplyState(IsAwakened());
        }

        private void OnFlagChanged(object data)
        {
            if (data is not string flagId) return;
            if (flagId != awakeningFlag) return;
            ApplyState(IsAwakened());
        }

        private bool IsAwakened() =>
            StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(awakeningFlag);

        private void ApplyState(bool awakened)
        {
            if (handToolObject != null)
                handToolObject.SetActive(!awakened); // que gắp CHỈ hiện khi CHƯA thức tỉnh

            if (tanHuongMarkerObject != null)
                tanHuongMarkerObject.SetActive(awakened);
        }
    }
}
