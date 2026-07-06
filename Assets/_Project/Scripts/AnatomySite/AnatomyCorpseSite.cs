using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào 1 GameObject có Collider2D (Is Trigger = true) đặt tại vị trí 1 xác
    /// Vọng Thú trong map Hub_TwilightCity/ScarZone. Player chạm vào -> gán species
    /// này làm mục tiêu thu hoạch (qua AnatomyHarvestContext) rồi chuyển sang scene
    /// AnatomySite.
    ///
    /// Đây là phần "merge AnatomySite vào luồng chơi thật" — CHƯA cần map vẽ xong.
    /// Chỉ cần 1 GameObject placeholder có Collider2D là test được luôn. Sau này
    /// lekhanh17 vẽ xong map thật, chỉ cần DI CHUYỂN GameObject này (đã có sẵn
    /// component + species gán rồi) tới đúng vị trí trên map, không cần code lại gì.
    /// </summary>
    public class AnatomyCorpseSite : MonoBehaviour
    {
        [Header("Loài Vọng Thú tại điểm này")]
        public EchoSpeciesData species;

        [Tooltip("Xác đã chết bao nhiêu giờ khi Player tới nơi (ảnh hưởng DecayStage — Phần 7.1 GDD). 0 = xác còn nóng.")]
        public float hoursSinceDeath = 0f;

        [Header("Đích chuyển tới")]
        public string targetScene = "AnatomySite";
        public string targetSpawnId = "FromHub";

        [Header("Yêu cầu Player có Tag đúng")]
        public string playerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag(playerTag)) return;

            if (species == null)
            {
                Debug.LogWarning($"[AnatomyCorpseSite] '{gameObject.name}' chưa gán species, huỷ chuyển scene.");
                return;
            }
            if (SceneTransitionManager.Instance == null)
            {
                Debug.LogWarning("[AnatomyCorpseSite] Chưa có SceneTransitionManager trong scene này.");
                return;
            }

            AnatomyHarvestContext.SetPending(species, hoursSinceDeath);
            SceneTransitionManager.Instance.GoToScene(targetScene, targetSpawnId);
        }

        // Vẽ gợi ý trong Scene view lúc thiết kế, không hiện lúc chơi thật.
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.9f);
            Gizmos.DrawWireSphere(transform.position, 0.6f);
#if UNITY_EDITOR
            string label = species != null
                ? $"Xác: {species.displayName} (Tier {species.tier})"
                : "Xác: CHƯA GÁN SPECIES";
            UnityEditor.Handles.color = new Color(1f, 0.5f, 0f);
            UnityEditor.Handles.Label(transform.position + Vector3.up * 0.8f, label);
#endif
        }
    }
}
