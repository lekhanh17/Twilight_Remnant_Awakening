using UnityEngine;

namespace TwilightRemnant
{
    public class Wallet : MonoBehaviour
    {
        public static Wallet Instance { get; private set; }

        public int gold;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Add(int amount)
        {
            gold += amount;
            EventBus.Emit(GameEvents.OnGoldChanged, gold);
        }

        public bool Spend(int amount)
        {
            if (gold < amount) return false;
            gold -= amount;
            EventBus.Emit(GameEvents.OnGoldChanged, gold);
            return true;
        }
    }
}
