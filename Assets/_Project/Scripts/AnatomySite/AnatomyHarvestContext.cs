namespace TwilightRemnant
{
    /// <summary>
    /// Cầu nối tạm giữa Hub_TwilightCity (nơi chọn xác nào để dọn) và scene AnatomySite
    /// (nơi thực sự chơi minigame). Không phải MonoBehaviour, không cần gắn vào
    /// GameObject nào — chỉ là 1 chỗ lưu static tồn tại xuyên suốt lúc chuyển scene
    /// (cùng ý tưởng với cách SceneTransitionManager giữ pendingSpawnId).
    ///
    /// Luồng hoạt động:
    /// 1. AnatomyCorpseSite.cs (đặt tại 1 xác trong Hub) gán PendingSpecies rồi gọi
    ///    SceneTransitionManager.GoToScene("AnatomySite", ...).
    /// 2. AnatomySiteManager.cs đọc PendingSpecies ngay lúc Awake ở scene AnatomySite,
    ///    rồi tự gọi Clear() để không bị dính lại cho lần vào sau.
    /// </summary>
    public static class AnatomyHarvestContext
    {
        public static EchoSpeciesData PendingSpecies { get; private set; }
        public static float PendingHoursSinceDeath { get; private set; }

        public static void SetPending(EchoSpeciesData species, float hoursSinceDeath = 0f)
        {
            PendingSpecies = species;
            PendingHoursSinceDeath = hoursSinceDeath;
        }

        public static void Clear()
        {
            PendingSpecies = null;
            PendingHoursSinceDeath = 0f;
        }
    }
}
