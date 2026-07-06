using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    public class StoryFlagManager : MonoBehaviour
    {
        public static StoryFlagManager Instance { get; private set; }

        private readonly Dictionary<string, bool> flags = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetFlag(string id, bool value = true)
        {
            flags[id] = value;
            EventBus.Emit(GameEvents.OnFlagChanged, id);
        }

        public bool HasFlag(string id) => flags.GetValueOrDefault(id, false);

        /// <summary>Xuất toàn bộ flag hiện có để SaveSystem ghi vào file save.</summary>
        public List<FlagEntry> GetAllFlags()
        {
            var list = new List<FlagEntry>();
            foreach (var kv in flags)
                list.Add(new FlagEntry { key = kv.Key, value = kv.Value });
            return list;
        }

        /// <summary>Nạp lại toàn bộ flag từ file save — gọi từ GameBootstrapper lúc Load.
        /// Xoá sạch flag cũ trước khi nạp để không lẫn dữ liệu của phiên chơi trước.</summary>
        public void LoadFlags(List<FlagEntry> savedFlags)
        {
            flags.Clear();
            if (savedFlags == null) return;
            foreach (var entry in savedFlags)
                flags[entry.key] = entry.value;
        }
    }
}