using UnityEngine;

namespace TwilightRemnant
{
    [CreateAssetMenu(fileName = "NewSkill", menuName = "TwilightRemnant/Skill Data")]
    public class SkillData : ScriptableObject
    {
        public string skillId;
        public string displayName;

        [Tooltip("Vanguard / Bastion / Ranger / Healer / Vessel")]
        public string className;

        [Tooltip("F-E / D-C / B+")]
        public string tier = "F-E";

        public float resRequired = 0f;
        public float conCost = 0f;
        public float cooldown = 1f;

        [TextArea] public string description;
    }
}
