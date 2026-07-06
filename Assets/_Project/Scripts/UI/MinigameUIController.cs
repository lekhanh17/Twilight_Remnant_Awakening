using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TwilightRemnant
{
    /// <summary>
    /// Quản lý UI cho 3 giai đoạn Anatomy Site. CHỈ nghe EventBus, không tự chứa logic gameplay.
    /// </summary>
    public class MinigameUIController : MonoBehaviour
    {
        [Header("Stage 1 - Stabilize")]
        [SerializeField] private GameObject stage1Panel;
        [SerializeField] private Slider stage1ProgressBar;
        [SerializeField] private Image stage1SafeZoneMarker;
        [SerializeField] private MinigameStage1 stage1Script;
        [SerializeField] private RectTransform stage1CoreIcon; // Icon_AnatomyCore, đặt cạnh/phía sau progress bar

        [Header("Stage 2 - Carving")]
        [SerializeField] private GameObject stage2Panel;
        [SerializeField] private TextMeshProUGUI stage2ExpectedKeyText;
        [SerializeField] private TextMeshProUGUI stage2ProgressText;
        [SerializeField] private Image stage2FeedbackFlash;
        [SerializeField] private Image stage2CarvingFrame; // Icon_CarvingFrame, đặt phía sau stage2ExpectedKeyText
        [SerializeField] private Color feedbackHitColor = Color.green;
        [SerializeField] private Color feedbackMissColor = Color.red;

        [Header("Stage 3 - The Choice")]
        [SerializeField] private GameObject stage3Panel;

        private int carvingTotalKeys;
        private int carvingKeysDone;

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnStabilizeBegin, OnStabilizeBegin);
            EventBus.On(GameEvents.OnStabilizeProgress, OnStabilizeProgress);
            EventBus.On(GameEvents.OnStabilizeDone, OnStabilizeDone);
            EventBus.On(GameEvents.OnStabilizeZoneChanged, OnStabilizeZoneChanged);

            EventBus.On(GameEvents.OnCarvingBegin, OnCarvingBegin);
            EventBus.On(GameEvents.OnCarvingKeyExpected, OnCarvingKeyExpected);
            EventBus.On(GameEvents.OnCarvingHit, OnCarvingHit);
            EventBus.On(GameEvents.OnCarvingDone, OnCarvingDone);

            EventBus.On(GameEvents.OnTheChoiceBegin, OnTheChoiceBegin);
            EventBus.On(GameEvents.OnCoreKeep, OnChoiceMade);
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnStabilizeBegin, OnStabilizeBegin);
            EventBus.Off(GameEvents.OnStabilizeProgress, OnStabilizeProgress);
            EventBus.Off(GameEvents.OnStabilizeDone, OnStabilizeDone);
            EventBus.Off(GameEvents.OnStabilizeZoneChanged, OnStabilizeZoneChanged);

            EventBus.Off(GameEvents.OnCarvingBegin, OnCarvingBegin);
            EventBus.Off(GameEvents.OnCarvingKeyExpected, OnCarvingKeyExpected);
            EventBus.Off(GameEvents.OnCarvingHit, OnCarvingHit);
            EventBus.Off(GameEvents.OnCarvingDone, OnCarvingDone);

            EventBus.Off(GameEvents.OnTheChoiceBegin, OnTheChoiceBegin);
            EventBus.Off(GameEvents.OnCoreKeep, OnChoiceMade);
        }

        private void Awake()
        {
            if (stage1Panel != null) stage1Panel.SetActive(false);
            if (stage2Panel != null) stage2Panel.SetActive(false);
            if (stage3Panel != null) stage3Panel.SetActive(false);
        }

        // ===== STAGE 1 =====

        private void OnStabilizeBegin(object data)
        {
            if (stage1Panel != null) stage1Panel.SetActive(true);
            if (stage1ProgressBar != null) stage1ProgressBar.value = 0f;

            RepositionSafeZoneMarker();
        }

        /// <summary>Vẽ lại khung marker theo đúng safeZoneMin/Max HIỆN TẠI của MinigameStage1.
        /// Gọi lúc bắt đầu Stage 1, VÀ mỗi lần Tier D-C/B-S "giật cục" đổi vị trí vùng an toàn
        /// (qua event OnStabilizeZoneChanged) — nếu không, marker sẽ đứng yên sai vị trí
        /// trong khi vùng an toàn thật đã đổi chỗ ngầm bên dưới.</summary>
        private void RepositionSafeZoneMarker()
        {
            if (stage1SafeZoneMarker == null || stage1Script == null) return;

            var rt = stage1SafeZoneMarker.rectTransform;
            var parentRt = rt.parent as RectTransform;
            if (parentRt == null) return;

            float zoneWidth = (stage1Script.safeZoneMax - stage1Script.safeZoneMin) * parentRt.rect.width;
            float zoneCenterX = (stage1Script.safeZoneMin + stage1Script.safeZoneMax) / 2f * parentRt.rect.width - parentRt.rect.width / 2f;
            rt.sizeDelta = new Vector2(zoneWidth, rt.sizeDelta.y);
            rt.anchoredPosition = new Vector2(zoneCenterX, rt.anchoredPosition.y);
        }

        private void OnStabilizeZoneChanged(object data)
        {
            RepositionSafeZoneMarker();
        }

        private void OnStabilizeProgress(object data)
        {
            float progress01 = (float)data;
            if (stage1ProgressBar != null) stage1ProgressBar.value = progress01;

            // Core icon "đập" nhẹ theo tiến độ - càng gần xong càng đập nhanh/rõ,
            // trực quan hoá việc Lõi đang dần được khống chế (Icon_AnatomyCore).
            if (stage1CoreIcon != null)
            {
                float pulse = 1f + Mathf.Sin(Time.time * (4f + progress01 * 6f)) * 0.06f;
                stage1CoreIcon.localScale = new Vector3(pulse, pulse, 1f);
            }
        }

        private void OnStabilizeDone(object data)
        {
            if (stage1Panel != null) stage1Panel.SetActive(false);
        }

        // ===== STAGE 2 =====

        private void OnCarvingBegin(object data)
        {
            carvingTotalKeys = (int)data;
            carvingKeysDone = 0;
            if (stage2Panel != null) stage2Panel.SetActive(true);
            UpdateCarvingProgressText();
        }

        private void OnCarvingKeyExpected(object data)
        {
            KeyCode key = (KeyCode)data;
            if (stage2ExpectedKeyText != null)
                stage2ExpectedKeyText.text = key.ToString();
        }

        private void OnCarvingHit(object data)
        {
            bool hit = (bool)data;
            carvingKeysDone++;
            UpdateCarvingProgressText();

            if (stage2FeedbackFlash != null)
            {
                stage2FeedbackFlash.color = hit ? feedbackHitColor : feedbackMissColor;
                StartCoroutine(ClearFeedbackFlash());
            }
        }

        private System.Collections.IEnumerator ClearFeedbackFlash()
        {
            yield return new WaitForSeconds(0.15f);
            if (stage2FeedbackFlash != null)
                stage2FeedbackFlash.color = new Color(1f, 1f, 1f, 0f);
        }

        private void UpdateCarvingProgressText()
        {
            if (stage2ProgressText != null)
                stage2ProgressText.text = $"{carvingKeysDone}/{carvingTotalKeys}";
        }

        private void OnCarvingDone(object data)
        {
            if (stage2Panel != null) stage2Panel.SetActive(false);
        }

        // ===== STAGE 3 =====

        private void OnTheChoiceBegin(object data)
        {
            if (stage3Panel != null) stage3Panel.SetActive(true);
        }

        private void OnChoiceMade(object data)
        {
            if (stage3Panel != null) stage3Panel.SetActive(false);
        }
    }
}
