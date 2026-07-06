using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Điều phối quy trình thu hoạch 3 giai đoạn (Phần 7.2 GDD):
    /// Ổn Định -> Cắt Tách -> Quyết Định (Nộp công ty / Giữ lại).
    ///
    /// Anatomy Insight (Phần 7.3 GDD): loài chưa từng dọn có 50% rủi ro Lõi bị hỏng
    /// khi hoàn thành Carving (giảm nửa phần thưởng); loài đã dọn đủ nhiều lần
    /// (>= masteredThreshold) + Lõi Hoàn Hảo có cơ hội "Điểm Vàng Chí Mạng"
    /// (thưởng thêm 50%, mở flag Ký Ức Vọng Thú riêng theo loài).
    ///
    /// Merge Hub: nếu có AnatomyHarvestContext.PendingSpecies (được AnatomyCorpseSite
    /// gán từ Hub trước khi chuyển scene), tự lấy species đó thay vì dùng field
    /// species gán tay trong Inspector — cho phép nhiều điểm thu hoạch khác species
    /// dùng chung 1 scene AnatomySite, không cần tạo nhiều scene riêng.
    /// </summary>
    public class AnatomySiteManager : MonoBehaviour
    {
        public enum Stage { Stabilize, Carving, TheChoice, Done }

        [Tooltip("Species mặc định nếu không có AnatomyHarvestContext.PendingSpecies nào được gán từ Hub (dùng khi test trực tiếp trong scene AnatomySite, không qua Hub).")]
        public EchoSpeciesData species;
        public DecayTimer decay;
        public Stage currentStage = Stage.Stabilize;

        [Header("Anatomy Insight (Phần 7.3 GDD)")]
        [Tooltip("Từ bao nhiêu lần dọn trở lên thì tính là 'đã thành thạo' loài này.")]
        public int masteredThreshold = 3;
        [Range(0f, 1f)] public float unseenCorruptionChance = 0.5f;

        /// <summary>true nếu Lõi vừa lấy bị hỏng do chưa quen loài (Anatomy Insight).</summary>
        public bool CoreCorrupted { get; private set; }
        /// <summary>true nếu vừa trúng Điểm Vàng Chí Mạng (Lõi Hoàn Hảo + đã thành thạo loài).</summary>
        public bool GoldenPointHit { get; private set; }

        private void Awake()
        {
            if (AnatomyHarvestContext.PendingSpecies != null)
            {
                species = AnatomyHarvestContext.PendingSpecies;
                if (decay != null)
                    decay.hoursSinceDeath = AnatomyHarvestContext.PendingHoursSinceDeath;
                AnatomyHarvestContext.Clear();
            }
        }

        public void CompleteStabilize(float qualityScore)
        {
            currentStage = Stage.Carving;
            EventBus.Emit(GameEvents.OnStabilizeDone, qualityScore);
        }

        public void CompleteCarving(bool perfect)
        {
            currentStage = Stage.TheChoice;
            ResolveAnatomyInsight(perfect);
            EventBus.Emit(GameEvents.OnCarvingDone, perfect);
            EventBus.Emit(GameEvents.OnTheChoiceBegin);
        }

        /// <summary>Áp dụng Anatomy Insight (Phần 7.3 GDD) ngay sau khi Carving xong,
        /// TRƯỚC khi cộng dồn lượt dọn mới (RegisterHarvest chỉ gọi trong MakeChoice).</summary>
        private void ResolveAnatomyInsight(bool perfectCarving)
        {
            CoreCorrupted = false;
            GoldenPointHit = false;

            if (species == null) return;

            int timesHarvested = AnatomyInsightTracker.Instance != null
                ? AnatomyInsightTracker.Instance.GetHarvestCount(species.speciesId)
                : 0;

            if (timesHarvested == 0)
            {
                // Loài chưa từng dọn: vùng an toàn mờ (UI tự đọc OnAnatomyInsightChanged
                // để hiển thị), rủi ro hỏng Lõi 50%.
                if (Random.value < unseenCorruptionChance)
                    CoreCorrupted = true;
            }
            else if (timesHarvested >= masteredThreshold && perfectCarving)
            {
                GoldenPointHit = true;
            }

            EventBus.Emit(GameEvents.OnAnatomyInsightChanged, timesHarvested);
        }

        /// <summary>true = Giữ lại (RES tăng, CON giảm, tăng flag chợ đen) | false = Nộp công ty (tiền).</summary>
        public void MakeChoice(bool keepCore)
        {
            var stats = PlayerStats.Instance?.stats;

            float resGain = species.resGain;
            int sellPrice = species.sellPrice;

            if (CoreCorrupted)
            {
                // Lõi hỏng: phần thưởng giảm còn 1 nửa.
                resGain *= 0.5f;
                sellPrice = Mathf.RoundToInt(sellPrice * 0.5f);
            }
            else if (GoldenPointHit)
            {
                // Điểm Vàng Chí Mạng: thưởng thêm 50%, mở Ký Ức Vọng Thú riêng theo loài.
                resGain *= 1.5f;
                sellPrice = Mathf.RoundToInt(sellPrice * 1.5f);
                StoryFlagManager.Instance?.SetFlag($"KyUcVongThu_{species.speciesId}", true);
            }

            if (keepCore)
            {
                if (stats != null)
                {
                    stats.RES += resGain;
                    stats.ModifyCON(-species.conCostOnKeep);
                }
                BlackMarketFlags.Instance?.RegisterCoreKept();
                EventBus.Emit(GameEvents.OnCoreKeep);
            }
            else
            {
                Wallet.Instance?.Add(sellPrice);
            }

            AnatomyInsightTracker.Instance?.RegisterHarvest(species.speciesId);
            currentStage = Stage.Done;
        }
    }
}
