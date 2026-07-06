using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào 1 GameObject có Collider2D (Is Trigger = true) đặt tại cổng vào
    /// Vết Sẹo/Anatomy Site trong map Hub_TwilightCity. Player chạm vào ->
    /// tự chuyển sang scene đích, xuất hiện tại đúng điểm spawn tương ứng.
    /// </summary>
    public class VetSeoEntrance : MonoBehaviour
    {
        [Header("Đích chuyển tới")]
        public string targetScene = "AnatomySite";
        public string targetSpawnId = "FromHub";

        [Header("Yêu cầu Player có Tag đúng")]
        public string playerTag = "Player";

        [Header("Hình ảnh cổng (hiện thật lúc chơi, không chỉ lúc thiết kế)")]
        [Tooltip("Để trống thì tự tìm sprite Icon_VetSeoGate trong Art/Effects lúc vào Play.")]
        public Sprite gateSprite;
        public float gateVisualScale = 1f;

        private void Awake()
        {
            EnsureGateVisual();
        }

        private void EnsureGateVisual()
        {
            // Nếu đã có sẵn con tên "GateVisual" (do đã tạo tay trước đó) thì không tạo lại.
            if (transform.Find("GateVisual") != null) return;

            var sprite = gateSprite;
#if UNITY_EDITOR
            if (sprite == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("Icon_VetSeoGate t:Sprite");
                if (guids.Length > 0)
                {
                    string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                    sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
                }
            }
#endif
            if (sprite == null) return; // không tìm được thì bỏ qua, không lỗi

            var visualObj = new GameObject("GateVisual");
            visualObj.transform.SetParent(transform, false);
            visualObj.transform.localPosition = Vector3.zero;
            visualObj.transform.localScale = Vector3.one * gateVisualScale;
            var sr = visualObj.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 0;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (SceneTransitionManager.Instance == null)
            {
                Debug.LogWarning("[VetSeoEntrance] Chưa có SceneTransitionManager trong scene này.");
                return;
            }
            SceneTransitionManager.Instance.GoToScene(targetScene, targetSpawnId);
        }

        // Vẽ hình cổng trong Scene view lúc THIẾT KẾ (thêm cho dễ thấy khi chưa Play,
        // hình thật ở trên đã hiện cả lúc chơi rồi, đây chỉ là viền phụ trợ khi chọn object).
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(0.7f, 0.3f, 1f, 0.9f);
            Vector3 pos = transform.position;
            Gizmos.DrawWireCube(pos, new Vector3(1f, 1.6f, 0.1f));

#if UNITY_EDITOR
            UnityEditor.Handles.color = new Color(0.7f, 0.3f, 1f);
            UnityEditor.Handles.Label(pos + Vector3.up * 1f, $"Cổng -> {targetScene} ({targetSpawnId})");
#endif
        }
    }
}
