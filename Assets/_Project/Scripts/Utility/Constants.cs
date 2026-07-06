namespace TwilightRemnant
{
    /// <summary>
    /// Tên event chuẩn cho EventBus — dùng hằng số này thay vì gõ tay chuỗi
    /// để tránh lỗi gõ sai (typo) không báo lỗi lúc compile.
    /// </summary>
    public static class GameEvents
    {
        public const string OnCONChanged = "OnCONChanged";           // data: float
        public const string OnCONDanger = "OnCONDanger";             // data: null
        public const string OnCONZero = "OnCONZero";                 // data: null
        public const string OnStaminaChanged = "OnStaminaChanged";   // data: float
        public const string OnHPChanged = "OnHPChanged";             // data: float
        public const string OnDayStarted = "OnDayStarted";           // data: int
        public const string OnPhaseChanged = "OnPhaseChanged";       // data: DayPhase
        public const string OnPlayerDied = "OnPlayerDied";           // data: null
        public const string OnLootPickup = "OnLootPickup";           // data: LootPickupData
        public const string OnInventoryChanged = "OnInventoryChanged"; // data: string (itemId vừa đổi)
        public const string OnCoreKeep = "OnCoreKeep";                // data: null
        public const string OnGoldChanged = "OnGoldChanged";         // data: int
        public const string OnRelationshipChanged = "OnRelationshipChanged"; // data: string npcId
        public const string OnStabilizeDone = "OnStabilizeDone";     // data: float quality
        public const string OnCarvingDone = "OnCarvingDone";         // data: bool perfect
        public const string OnFlagChanged = "OnFlagChanged";         // data: string flagId
        public const string OnPointOfNoReturnTriggered = "OnPointOfNoReturnTriggered"; // data: null

        // Giai đoạn F — MinigameUI (Anatomy Site)
        public const string OnStabilizeBegin = "OnStabilizeBegin";           // data: null
        public const string OnStabilizeProgress = "OnStabilizeProgress";     // data: float (0..1)
        public const string OnCarvingBegin = "OnCarvingBegin";               // data: int (tổng số phím QTE)
        public const string OnCarvingKeyExpected = "OnCarvingKeyExpected";   // data: KeyCode
        public const string OnCarvingHit = "OnCarvingHit";                   // data: bool (đúng/sai)
        public const string OnTheChoiceBegin = "OnTheChoiceBegin";           // data: null

        // Anatomy Site — Tier D-C/B-S hazard + Anatomy Insight (Phần 7.3 GDD)
        public const string OnAnatomyHazardHit = "OnAnatomyHazardHit";           // data: float (số dame vừa bị nổ)
        public const string OnAnatomyInsightChanged = "OnAnatomyInsightChanged"; // data: int (số lần đã dọn loài này, TRƯỚC lần vừa xong)
        public const string OnStabilizeZoneChanged = "OnStabilizeZoneChanged";   // data: null (báo UI vẽ lại marker vùng an toàn — Tier D-C/B-S "giật cục")

        // Giai đoạn F — EndingUI
        public const string OnEndingResolved = "OnEndingResolved";           // data: string (tên ending)
    }
}
