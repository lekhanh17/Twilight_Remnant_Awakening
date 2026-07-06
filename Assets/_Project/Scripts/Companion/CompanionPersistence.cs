using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào GameObject Companion (cùng cấp CompanionAI). Giữ đồng minh sống
    /// xuyên suốt khi đổi scene — nếu không có script này, Companion sẽ bị Unity
    /// huỷ mỗi lần đổi scene (giống Player trước khi có PlayerPersistence).
    ///
    /// ⚠️ LƯU Ý KỸ THUẬT: CompanionAI dùng NavMeshAgent, cần NavMesh đã bake sẵn
    /// trong TỪNG scene mà Companion có thể xuất hiện. Nếu 1 scene chưa bake
    /// NavMesh (ví dụ Bootstrap hiện tại), NavMeshAgent sẽ không di chuyển được
    /// đúng khi Companion persist qua đó — cần bake NavMesh cho scene đó trước.
    /// </summary>
    public class CompanionPersistence : MonoBehaviour
    {
        private static CompanionPersistence instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
