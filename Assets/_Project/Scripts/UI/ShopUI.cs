using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// UI cơ bản cho Cửa hàng/Chợ đen — CHỈ hiển thị + gọi ShopManager, không tự
    /// chứa logic gameplay (đúng nguyên tắc UI đã chốt từ Giai đoạn F).
    /// Đây là bản placeholder chức năng đầy đủ nhưng giao diện đơn giản (Text/Button
    /// mặc định) — lekhanh17 vẽ lại đẹp hơn sau, giữ nguyên cấu trúc field để không
    /// phải sửa code khi thay UI.
    /// </summary>
    public class ShopUI : MonoBehaviour
    {
        [Header("Danh sách vật phẩm cửa hàng bán ra (Aftermath Corp)")]
        public ItemData[] shopItems;

        [Header("Có phải chợ đen Cô Tư không (ảnh hưởng giá bán)")]
        public bool isBlackMarket = false;

        [Header("Tham chiếu UI")]
        public Transform itemListContainer;   // cha chứa các dòng item (dùng Vertical Layout Group)
        public GameObject itemRowPrefab;       // prefab 1 dòng: Text tên, Text giá, nút Mua, nút Bán
        public TextMeshProUGUI goldText;

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnGoldChanged, OnGoldChanged);
            EventBus.On(GameEvents.OnInventoryChanged, OnInventoryChanged);
            RefreshGold();
            RefreshList();
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnGoldChanged, OnGoldChanged);
            EventBus.Off(GameEvents.OnInventoryChanged, OnInventoryChanged);
        }

        private void OnGoldChanged(object data) => RefreshGold();
        private void OnInventoryChanged(object data) => RefreshList();

        private void RefreshGold()
        {
            if (goldText != null && Wallet.Instance != null)
                goldText.text = $"Tiền: {Wallet.Instance.gold}";
        }

        private void RefreshList()
        {
            if (itemListContainer == null || itemRowPrefab == null) return;

            foreach (Transform child in itemListContainer) Destroy(child.gameObject);

            foreach (var item in shopItems)
            {
                if (item == null) continue;
                var row = Instantiate(itemRowPrefab, itemListContainer);
                var texts = row.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length > 0) texts[0].text = item.displayName;
                if (texts.Length > 1)
                {
                    int owned = Inventory.Instance != null ? Inventory.Instance.GetQuantity(item.itemId) : 0;
                    texts[1].text = $"Mua: {item.buyPrice} | Bán: {item.sellPrice} | Đang có: {owned}";
                }

                var buttons = row.GetComponentsInChildren<Button>();
                if (buttons.Length > 0)
                {
                    var captured = item;
                    buttons[0].onClick.AddListener(() => ShopManager.Instance?.BuyItem(captured, 1));
                }
                if (buttons.Length > 1)
                {
                    var captured = item;
                    buttons[1].onClick.AddListener(() => ShopManager.Instance?.SellItem(captured, 1, isBlackMarket));
                }
            }
        }
    }
}
