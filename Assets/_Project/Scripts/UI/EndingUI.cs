using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// Màn hình kết thúc game. CHỈ nghe EventBus, không tự chứa logic gameplay.
    /// </summary>
    public class EndingUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI subtitleText;
        [SerializeField] private Image backgroundTint;
        [SerializeField] private Color lightEndingColor = new Color(1f, 0.95f, 0.7f, 1f);
        [SerializeField] private Color corruptionEndingColor = new Color(0.25f, 0.05f, 0.1f, 1f);

        private void Awake()
        {
            if (panel != null)
                panel.SetActive(false);

            // FIX: backgroundTint là object RIÊNG, không phải con của panel, nên phải tự tắt
            // ở đây — nếu không nó sẽ luôn Active = true, màu đen phủ kín toàn màn hình ngay
            // từ frame đầu tiên mỗi lần Play, che hết mọi UI khác (HUD, DevConsole nằm dưới nó
            // theo Canvas Sort Order, chỉ có IMGUI mới vẽ đè lên trên được).
            if (backgroundTint != null)
                backgroundTint.gameObject.SetActive(false);
        }

        private void OnEnable() => EventBus.On(GameEvents.OnEndingResolved, OnEndingResolved);
        private void OnDisable() => EventBus.Off(GameEvents.OnEndingResolved, OnEndingResolved);

        private void OnEndingResolved(object data)
        {
            string ending = (string)data;
            if (panel != null) panel.SetActive(true);
            if (titleText != null) titleText.text = ending;

            bool isLight = ending == "Kết Thúc Ánh Sáng";

            if (subtitleText != null)
            {
                subtitleText.text = isLight
                    ? "Ren giữ được chính mình cho đến cuối cùng."
                    : "Ranh giới đã bị vượt qua. Không còn đường lùi.";
            }

            if (backgroundTint != null)
            {
                backgroundTint.gameObject.SetActive(true);
                backgroundTint.color = isLight ? lightEndingColor : corruptionEndingColor;
            }
        }
    }
}