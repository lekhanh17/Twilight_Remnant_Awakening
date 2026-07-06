using UnityEngine;
using UnityEngine.AI;

namespace TwilightRemnant
{
    /// <summary>
    /// CONSOLE DEBUG DUY NHẤT — thay thế toàn bộ:
    /// DebugPanel.cs, AnatomyDebugPanel.cs, SkillCompanionDebugPanel.cs,
    /// CompanionPositionLogger.cs, và bản DevTestHooks.cs cũ.
    ///
    /// 1 phím tắt duy nhất (F1), 1 cửa sổ duy nhất, có ScrollView nên không tràn
    /// màn hình dù ở tỉ lệ 16:9 hay màn hình thấp — không còn tình trạng nhiều
    /// panel đè lên nhau như bản cũ (AnatomyDebugPanel vs SkillCompanionDebugPanel
    /// từng đè nhau ở vùng x=340-540 do Rect đặt sai từ Giai đoạn C).
    ///
    /// NGUYÊN TẮC: chức năng nào ĐÃ có UI thật thay thế (Stage 3 Nộp/Giữ dùng nút
    /// thật trong MinigameCanvas, MainMenu Load lúc khởi động) thì KHÔNG lặp lại
    /// ở đây — chỉ giữ đúng phần CHƯA có UI/cơ chế thật: DayCycle (chưa có đồng hồ/
    /// hành động "đi ngủ" thật), trigger Stage1/2 Anatomy Site, Skill Tree, Vessel
    /// Form, Save/Load giữa lúc chơi, Ending test, số liệu HP/Stamina/CON để phục vụ
    /// Giai đoạn I (Playtest & Balance), và log vị trí Companion.
    ///
    /// Đặt ở Assets/ gốc — quy ước debug tạm, xoá khi có đủ UI/cơ chế thật cho
    /// TẤT CẢ mục còn thiếu bên trên.
    /// </summary>
    public class DevConsole : MonoBehaviour
    {
        [Header("Anatomy Site (scene AnatomySite)")]
        public AnatomySiteManager anatomyManager;
        public MinigameStage1 stage1;
        public MinigameStage2 stage2;
        public EchoSpeciesData testSpecies;

        [Header("Skill Tree / Vessel Form (chưa có UI thật)")]
        public SkillTree skillTree;
        public SkillData testSkill;
        public VesselFormController vesselController;

        [Header("Companion (log vị trí — tuỳ chọn, để trống nếu không cần)")]
        public NavMeshAgent companionAgent;

        private bool isVisible = true;
        private bool logCompanionPosition;
        private float companionLogTimer;
        private float conSliderValue = 100f;
        private string dayJumpInput = "1";
        private Vector2 scrollPos;

        private const float WindowY = 20f;
        private const float WindowWidth = 380f;

        private void Start()
        {
            if (PlayerStats.Instance != null)
                conSliderValue = PlayerStats.Instance.stats.CON;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                isVisible = !isVisible;

            if (logCompanionPosition && companionAgent != null)
            {
                companionLogTimer += Time.deltaTime;
                if (companionLogTimer >= 1f)
                {
                    companionLogTimer = 0f;
                    Debug.Log($"[DevConsole] Companion Position={companionAgent.transform.position} | " +
                              $"IsOnNavMesh={companionAgent.isOnNavMesh} | HasPath={companionAgent.hasPath} | " +
                              $"RemainingDistance={companionAgent.remainingDistance} | Destination={companionAgent.destination}");
                }
            }
        }

        private void OnGUI()
        {
            GUI.Label(new Rect(10, Screen.height - 25, 260, 25), "F1 = Dev Console (" + isVisible + ")", GUI.skin.box);
            if (!isVisible) return;

            // QUAN TRỌNG: IMGUI (OnGUI) luôn vẽ ĐÈ LÊN TRÊN mọi Canvas UI khác, bất kể Sort Order
            // (đúng lỗi #8 đã ghi trong bàn giao Giai đoạn F). Vì HUDCanvas neo góc TRÁI-DƯỚI màn
            // hình, đặt cửa sổ này ở góc PHẢI để không bao giờ che mất HUD hoặc StatPanel/Minigame
            // (các panel đó thường neo giữa/trái màn hình).
            float height = Screen.height - 40f;
            float windowX = Screen.width - WindowWidth - 20f;
            var areaRect = new Rect(windowX, WindowY, WindowWidth, height);

            GUILayout.BeginArea(areaRect, "DEV CONSOLE (F1 để ẩn/hiện)", GUI.skin.window);
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(WindowWidth - 10), GUILayout.Height(height - 30));

