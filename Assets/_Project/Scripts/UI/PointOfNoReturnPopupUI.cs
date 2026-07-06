using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace TwilightRemnant
{
    /// <summary>
    /// Popup xác nhận Điểm Không Quay Đầu (Giai đoạn E).
    /// Chạy dialogue Yuki trước, rồi mới hiện 2 nút lựa chọn.
    /// </summary>
    public class PointOfNoReturnPopupUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button declineButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private DialogueRunner dialogueRunner;

        private void OnEnable() => EventBus.On(GameEvents.OnPointOfNoReturnTriggered, OnTriggered);
        private void OnDisable() => EventBus.Off(GameEvents.OnPointOfNoReturnTriggered, OnTriggered);

        private void Awake()
        {
            declineButton.onClick.AddListener(OnDeclineClicked);
            confirmButton.onClick.AddListener(OnConfirmClicked);

            if (panel != null)
                panel.SetActive(false);
        }

        private void OnTriggered(object data)
        {
            if (panel != null)
                panel.SetActive(false); // ẩn nút trong lúc Yuki nói

            dialogueRunner.onDialogueComplete.AddListener(ShowChoiceButtons);
            dialogueRunner.StartDialogue("DiemKhongQuayDau_Main");
        }

        private void ShowChoiceButtons()
        {
            dialogueRunner.onDialogueComplete.RemoveListener(ShowChoiceButtons);
            if (panel != null)
                panel.SetActive(true);
        }

        private void OnDeclineClicked()
        {
            if (panel != null)
                panel.SetActive(false);

            EndingManager.Instance?.DeclineCorruption();
            dialogueRunner.StartDialogue("DiemKhongQuayDau_TuChoi");
        }

        private void OnConfirmClicked()
        {
            if (panel != null)
                panel.SetActive(false);

            EndingManager.Instance?.ConfirmCorruption();
            dialogueRunner.StartDialogue("DiemKhongQuayDau_XacNhan");
        }
    }
}