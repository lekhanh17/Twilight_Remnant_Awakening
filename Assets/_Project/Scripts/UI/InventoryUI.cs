using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// UI cơ bản hiển thị túi đồ + nút "Dùng" cho vật phẩm Consumable.
    /// Chỉ nghe EventBus, gọi Inventory.UseItem() — không tự chứa logic gameplay.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [Header("Toàn bộ ItemData có thể xuất hiện trong túi đồ (để tra tên/icon)")]
        public ItemData[] knownItems;

        [Header("Tham chiếu UI")]
        public Transform itemListContainer;
        public GameObject itemRowPrefab; // dòng: Text tên+số lượng, nút Dùng (ẩn nếu không phải Consumable)

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnInventoryChanged, OnInventoryChanged);
            EventBus.On(GameEvents.OnLootPickup, OnInventoryChanged);
            RefreshList();
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnInventoryChanged, OnInventoryChanged);
            EventBus.Off(GameEvents.OnLootPickup, OnInventoryChanged);
        }

        private void OnInventoryChanged(object data) => RefreshList();

        private void RefreshList()
        {
            if (itemListContainer == null || itemRowPrefab == null || Inventory.Instance == null) return;

            foreach (Transform child in itemListContainer) Destroy(child.gameObject);

            foreach (var item in knownItems)
            {
                if (item == null) continue;
                int qty = Inventory.Instance.GetQuantity(item.itemId);
                if (qty <= 0) continue; // chỉ hiện item đang có

                var row = Instantiate(itemRowPrefab, itemListContainer);
                var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length > 0) texts[0].text = $"{item.displayName} x{qty}";

                var buttons = row.GetComponentsInChildren<Button>();
                if (buttons.Length > 0)
                {
                    buttons[0].gameObject.SetActive(item.itemType == ItemType.Consumable);
                    var captured = item;
                    buttons[0].onClick.AddListener(() => Inventory.Instance.UseItem(captured));
                }
            }
        }
    }
}
