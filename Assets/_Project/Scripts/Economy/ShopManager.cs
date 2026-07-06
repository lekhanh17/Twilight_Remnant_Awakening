using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Logic mua/bán trung tâm — Cửa hàng Aftermath Corp và Chợ đen Cô Tư đều
    /// dùng chung class này, chỉ khác danh sách ItemData và có thể khác giá
    /// (dùng blackMarketPriceMultiplier cho chợ đen, theo GDD Phần 2.2/3.4b).
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>Mua 1 vật phẩm — trừ tiền, cộng vào túi đồ. False nếu không đủ tiền.</summary>
        public bool BuyItem(ItemData item, int quantity = 1)
        {
            if (item == null || item.buyPrice <= 0) return false;
            int totalCost = item.buyPrice * quantity;
            if (Wallet.Instance == null || !Wallet.Instance.Spend(totalCost)) return false;

            Inventory.Instance?.Add(item.itemId, quantity);
            Debug.Log($"[ShopManager] Đã mua {quantity}x {item.displayName}, tốn {totalCost} tiền.");
            return true;
        }

        /// <summary>Bán vật phẩm ra (nộp công ty hoặc chợ đen) — trừ khỏi túi đồ, cộng tiền.
        /// isBlackMarket=true dùng hệ số giá riêng của chợ đen Cô Tư.</summary>
        public bool SellItem(ItemData item, int quantity = 1, bool isBlackMarket = false, float blackMarketPriceMultiplier = 1.3f)
        {
            if (item == null || item.sellPrice <= 0) return false;
            if (Inventory.Instance == null || !Inventory.Instance.Remove(item.itemId, quantity)) return false;

            int unitPrice = isBlackMarket
                ? Mathf.RoundToInt(item.sellPrice * blackMarketPriceMultiplier)
                : item.sellPrice;
            int totalGain = unitPrice * quantity;
            Wallet.Instance?.Add(totalGain);

            if (isBlackMarket)
                BlackMarketFlags.Instance?.RecordSale(item.itemId, quantity);

            Debug.Log($"[ShopManager] Đã bán {quantity}x {item.displayName} ({(isBlackMarket ? "chợ đen" : "công ty")}), nhận {totalGain} tiền.");
            return true;
        }
    }
}
