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
        public const string OnCoreKeep = "OnCoreKeep";                // data: null
        public const string OnGoldChanged = "OnGoldChanged";         // data: int
        public const string OnRelationshipChanged = "OnRelationshipChanged"; // data: string npcId
        public const string OnStabilizeDone = "OnStabilizeDone";     // data: float quality
        public const string OnCarvingDone = "OnCarvingDone";         // data: bool perfect
        public const string OnFlagChanged = "OnFlagChanged";         // data: string flagId
    }
}
