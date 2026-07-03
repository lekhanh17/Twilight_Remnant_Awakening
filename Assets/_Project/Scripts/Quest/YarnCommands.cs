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
    }
}