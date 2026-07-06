using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào bất kỳ GameObject UI nào cần tồn tại xuyên suốt mọi scene
    /// (HUDCanvas, StatPanelCanvas...). Giống PlayerPersistence nhưng dùng
    /// chung cho nhiều loại GameObject khác nhau, không riêng gì Player.
    /// Nếu 1 scene khác có sẵn bản UI riêng cùng loại, bản đó tự huỷ.
    ///
    /// LƯU Ý: mỗi loại UI cần persist (HUD, StatPanel...) dùng 1 static
    /// Dictionary riêng theo "key" để không bị nhầm giữa các loại khác nhau.
    /// </summary>
    public class PersistentUI : MonoBehaviour
    {
        [Tooltip("Đặt tên riêng cho từng loại UI, ví dụ \"HUD\", \"StatPanel\" — " +
                 "để phân biệt, không dùng chung 1 key cho nhiều Canvas khác nhau.")]
        public string persistKey = "HUD";

        private static readonly System.Collections.Generic.Dictionary<string, PersistentUI> instances = new();

        private void Awake()
        {
            if (instances.TryGetValue(persistKey, out var existing) && existing != null && existing != this)
            {
                Destroy(gameObject);
                return;
            }
            instances[persistKey] = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
