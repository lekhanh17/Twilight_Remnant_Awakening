using UnityEngine;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// Bảng 6 chỉ số STR/AGI/INT/VIT/RES/CON — bấm Tab để mở/tắt.
    /// CHỈ nghe EventBus + đọc PlayerStats, không tự chứa logic gameplay.
    /// </summary>
    public class StatPanelUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI strText;
        [SerializeField] private TextMeshProUGUI agiText;
        [SerializeField] private TextMeshProUGUI intText;
        [SerializeField] private TextMeshProUGUI vitText;
        [SerializeField] private TextMeshProUGUI resText;
        [SerializeField] private TextMeshProUGUI conText;

        private bool isOpen;

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("[StatPanelUI] Tab được bấm!");
                TogglePanel();
            }
        }

        private void OnEnable() => EventBus.On(GameEvents.OnCONChanged, OnCONChanged);
        private void OnDisable() => EventBus.Off(GameEvents.OnCONChanged, OnCONChanged);

        private void TogglePanel()
        {
            isOpen = !isOpen;
            if (panel != null)
                panel.SetActive(isOpen);

            if (isOpen)
                RefreshAll();
        }

        private void OnCONChanged(object data)
        {
            if (!isOpen) return;
            float con = (float)data;
            if (conText != null) conText.text = $"CON: {con:F0}";
        }

        private void RefreshAll()
        {
            if (PlayerStats.Instance == null) return;
            var s = PlayerStats.Instance.stats;

            if (strText != null) strText.text = $"STR: {s.STR:F0}";
            if (agiText != null) agiText.text = $"AGI: {s.AGI:F0}";
            if (intText != null) intText.text = $"INT: {s.INT:F0}";
            if (vitText != null) vitText.text = $"VIT: {s.VIT:F0}";
            if (resText != null) resText.text = $"RES: {s.RES:F0}";
            if (conText != null) conText.text = $"CON: {s.CON:F0}";
        }
    }
}