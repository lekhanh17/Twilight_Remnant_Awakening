using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    public class SkillTree : MonoBehaviour
    {
        public List<SkillData> unlockedSkills = new();

        public bool TryUnlock(SkillData skill, StatBlock stats)
        {
            if (stats.RES < skill.resRequired) return false;
            if (!unlockedSkills.Contains(skill))
                unlockedSkills.Add(skill);
            return true;
        }

        public void UseSkill(SkillData skill, StatBlock casterStats)
        {
            if (!unlockedSkills.Contains(skill))
            {
                Debug.LogWarning($"[SkillTree] Chưa unlock skill {skill.displayName}");
                return;
            }
            casterStats.ModifyCON(-skill.conCost);
            SkillEffect.Execute(skill, casterStats);
        }
    }

    /// <summary>Nơi hiện thực hoá hiệu ứng thật của từng skill (VFX, sát thương, buff...).</summary>
    public static class SkillEffect
    {
        public static void Execute(SkillData skill, StatBlock casterStats)
        {
            // TODO: switch theo skill.skillId, gọi đúng logic/VFX cho từng skill cụ thể
            // (Cắt Cộng Hưởng, Xuyên Vết Nứt, Cộng Hưởng Bùng Vỡ, v.v. — Phần 5 GDD).
            Debug.Log($"[SkillEffect] Thực thi {skill.displayName}");
        }
    }
}
