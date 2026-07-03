using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Công thức sát thương khởi điểm — tinh chỉnh ở bước 21 (balance pass)
    /// sau khi có bản chơi được.
    /// </summary>
    public static class DamageCalculator
    {
        public static float Calculate(StatBlock attacker, float baseDamage, float targetArmor, float ignoreArmorPct = 0f)
        {
            float effectiveArmor = targetArmor * (1f - ignoreArmorPct);
            float strMultiplier = 1f + attacker.STR * 0.01f;
            float raw = baseDamage * strMultiplier - effectiveArmor;
            return Mathf.Max(raw, baseDamage * 0.1f); // sát thương tối thiểu = 10% base
        }
    }
}
