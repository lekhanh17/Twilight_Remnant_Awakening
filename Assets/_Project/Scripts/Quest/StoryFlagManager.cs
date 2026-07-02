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
    }
}
