using UnityEngine;
using Yarn.Unity;

namespace TwilightRemnant
{
    /// <summary>
    /// Cầu nối giữa file .yarn và hệ thống game thật.
    /// Gọi từ trong .yarn bằng cú pháp: &lt;&lt;set_flag "TênFlag"&gt;&gt;
    /// </summary>
    public class YarnCommands : MonoBehaviour
    {
        [YarnCommand("set_flag")]
        public static void SetFlag(string flagId)
        {
            StoryFlagManager.Instance?.SetFlag(flagId, true);
            Debug.Log("[YarnCommands] set_flag: " + flagId);
        }

        [YarnFunction("has_flag")]
        public static bool HasFlag(string flagId)
        {
            return StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(flagId);
        }

        /// <summary>
        /// Cộng/trừ trực tiếp CON của Ren từ trong file .yarn.
        /// Chèn <<modify_con -15>> (số âm = giảm, số dương = hồi).
        /// Đi qua đúng StatBlock.ModifyCON() nên vẫn Emit OnCONChanged/kiểm tra ngưỡng
        /// như mọi nơi khác trong code — không bypass EventBus.
        /// </summary>
        [YarnCommand("modify_con")]
        public static void ModifyCON(float delta)
        {
            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.stats.ModifyCON(delta);
                Debug.Log("[YarnCommands] modify_con: " + delta);
            }
            else
            {
                Debug.LogWarning("[YarnCommands] modify_con: thiếu PlayerStats.Instance");
            }
        }

        /// <summary>
        /// Ghi lại CON hiện tại vào EndingManager để tính Ending sau này.
        /// Chèn <<snapshot_con>> vào cuối node .yarn của cảnh cuối mỗi hồi.
        /// </summary>
        [YarnCommand("snapshot_con")]
        public static void SnapshotCON()
        {
            if (PlayerStats.Instance != null && EndingManager.Instance != null)
            {
                EndingManager.Instance.SnapshotCON(PlayerStats.Instance.stats.CON);
                Debug.Log("[YarnCommands] snapshot_con: " + PlayerStats.Instance.stats.CON);
            }
            else
            {
                Debug.LogWarning("[YarnCommands] snapshot_con: thiếu PlayerStats.Instance hoặc EndingManager.Instance");
            }
        }
    }
}