            DrawDayCycleSection();
            GUILayout.Space(12);
            DrawStatTestSection();
            GUILayout.Space(12);
            DrawAnatomySection();
            GUILayout.Space(12);
            DrawSkillVesselSection();
            DrawShopTestSection();
            GUILayout.Space(12);
            DrawSaveLoadSection();
            GUILayout.Space(12);
            DrawEndingSection();
            GUILayout.Space(12);
            DrawCompanionSection();
            GUILayout.Space(20); // đệm cuối để nút cuối cùng không dính sát viền scroll

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        // ===================== DAY CYCLE =====================
        private void DrawDayCycleSection()
        {
            GUILayout.Label("=== DAY CYCLE ===");
            if (DayCycleManager.Instance == null)
            {
                GUILayout.Label("Thiếu DayCycleManager.Instance (mở scene Bootstrap).");
                return;
            }

            if (GUILayout.Button("Morning")) DayCycleManager.Instance.AdvancePhase(DayPhase.Morning);
            if (GUILayout.Button("WorkShift")) DayCycleManager.Instance.AdvancePhase(DayPhase.WorkShift);
            if (GUILayout.Button("Lunch")) DayCycleManager.Instance.AdvancePhase(DayPhase.Lunch);
            if (GUILayout.Button("TurnIn")) DayCycleManager.Instance.AdvancePhase(DayPhase.TurnIn);
            if (GUILayout.Button("EveningChoice (19:00/21:00)")) DayCycleManager.Instance.AdvancePhase(DayPhase.EveningChoice);
            if (GUILayout.Button("NightScar")) DayCycleManager.Instance.AdvancePhase(DayPhase.NightScar);
            if (GUILayout.Button("Sleep (kết thúc ngày)")) DayCycleManager.Instance.AdvancePhase(DayPhase.Sleep);

            GUILayout.Label($"Ngày: {DayCycleManager.Instance.currentDay} | Phase: {DayCycleManager.Instance.currentPhase}");

            if (GUILayout.Button("Reset khung 19:00/21:00 (gỡ chặn khi nhảy ngày)"))
                DayCycleManager.Instance.ResetEveningSlot();

            // MỚI so với bản cũ: nhảy thẳng tới 1 ngày cụ thể, không cần bấm Sleep lặp lại
            // nhiều lần hoặc sửa tay trong Inspector -> tiện test Cảnh 09-12 (Day16-21).
            GUILayout.BeginHorizontal();
            GUILayout.Label("Nhảy tới ngày:", GUILayout.Width(100));
            dayJumpInput = GUILayout.TextField(dayJumpInput, GUILayout.Width(50));
            if (GUILayout.Button("Áp dụng", GUILayout.Width(80)))
            {
                if (int.TryParse(dayJumpInput, out int targetDay))
                {
                    DayCycleManager.Instance.currentDay = targetDay;
                    // TỰ ĐỘNG reset khung 19:00/21:00 mỗi lần nhảy ngày, vì nhảy ngày
                    // bỏ qua EndDay() (nơi lẽ ra cờ này được reset tự nhiên).
                    // Không cần bấm nút Reset riêng nữa.
                    DayCycleManager.Instance.ResetEveningSlot();
                }
                else
                {
                    Debug.LogWarning("[DevConsole] \"" + dayJumpInput + "\" không phải số hợp lệ.");
                }
            }
            GUILayout.EndHorizontal();
        }

