using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>Gắn vào GameObject Player. Nơi duy nhất giữ StatBlock của Ren.</summary>
    public class PlayerStats : MonoBehaviour
    {
        public static PlayerStats Instance { get; private set; }

        public StatBlock stats = new StatBlock();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }
    }
}
