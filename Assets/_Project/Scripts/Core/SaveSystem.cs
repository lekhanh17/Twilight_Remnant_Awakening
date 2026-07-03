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
    public class SaveData
    {
        public int currentDay;
        public float conAverage;
        public bool pointOfNoReturnTriggered;
        public List<FlagEntry> storyFlags = new();
        public int gold;
        public List<string> inventoryItemIds = new();
    }

    /// <summary>
    /// Lưu/tải bằng JsonUtility — đơn giản, đủ dùng cho scope Episode 1.
    /// JsonUtility không serialize Dictionary trực tiếp nên dùng List&lt;FlagEntry&gt;.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private string SavePath => Path.Combine(Application.persistentDataPath, "save1.json");

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

        private SaveData BuildSaveData()
        {
            var data = new SaveData
            {
                currentDay = DayCycleManager.Instance != null ? DayCycleManager.Instance.currentDay : 1,
                gold = Wallet.Instance != null ? Wallet.Instance.gold : 0
            };
            // TODO: đổ dữ liệu CON trung bình, storyFlags, inventory thật từ các system tương ứng
            // khi những system đó đã hoàn thiện (xem EndingManager, StoryFlagManager, Inventory).
            return data;
        }
    }
}
