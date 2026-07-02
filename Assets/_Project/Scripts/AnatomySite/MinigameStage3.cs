using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Giai đoạn "Quyết Định" — chỉ là 2 nút UI gọi thẳng AnatomySiteManager.MakeChoice().
    /// Gắn 2 hàm dưới vào OnClick() của nút "Nộp công ty" và "Giữ lại".
    /// </summary>
    public class MinigameStage3 : MonoBehaviour
    {
        public AnatomySiteManager manager;

        public void ChooseSubmit() => manager.MakeChoice(keepCore: false);
        public void ChooseKeep() => manager.MakeChoice(keepCore: true);
    }
}
