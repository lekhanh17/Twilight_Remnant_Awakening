using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TwilightRemnant
{
    [System.Serializable]
    public class FlagEntry
    {
        public string key;
        public bool value;
    }

    [System.Serializable]
    public class InsightEntry
    {
        public string speciesId;
        public int count;
    }

    [System.Serializable]
    public class SaveData
    {
        public int currentDay;
        public float conAverage;
        public bool pointOfNoReturnTriggered;
        public List<FlagEntry> storyFlags = new();
        public int gold;
        public List<InventoryEntry> inventoryItems = new();
        /// <summary>Mốc CON đã SnapshotCON() qua các hồi — dùng để EndingManager tính lại đúng
        /// GetWeightedCONAverage() sau khi Load, không chỉ có 1 số trung bình tĩnh.</summary>
        public List<float> conSnapshots = new();

        // Bổ sung: trước đây KHÔNG lưu scene/vị trí/stat sống, khiến Load xong
        // Player về lại CON=100/HP=100 mặc định và đứng vị trí bất kỳ.
        public string sceneName;
        public float posX;
        public float posY;
        public float statSTR, statAGI, statINT, statVIT, statRES, statCON, statHP, statStamina;

        /// <summary>Số lần đã dọn từng loài Vọng Thú — dùng cho Anatomy Insight (Phần 7.3 GDD).</summary>
        public List<InsightEntry> insightData = new();
    }

    /// <summary>
    /// Lưu/tải bằng JsonUtility — đơn giản, đủ dùng cho scope Episode 1.
    /// JsonUtility không serialize Dictionary trực tiếp nên dùng List&lt;FlagEntry&gt;.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, "save1.json");
        private static string StaticSavePath => Path.Combine(Application.persistentDataPath, "save1.json");

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AutoSave()
        {
            var data = BuildSaveData();
            File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
            Debug.Log("[SaveSystem] Đã lưu game.");
        }

        public SaveData Load()
        {
            if (!File.Exists(SavePath))
            {
                Debug.LogWarning("[SaveSystem] Không tìm thấy file save.");
                return null;
            }
            return JsonUtility.FromJson<SaveData>(File.ReadAllText(SavePath));
        }

        /// <summary>Gọi từ MainMenu để biết có hiện nút Load hay không, trước khi SaveSystem.Instance tồn tại.</summary>
        public static bool SaveFileExists() => File.Exists(StaticSavePath);

        private SaveData BuildSaveData()
        {
            var data = new SaveData
            {
                currentDay = DayCycleManager.Instance != null ? DayCycleManager.Instance.currentDay : 1,
                gold = Wallet.Instance != null ? Wallet.Instance.gold : 0,
                conAverage = EndingManager.Instance != null ? EndingManager.Instance.GetWeightedCONAverage() : 100f,
                pointOfNoReturnTriggered = EndingManager.Instance != null && EndingManager.Instance.IsPointOfNoReturnConfirmed,
                storyFlags = StoryFlagManager.Instance != null ? StoryFlagManager.Instance.GetAllFlags() : new List<FlagEntry>(),
                inventoryItems = Inventory.Instance != null ? Inventory.Instance.GetAllItems() : new List<InventoryEntry>(),
                conSnapshots = EndingManager.Instance != null ? EndingManager.Instance.GetConSnapshotsForSave() : new List<float>(),
                sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name,
                insightData = AnatomyInsightTracker.Instance != null ? AnatomyInsightTracker.Instance.GetAllForSave() : new List<InsightEntry>()
            };

            if (PlayerStats.Instance != null)
            {
                var t = PlayerStats.Instance.transform;
                data.posX = t.position.x;
                data.posY = t.position.y;

                var s = PlayerStats.Instance.stats;
                data.statSTR = s.STR;
                data.statAGI = s.AGI;
                data.statINT = s.INT;
                data.statVIT = s.VIT;
                data.statRES = s.RES;
                data.statCON = s.CON;
                data.statHP = s.HP;
                data.statStamina = s.Stamina;
            }

            return data;
        }

        /// <summary>
        /// Áp toàn bộ dữ liệu save vào đúng singleton tương ứng.
        /// Gọi từ GameBootstrapper sau khi Load() — gom 1 chỗ để không rải logic nạp save
        /// ra nhiều nơi khác nhau (khác với cách currentDay/gold đang bị làm rải trước đây).
        /// </summary>
        public static void ApplyLoadedData(SaveData data)
        {
            if (data == null) return;

            if (DayCycleManager.Instance != null)
                DayCycleManager.Instance.currentDay = data.currentDay;

            if (Wallet.Instance != null)
                Wallet.Instance.gold = data.gold;

            if (StoryFlagManager.Instance != null)
                StoryFlagManager.Instance.LoadFlags(data.storyFlags);

            if (Inventory.Instance != null)
                Inventory.Instance.LoadItems(data.inventoryItems);

            if (EndingManager.Instance != null)
                EndingManager.Instance.LoadFromSave(data.conSnapshots, data.pointOfNoReturnTriggered);

            if (AnatomyInsightTracker.Instance != null)
                AnatomyInsightTracker.Instance.LoadFromSave(data.insightData);

            if (PlayerStats.Instance != null)
            {
                var s = PlayerStats.Instance.stats;
                s.STR = data.statSTR;
                s.AGI = data.statAGI;
                s.INT = data.statINT;
                s.VIT = data.statVIT;
                s.RES = data.statRES;
                s.CON = data.statCON;
                s.HP = data.statHP;
                s.Stamina = data.statStamina;

                // Chỉ đặt lại vị trí NGAY nếu đang đúng scene đã lưu — nếu save ở scene
                // khác (vd AnatomySite) mà hiện đang ở Bootstrap, cần chuyển scene trước,
                // xem ghi chú LoadAndGoToSavedScene() bên dưới cho trường hợp đó.
                bool sameScene = string.IsNullOrEmpty(data.sceneName) ||
                                  data.sceneName == UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                if (sameScene)
                    PlayerStats.Instance.transform.position = new Vector3(data.posX, data.posY, PlayerStats.Instance.transform.position.z);
            }

            Debug.Log($"[SaveSystem] Đã áp dụng save đầy đủ: Day={data.currentDay}, Gold={data.gold}, " +
                      $"Flags={data.storyFlags.Count}, Items={data.inventoryItems.Count}, CONSnapshots={data.conSnapshots.Count}, " +
                      $"Insight={data.insightData.Count}, Scene={data.sceneName}, Pos=({data.posX:F1},{data.posY:F1}), " +
                      $"CON={data.statCON:F1}, HP={data.statHP:F1}");
        }

        /// <summary>
        /// Gọi hàm này (thay vì Load()+ApplyLoadedData() thủ công) khi muốn Load xử lý ĐÚNG
        /// cả trường hợp save nằm ở 1 scene khác scene hiện tại — tự chuyển scene trước,
        /// rồi mới áp dữ liệu (kể cả vị trí) sau khi scene đích load xong.
        /// </summary>
        public void LoadAndGoToSavedScene()
        {
            var data = Load();
            if (data == null) return;

            string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(data.sceneName) && data.sceneName != currentScene)
            {
                pendingLoadData = data;
                UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnPendingSceneLoaded;
                UnityEngine.SceneManagement.SceneManager.LoadScene(data.sceneName);
            }
            else
            {
                ApplyLoadedData(data);
            }
        }

        private SaveData pendingLoadData;

        private void OnPendingSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnPendingSceneLoaded;
            ApplyLoadedData(pendingLoadData);
            pendingLoadData = null;
        }
    }
}
