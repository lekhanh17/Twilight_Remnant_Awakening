using UnityEngine;

namespace TwilightRemnant
{
    public enum DayPhase { Morning, WorkShift, Lunch, TurnIn, EveningChoice, NightScar, Sleep }

    /// <summary>
    /// Điều khiển flow 1 ngày theo Phần 6 GDD:
    /// 06:00 chuẩn bị, 07:00 ca làm, 12:00 nghỉ trưa, 17:00 nộp/mua sắm,
    /// 19:00 HOẶC 21:00 (loại trừ nhau), 23:00 Vết Sẹo ban đêm (nếu đủ Thể Lực), Ngủ.
    /// </summary>
    public class DayCycleManager : MonoBehaviour
    {
        public static DayCycleManager Instance { get; private set; }

        public int currentDay = 1;
        public DayPhase currentPhase = DayPhase.Morning;

        // 19:00/21:00 loại trừ nhau trong cùng 1 ngày
        private bool eveningSlotUsedToday = false;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool CanEnterEveningChoice() => !eveningSlotUsedToday;

        /// <summary>
        /// Gỡ khoá 19:00/21:00 thủ công — dùng khi test bằng cách nhảy ngày trực tiếp
        /// (không qua Sleep), vì bình thường cờ này chỉ tự reset trong EndDay().
        /// </summary>
        public void ResetEveningSlot() => eveningSlotUsedToday = false;

        public void AdvancePhase(DayPhase next)
        {
            if (next == DayPhase.EveningChoice)
            {
                if (eveningSlotUsedToday)
                {
                    Debug.LogWarning("[DayCycleManager] Đã dùng khung 19:00/21:00 hôm nay rồi.");
                    return;
                }
                eveningSlotUsedToday = true;
            }

            currentPhase = next;
            EventBus.Emit(GameEvents.OnPhaseChanged, next);

            if (next == DayPhase.Sleep)
                EndDay();
        }

        private void EndDay()
        {
            StaminaSystem.Instance?.RestoreFull();
            currentDay++;
            eveningSlotUsedToday = false;
            currentPhase = DayPhase.Morning;
            EventBus.Emit(GameEvents.OnDayStarted, currentDay);
            SaveSystem.Instance?.AutoSave();
        }
    }
}