        // ===================== STAMINA / CON / HP =====================
        private void DrawStatTestSection()
        {
            GUILayout.Label("=== STAMINA TEST ===");
            if (StaminaSystem.Instance != null)
            {
                GUILayout.Label($"Hiện tại: {StaminaSystem.Instance.current}/{StaminaSystem.Instance.max}");
                if (GUILayout.Button("Tốn 10 (giả lập hành động)"))
                    Debug.Log("[DevConsole] TrySpend(10) = " + StaminaSystem.Instance.TrySpend(10f));
                if (GUILayout.Button("Tốn 20 Thể Lực"))
                    Debug.Log("[DevConsole] TrySpend(20) = " + StaminaSystem.Instance.TrySpend(20f));
                if (GUILayout.Button("Hồi đầy Thể Lực"))
                    StaminaSystem.Instance.RestoreFull();
            }
            else GUILayout.Label("Thiếu StaminaSystem.Instance.");

            GUILayout.Space(8);
            GUILayout.Label("=== CON TEST ===");
            if (PlayerStats.Instance != null)
            {
                GUILayout.Label($"CON: {PlayerStats.Instance.stats.CON:F1} | {CONThresholds.DescribeState(PlayerStats.Instance.stats.CON)}");
                float newSlider = GUILayout.HorizontalSlider(conSliderValue, 0f, 100f);
                if (!Mathf.Approximately(newSlider, conSliderValue))
                {
                    float delta = newSlider - conSliderValue;
                    conSliderValue = newSlider;
                    PlayerStats.Instance.stats.ModifyCON(delta); // FIX: dòng này thiếu trước đó khiến thanh trượt không đổi CON thật
                    // Log rõ ràng mỗi lần CON bị chỉnh qua thanh trượt này — trước đây
                    // không có dòng log nào, nên lỡ rê/cuộn chuột qua đây sẽ chỉnh CON
                    // ÂM THẦM không để lại dấu vết gì trong Console (nguyên nhân gây bug
                    // "CON tụt không rõ lý do" / "màn hình Ending hiện ngay lúc Play").
                    Debug.Log($"[DevConsole] CON TEST slider kéo: delta={delta:F1} -> CON mới={PlayerStats.Instance.stats.CON:F1}");
                }
                if (GUILayout.Button("Reset CON về 100 (khôi phục nếu lỡ tay)"))
                {
                    float delta = 100f - PlayerStats.Instance.stats.CON;
                    PlayerStats.Instance.stats.ModifyCON(delta);
                    conSliderValue = 100f;
                    Debug.Log("[DevConsole] Đã reset CON về 100.");
                }

                GUILayout.Space(8);
                GUILayout.Label("=== HP TEST ===");
                GUILayout.Label($"HP: {PlayerStats.Instance.stats.HP:F0}/{PlayerStats.Instance.stats.maxHP:F0}");
                if (GUILayout.Button("Mất 20 HP (giả lập bị đánh)"))
                    PlayerStats.Instance.stats.ModifyHP(-20f);
                if (GUILayout.Button("Hồi đầy HP"))
                    PlayerStats.Instance.stats.ModifyHP(PlayerStats.Instance.stats.maxHP);
            }
            else GUILayout.Label("Thiếu PlayerStats.Instance.");
        }

        // ===================== ANATOMY SITE =====================
        private void DrawAnatomySection()
        {
            GUILayout.Label("=== ANATOMY SITE ===");
            if (anatomyManager == null)
            {
                GUILayout.Label("Chưa gán anatomyManager trong Inspector.");
                return;
            }

            GUILayout.Label("Current Stage: " + anatomyManager.currentStage);

            if (GUILayout.Button("Bắt đầu Stage 1 (Ổn Định)"))
            {
                if (stage1 != null) stage1.BeginStage();
                else Debug.LogWarning("[DevConsole] Chưa gán stage1.");
            }
            if (GUILayout.Button("Bắt đầu Stage 2 (Cắt Tách QTE)"))
            {
                if (stage2 != null && testSpecies != null) stage2.BeginStage(testSpecies);
                else Debug.LogWarning("[DevConsole] Chưa gán stage2/testSpecies.");
            }

            GUILayout.Label("Stage 3 (Nộp/Giữ): đã có UI thật trong MinigameCanvas — không lặp lại ở đây.");
            GUILayout.Label("Wallet: " + (Wallet.Instance != null ? Wallet.Instance.gold.ToString() : "N/A"));
            GUILayout.Label("BlackMarket timesKeptCore: " + (BlackMarketFlags.Instance != null ? BlackMarketFlags.Instance.timesKeptCore.ToString() : "N/A"));
        }

        // ===================== SKILL TREE / VESSEL FORM =====================
        private void DrawSkillVesselSection()
        {
            GUILayout.Label("=== SKILL TREE === (chưa có UI thật)");
            if (skillTree != null && testSkill != null && PlayerStats.Instance != null)
            {
                GUILayout.Label($"RES: {PlayerStats.Instance.stats.RES:F1} | CON: {PlayerStats.Instance.stats.CON:F1}");
                if (GUILayout.Button($"Thử Unlock Skill (cần RES >= {testSkill.resRequired})"))
                    Debug.Log("[DevConsole] TryUnlock = " + skillTree.TryUnlock(testSkill, PlayerStats.Instance.stats));
                if (GUILayout.Button($"Dùng Skill (tốn {testSkill.conCost} CON)"))
                    skillTree.UseSkill(testSkill, PlayerStats.Instance.stats);
                GUILayout.Label("Đã unlock: " + skillTree.unlockedSkills.Count + " skill");
            }
            else GUILayout.Label("Chưa gán skillTree/testSkill, hoặc thiếu PlayerStats.Instance.");

            GUILayout.Space(8);
            GUILayout.Label("=== VESSEL FORM === (chưa có UI thật)");
            if (vesselController != null && PlayerStats.Instance != null)
            {
                GUILayout.Label("IsActive: " + vesselController.IsActive);
                if (GUILayout.Button("Activate Vessel Form (-10 CON)"))
                    vesselController.Activate(PlayerStats.Instance.stats);
            }
            else GUILayout.Label("Chưa gán vesselController.");
        }

