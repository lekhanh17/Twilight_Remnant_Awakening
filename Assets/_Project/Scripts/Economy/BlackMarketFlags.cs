using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Theo dõi số lần người chơi chọn "Giữ lại" Lõi (Phần 7.2 GDD).
    /// Dùng để quyết định biến thể A/B khi CÔ TƯ gửi tin nhắn ẩn danh cuối Arc 1
    /// (Phần 3.4b, GDD bản 3.1).
    /// </summary>
    public class BlackMarketFlags : MonoBehaviour
    {
        public static BlackMarketFlags Instance { get; private set; }

        public int timesKeptCore = 0;
        public int totalBlackMarketSales = 0;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegisterCoreKept() => timesKeptCore++;

        /// <summary>Ghi nhận 1 lần bán ra chợ đen Cô Tư — dùng để mở khoá nội dung
        /// liên quan hợp tác Cô Tư ở Hồi 2 (Phần 3.4b GDD).</summary>
        public void RecordSale(string itemId, int quantity) => totalBlackMarketSales += quantity;

        /// <summary>true = dùng Biến thể A (đã từng giữ Lõi), false = Biến thể B (chưa từng giữ).</summary>
        public bool ShouldUseVariantA => timesKeptCore >= 1;
    }
}
