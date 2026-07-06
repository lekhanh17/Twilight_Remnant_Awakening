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

    /// <summary>Dùng để ghi/đọc inventory trong file save (JsonUtility không serialize Dictionary trực tiếp).</summary>
    [System.Serializable]
    public class InventoryEntry
    {
        public string itemId;
        public int quantity;
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
            EventBus.Emit(GameEvents.OnInventoryChanged, itemId);
        }

        /// <summary>Trừ vật phẩm, trả về false nếu không đủ số lượng (không trừ gì cả).</summary>
        public bool Remove(string itemId, int qty)
        {
            int current = items.GetValueOrDefault(itemId, 0);
            if (current < qty) return false;
            int remaining = current - qty;
            if (remaining <= 0) items.Remove(itemId);
            else items[itemId] = remaining;
            EventBus.Emit(GameEvents.OnInventoryChanged, itemId);
            return true;
        }

        /// <summary>Dùng 1 vật phẩm Consumable — trừ khỏi túi đồ + áp hiệu ứng hồi phục lên Player.
        /// Trả về false nếu không có vật phẩm đó hoặc không phải loại Consumable.</summary>
        public bool UseItem(ItemData item)
        {
            if (item == null || item.itemType != ItemType.Consumable) return false;
            if (!Remove(item.itemId, 1)) return false;

            if (PlayerStats.Instance != null)
            {
                var stats = PlayerStats.Instance.stats;
                if (item.restoreHP > 0) stats.ModifyHP(item.restoreHP);
                if (item.restoreStamina > 0) stats.ModifyStamina(item.restoreStamina);
                if (item.restoreCON > 0) stats.ModifyCON(item.restoreCON);
            }

            Debug.Log($"[Inventory] Đã dùng {item.displayName} (+{item.restoreHP} HP, +{item.restoreStamina} Thể Lực, +{item.restoreCON} CON)");
            return true;
        }

        public int GetQuantity(string itemId) => items.GetValueOrDefault(itemId, 0);

        /// <summary>Xuất toàn bộ item hiện có để SaveSystem ghi vào file save.</summary>
        public List<InventoryEntry> GetAllItems()
        {
            var list = new List<InventoryEntry>();
            foreach (var kv in items)
                list.Add(new InventoryEntry { itemId = kv.Key, quantity = kv.Value });
            return list;
        }

        /// <summary>Nạp lại toàn bộ item từ file save — gọi từ GameBootstrapper lúc Load.
        /// Xoá sạch item cũ trước khi nạp để không cộng dồn nhầm với dữ liệu mặc định.</summary>
        public void LoadItems(List<InventoryEntry> savedItems)
        {
            items.Clear();
            if (savedItems == null) return;
            foreach (var entry in savedItems)
                items[entry.itemId] = entry.quantity;
        }
    }
}