        // ===================== SHOP / INVENTORY TEST =====================
        [Header("Gán 1 vài ItemData để test mua/bán/dùng")]
        public ItemData[] testItems;

        private void DrawShopTestSection()
        {
            GUILayout.Space(8);
            GUILayout.Label("=== SHOP/INVENTORY TEST ===");
            if (Wallet.Instance == null || Inventory.Instance == null || ShopManager.Instance == null)
            {
                GUILayout.Label("Thiếu Wallet/Inventory/ShopManager.Instance trong scene.");
                return;
            }
            GUILayout.Label($"Tiền: {Wallet.Instance.gold}");
            if (GUILayout.Button("Cộng 100 tiền (test)")) Wallet.Instance.Add(100);

            if (testItems == null || testItems.Length == 0)
            {
                GUILayout.Label("Chưa gán testItems trong Inspector.");
                return;
            }
            foreach (var item in testItems)
            {
                if (item == null) continue;
                int owned = Inventory.Instance.GetQuantity(item.itemId);
                GUILayout.Label($"{item.displayName} — đang có: {owned} | Mua: {item.buyPrice} | Bán: {item.sellPrice}");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Mua 1")) ShopManager.Instance.BuyItem(item, 1);
                if (GUILayout.Button("Bán 1")) ShopManager.Instance.SellItem(item, 1, false);
                if (GUILayout.Button("Bán 1 (chợ đen)")) ShopManager.Instance.SellItem(item, 1, true);
                if (item.itemType == ItemType.Consumable && GUILayout.Button("Dùng"))
                    Inventory.Instance.UseItem(item);
                GUILayout.EndHorizontal();
            }
        }

        // ===================== SAVE / LOAD =====================
        private void DrawSaveLoadSection()
        {
            GUILayout.Label("=== SAVE / LOAD (giữa lúc chơi) ===");
            if (SaveSystem.Instance == null)
            {
                GUILayout.Label("Thiếu SaveSystem.Instance (mở scene Bootstrap).");
                return;
            }
            if (GUILayout.Button("AutoSave"))
                SaveSystem.Instance.AutoSave();
            if (GUILayout.Button("Load Game (áp dụng ngay)"))
            {
                var data = SaveSystem.Instance.Load();
                if (data != null) SaveSystem.ApplyLoadedData(data);
                else Debug.LogWarning("[DevConsole] Không có file save.");
            }
        }

        // ===================== ENDING =====================
        private void DrawEndingSection()
        {
            GUILayout.Label("=== ENDING TEST === (chưa có scene cuối Arc 3 gọi thật)");
            if (EndingManager.Instance == null || PlayerStats.Instance == null)
            {
                GUILayout.Label("Thiếu EndingManager.Instance hoặc PlayerStats.Instance.");
                return;
            }
            if (GUILayout.Button("SnapshotCON (ghi CON hiện tại)"))
            {
                EndingManager.Instance.SnapshotCON(PlayerStats.Instance.stats.CON);
                Debug.Log("[DevConsole] SnapshotCON(" + PlayerStats.Instance.stats.CON.ToString("F1") + ")");
            }
            if (GUILayout.Button("Test ResolveEnding()"))
            {
                float avg = EndingManager.Instance.GetWeightedCONAverage();
                string result = EndingManager.Instance.ResolveEnding();
                Debug.Log("[DevConsole] CON trung bình có trọng số = " + avg.ToString("F1")
                    + " | PointOfNoReturnConfirmed = " + EndingManager.Instance.IsPointOfNoReturnConfirmed
                    + " | ResolveEnding() = " + result);
            }
            if (GUILayout.Button("Trigger Ending Screen"))
                EndingManager.Instance.TriggerEnding();
        }

        // ===================== COMPANION LOG =====================
        private void DrawCompanionSection()
        {
            GUILayout.Label("=== COMPANION LOG ===");
            if (companionAgent == null)
            {
                GUILayout.Label("Chưa gán companionAgent (kéo GameObject Companion có NavMeshAgent).");
                return;
            }
            logCompanionPosition = GUILayout.Toggle(logCompanionPosition, " Log vị trí Companion mỗi giây (xem Console)");
        }
    }
}
