using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// 6 chỉ số theo Phần 3.5 GDD: STR, AGI, INT, VIT, RES, CON.
    /// CON có ngưỡng hành vi đặc biệt — xem CONThresholds.cs.
    /// </summary>
    [System.Serializable]
    public class StatBlock
    {
        public float STR = 10f;
        public float AGI = 10f;
        public float INT = 10f;
        public float VIT = 10f;
        public float RES = 0f;
        public float CON = 100f;

        public float HP = 100f;
        public float maxHP = 100f;
        public float Stamina = 100f;
        public float maxStamina = 100f;

        public void ModifyCON(float delta)
        {
            float old = CON;
            CON = Mathf.Clamp(CON + delta, 0f, 100f);
            EventBus.Emit(GameEvents.OnCONChanged, CON);
            CONThresholds.CheckThreshold(old, CON);
        }

        public void ModifyHP(float delta)
        {
            HP = Mathf.Clamp(HP + delta, 0f, maxHP);
            EventBus.Emit(GameEvents.OnHPChanged, HP);
            if (HP <= 0f)
                EventBus.Emit(GameEvents.OnPlayerDied);
        }

        public void ModifyStamina(float delta)
        {
            Stamina = Mathf.Clamp(Stamina + delta, 0f, maxStamina);
            EventBus.Emit(GameEvents.OnStaminaChanged, Stamina);
        }
    }
}
