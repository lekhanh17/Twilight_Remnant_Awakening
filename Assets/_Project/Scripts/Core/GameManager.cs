using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Singleton tồn tại xuyên suốt vòng đời game. Đặt vào scene Bootstrap,
    /// tất cả script khác truy cập qua GameManager.Instance.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Time")]
        [Tooltip("1 giờ trong game trôi qua bao nhiêu giây thật (dùng cho DecayTimer, v.v.)")]
        public float hourPerSecond = 2f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
