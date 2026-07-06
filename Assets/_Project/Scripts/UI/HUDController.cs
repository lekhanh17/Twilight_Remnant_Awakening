using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// HUD hiện CON/HP/Thể Lực real-time. CHỈ nghe EventBus, không tự chứa logic.
    /// </summary>
    public class HUDController : MonoBehaviour
    {
        [Header("HP")]
        [SerializeField] private Slider hpBar;
        [SerializeField] private TextMeshProUGUI hpText;

        [Header("Thể Lực")]
        [SerializeField] private Slider staminaBar;
        [SerializeField] private TextMeshProUGUI staminaText;

        [Header("CON")]
        [SerializeField] private Slider conBar;
        [SerializeField] private TextMeshProUGUI conText;
        [SerializeField] private Image conBarFill;
        [SerializeField] private Color conNormalColor = Color.white;
        [SerializeField] private Color conDangerColor = Color.red;

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnHPChanged, OnHPChanged);
            EventBus.On(GameEvents.OnStaminaChanged, OnStaminaChanged);
            EventBus.On(GameEvents.OnCONChanged, OnCONChanged);
            EventBus.On(GameEvents.OnCONDanger, OnCONDanger);
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnHPChanged, OnHPChanged);
            EventBus.Off(GameEvents.OnStaminaChanged, OnStaminaChanged);
            EventBus.Off(GameEvents.OnCONChanged, OnCONChanged);
            EventBus.Off(GameEvents.OnCONDanger, OnCONDanger);
        }

        private void Start()
        {
            if (PlayerStats.Instance != null)
            {
                RefreshHP(PlayerStats.Instance.stats.HP, PlayerStats.Instance.stats.maxHP);
                RefreshCON(PlayerStats.Instance.stats.CON);
            }
            if (StaminaSystem.Instance != null)
                RefreshStamina(StaminaSystem.Instance.current, StaminaSystem.Instance.max);
        }

        private void OnHPChanged(object data)
        {
            float hp = (float)data;
            float maxHp = PlayerStats.Instance != null ? PlayerStats.Instance.stats.maxHP : 100f;
            RefreshHP(hp, maxHp);
        }

        private void OnStaminaChanged(object data)
        {
            float stamina = (float)data;
            float maxStamina = StaminaSystem.Instance != null ? StaminaSystem.Instance.max : 100f;
            RefreshStamina(stamina, maxStamina);
        }

        private void OnCONChanged(object data)
        {
            float con = (float)data;
            RefreshCON(con);
        }

        private void OnCONDanger(object data)
        {
            if (conBarFill != null)
                conBarFill.color = conDangerColor;
        }

        private void RefreshHP(float hp, float maxHp)
        {
            if (hpBar != null) { hpBar.maxValue = maxHp; hpBar.value = hp; }
            if (hpText != null) hpText.text = $"{hp:F0}/{maxHp:F0}";
        }

        private void RefreshStamina(float stamina, float maxStamina)
        {
            if (staminaBar != null) { staminaBar.maxValue = maxStamina; staminaBar.value = stamina; }
            if (staminaText != null) staminaText.text = $"{stamina:F0}/{maxStamina:F0}";
        }

        private void RefreshCON(float con)
        {
            if (conBar != null) { conBar.maxValue = 100f; conBar.value = con; }
            if (conText != null) conText.text = $"{con:F0}";

            if (conBarFill != null)
                conBarFill.color = con <= 20f ? conDangerColor : conNormalColor;
        }
    }
}