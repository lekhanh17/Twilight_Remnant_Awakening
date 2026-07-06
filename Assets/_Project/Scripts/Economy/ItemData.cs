using UnityEngine;

namespace TwilightRemnant
{
    public enum ItemType
    {
        Material,     // nguyên liệu (Lõi, mảnh Vọng Thú...) - bán được, không dùng trực tiếp
        Consumable,    // vật phẩm tiêu hao - dùng để hồi HP/Thể Lực/CON
        QuestItem      // vật phẩm nhiệm vụ - không mua/bán được
    }

    /// <summary>
    /// Định nghĩa 1 loại vật phẩm trong game. Tạo asset: chuột phải trong
    /// Project → Create → TwilightRemnant → Item Data.
    /// </summary>
    [CreateAssetMenu(fileName = "Item_", menuName = "TwilightRemnant/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Định danh")]
        public string itemId;          // dùng để lưu/tải save, PHẢI duy nhất, không trùng loài khác
        public string displayName;
        [TextArea] public string description;
        public Sprite icon;

        [Header("Kinh tế")]
        public ItemType itemType = ItemType.Material;
        public int buyPrice = 0;   // giá mua từ cửa hàng/chợ đen, 0 = không bán cho người chơi
        public int sellPrice = 0;  // giá bán ra (nộp công ty hoặc chợ đen Cô Tư)

        [Header("Hiệu ứng khi dùng (chỉ áp dụng nếu itemType = Consumable)")]
        public float restoreHP = 0f;
        public float restoreStamina = 0f;
        public float restoreCON = 0f;
    }
}
