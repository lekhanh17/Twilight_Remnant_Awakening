using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Tính Ending theo Phần 8.3a/8.3b GDD:
    /// CON trung bình có trọng số >= 50 VÀ chưa từng chủ động kích hoạt
    /// Điểm Không Quay Đầu -> Kết Thúc Ánh Sáng. Ngược lại -> Kết Thúc Tha Hóa.
    /// </summary>
    public class EndingManager : MonoBehaviour
    {
        public static EndingManager Instance { get; private set; }

        private readonly List<float> conSnapshots = new();
        private bool pointOfNoReturnConfirmed;
        public bool IsPointOfNoReturnConfirmed => pointOfNoReturnConfirmed;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => EventBus.On(GameEvents.OnCONZero, OnCONZeroReceived);
        private void OnDisable() => EventBus.Off(GameEvents.OnCONZero, OnCONZeroReceived);

        private void OnCONZeroReceived(object data) => HandleCONZero();

        /// <summary>Gọi ở các mốc cuối mỗi hồi để ghi nhận CON hiện tại.</summary>
        public void SnapshotCON(float currentCON) => conSnapshots.Add(currentCON);

        /// <summary>Xuất toàn bộ mốc CON đã ghi nhận, để SaveSystem lưu — dùng List&lt;float&gt; nên
        /// JsonUtility serialize được thẳng, không cần wrapper riêng như FlagEntry/InventoryEntry.</summary>
        public List<float> GetConSnapshotsForSave() => new List<float>(conSnapshots);

        /// <summary>Nạp lại mốc CON + trạng thái Điểm Không Quay Đầu từ file save.</summary>
        public void LoadFromSave(List<float> savedSnapshots, bool savedPointOfNoReturnConfirmed)
        {
            conSnapshots.Clear();
            if (savedSnapshots != null)
                conSnapshots.AddRange(savedSnapshots);
            pointOfNoReturnConfirmed = savedPointOfNoReturnConfirmed;
        }

        private void HandleCONZero()
        {
            if (!pointOfNoReturnConfirmed)
            {
                EventBus.Emit(GameEvents.OnPointOfNoReturnTriggered, null);
                Debug.Log("[EndingManager] Đã Emit OnPointOfNoReturnTriggered — chờ popup UI xử lý");
            }
            else
            {
                LockCorruptionRoute();
            }
        }

        public void DeclineCorruption()
        {
            if (PlayerStats.Instance != null)
                PlayerStats.Instance.stats.CON = 5f; // kéo về ngưỡng tối thiểu, 1 cơ hội cuối
        }

        public void ConfirmCorruption()
        {
            pointOfNoReturnConfirmed = true;
            LockCorruptionRoute();
        }

        private void LockCorruptionRoute()
        {
            StoryFlagManager.Instance?.SetFlag("CorruptionRouteLocked", true);
            Debug.Log("[EndingManager] CorruptionRouteLocked = " + (StoryFlagManager.Instance?.HasFlag("CorruptionRouteLocked") ?? false));
        }

        public float GetWeightedCONAverage()
        {
            if (conSnapshots.Count == 0) return 100f;
            float sum = 0f, weightSum = 0f;
            for (int i = 0; i < conSnapshots.Count; i++)
            {
                float weight = 1f + i * 0.5f; // trọng số cao hơn cho mốc gần cuối Hồi 3
                sum += conSnapshots[i] * weight;
                weightSum += weight;
            }
            return weightSum > 0f ? sum / weightSum : 100f;
        }

        public string ResolveEnding()
        {
            bool isLight = GetWeightedCONAverage() >= 50f && !pointOfNoReturnConfirmed;
            return isLight ? "Kết Thúc Ánh Sáng" : "Kết Thúc Tha Hóa";
            // Quan hệ Yuki/Raiko là biến phụ: nếu Ánh Sáng nhưng quan hệ thấp nhất
            // -> vẫn Ánh Sáng, chỉ thiếu 2 NPC ở cảnh đoàn tụ (xử lý ở UI/scene ending).
        }

        /// <summary>Gọi ở cảnh kết thúc game (Arc 3) để tính và công bố ending.</summary>
        public void TriggerEnding()
        {
            string result = ResolveEnding();
            EventBus.Emit(GameEvents.OnEndingResolved, result);
        }
    }
}