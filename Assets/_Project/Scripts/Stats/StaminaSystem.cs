using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Thể Lực tối đa 100, KHÔNG hồi qua hoạt động ban ngày/tối — chỉ hồi khi ngủ qua đêm
    /// (Phần 6 GDD). Ca làm 07:00 tốn 5-20/hành động, Vết Sẹo ban đêm tốn 15-30/trận.
    /// </summary>
    public class StaminaSystem : MonoBehaviour
    {
        public static StaminaSystem Instance { get; private set; }

        public float current = 100f;
        public float max = 100f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool TrySpend(float amount)
        {
            if (current < amount) return false;
            current -= amount;
            EventBus.Emit(GameEvents.OnStaminaChanged, current);
            return true;
        }

        public void RestoreFull()
        {
            current = max;
            EventBus.Emit(GameEvents.OnStaminaChanged, current);
        }
    }
}
