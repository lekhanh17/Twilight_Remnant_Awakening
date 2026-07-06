using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào GameObject Player (gốc, cùng cấp PlayerStats/PlayerCombat).
    /// Giữ Player (và toàn bộ con: HandTool, VesselAura...) sống xuyên suốt
    /// khi đổi scene — nếu không có script này, PlayerStats.stats (CON/HP/RES)
    /// sẽ bị Unity huỷ và tạo lại từ đầu (CON reset về 100) mỗi lần LoadScene.
    ///
    /// Nếu 1 scene khác (vd AnatomySite bản test cũ) có đặt sẵn 1 Player riêng,
    /// bản đó sẽ tự huỷ khi phát hiện đã có Player thật persist từ trước.
    /// </summary>
    public class PlayerPersistence : MonoBehaviour
    {
        private static PlayerPersistence instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            // Phải là root GameObject thì DontDestroyOnLoad mới hoạt động đúng —
            // nếu Player đang là con của Bootstrap, dòng này tự tách nó ra root.
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
    }
}
