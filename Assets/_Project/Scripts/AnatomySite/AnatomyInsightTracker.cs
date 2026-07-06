using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Đếm số lần Ren đã dọn xong (MakeChoice) từng loài Vọng Thú — dùng cho Anatomy
    /// Insight (Phần 7.3 GDD): loài chưa dọn lần nào = vùng an toàn mờ + 50% rủi ro
    /// hỏng Lõi; loài đã dọn đủ nhiều lần = hiện rõ + cơ hội Điểm Vàng Chí Mạng.
    ///
    /// Singleton, sống qua scene giống các singleton khác trong Core (Wallet,
    /// Inventory, StoryFlagManager...). Cần gắn vào 1 GameObject persist trong
    /// Bootstrap — xem hướng dẫn cuối file giao hàng.
    /// </summary>
    public class AnatomyInsightTracker : MonoBehaviour
    {
        public static AnatomyInsightTracker Instance { get; private set; }

        private readonly Dictionary<string, int> harvestCounts = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public int GetHarvestCount(string speciesId)
        {
            if (string.IsNullOrEmpty(speciesId)) return 0;
            return harvestCounts.GetValueOrDefault(speciesId, 0);
        }

        public void RegisterHarvest(string speciesId)
        {
            if (string.IsNullOrEmpty(speciesId)) return;
            harvestCounts[speciesId] = GetHarvestCount(speciesId) + 1;
        }

        /// <summary>Xuất toàn bộ dữ liệu để SaveSystem ghi vào file save.</summary>
        public List<InsightEntry> GetAllForSave()
        {
            var list = new List<InsightEntry>();
            foreach (var kv in harvestCounts)
                list.Add(new InsightEntry { speciesId = kv.Key, count = kv.Value });
            return list;
        }

        /// <summary>Nạp lại từ file save — gọi từ SaveSystem.ApplyLoadedData().
        /// Xoá dữ liệu cũ trước khi nạp để không lẫn phiên chơi trước.</summary>
        public void LoadFromSave(List<InsightEntry> saved)
        {
            harvestCounts.Clear();
            if (saved == null) return;
            foreach (var entry in saved)
                harvestCounts[entry.speciesId] = entry.count;
        }
    }
}
