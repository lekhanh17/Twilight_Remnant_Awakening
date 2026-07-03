using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Định nghĩa 1 sự kiện gắn với 1 ngày + 1 phase cụ thể trong DayCycleManager.
    /// Tạo asset: chuột phải Project window -> Create -> TwilightRemnant -> Day Event Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewDayEvent", menuName = "TwilightRemnant/Day Event Data")]
    public class DayEventData : ScriptableObject
    {
        [Header("Điều kiện kích hoạt")]
        public int dayIndex = 1;
        public string arcId = "Arc1";
        public DayPhase phaseTrigger = DayPhase.Morning;

        [Header("Nội dung")]
        [Tooltip("Tên node trong file .yarn sẽ chạy khi tới đúng ngày/phase này")]
        public string yarnNodeName;

        [Header("Điều kiện phụ (để trống nếu không cần)")]
        [Tooltip("Chỉ chạy event này nếu flag này ĐÃ có (để trống = luôn chạy)")]
        public string requiredFlag;
        [Tooltip("Chỉ chạy event này nếu flag này CHƯA có (để trống = không kiểm tra)")]
        public string blockedByFlag;

        public bool CanTrigger()
        {
            if (!string.IsNullOrEmpty(requiredFlag) && StoryFlagManager.Instance != null
                && !StoryFlagManager.Instance.HasFlag(requiredFlag))
                return false;

            if (!string.IsNullOrEmpty(blockedByFlag) && StoryFlagManager.Instance != null
                && StoryFlagManager.Instance.HasFlag(blockedByFlag))
                return false;

            return true;
        }
    }
}