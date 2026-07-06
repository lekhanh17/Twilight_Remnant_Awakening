using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Đặt 1 GameObject rỗng có script này ở vị trí muốn Player xuất hiện khi
    /// vào scene (ví dụ ngay cửa vào Anatomy Site). Mỗi scene có thể có nhiều
    /// điểm spawn khác nhau, phân biệt bằng spawnId (ví dụ "FromHub", "Default").
    /// SceneTransitionManager sẽ tìm đúng điểm này sau khi load scene xong.
    /// </summary>
    public class ScenePlayerSpawnPoint : MonoBehaviour
    {
        public string spawnId = "Default";

        // Vẽ hình trong Scene view (CHỈ lúc thiết kế trong Editor, KHÔNG hiện lúc
        // chơi game thật) để dễ thấy vị trí spawn khi dựng map.
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.9f, 1f, 0.9f); // xanh cyan, dễ thấy
            Gizmos.DrawWireSphere(transform.position, 0.4f);
            Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.8f);
            Gizmos.DrawWireCube(transform.position + Vector3.up * 0.9f, new Vector3(0.3f, 0.3f, 0.01f));

#if UNITY_EDITOR
            UnityEditor.Handles.color = Color.cyan;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 1.2f, $"Spawn: {spawnId}");
#endif
        }
    }
}
