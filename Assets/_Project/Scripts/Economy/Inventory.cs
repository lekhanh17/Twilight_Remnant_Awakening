using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    public struct LootPickupData
    {
        public string itemId;
        public int quantity;
        public LootPickupData(string itemId, int quantity)
        {
            this.itemId = itemId;
            this.quantity = quantity;
        }
    }

    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        private readonly Dictionary<string, int> items = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Add(string itemId, int qty)
        {
            items[itemId] = items.GetValueOrDefault(itemId, 0) + qty;
            EventBus.Emit(GameEvents.OnLootPickup, new LootPickupData(itemId, qty));
        }

        public int GetQuantity(string itemId) => items.GetValueOrDefault(itemId, 0);
    }
}
