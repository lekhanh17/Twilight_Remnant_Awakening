using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Kiểm tra cờ "cần Load" do MainMenu đặt trước khi chuyển scene,
    /// áp dụng dữ liệu save đã có (currentDay, gold) khi Bootstrap vừa load xong.
    /// </summary>
    public class GameBootstrapper : MonoBehaviour
    {
        private const string PendingLoadKey = "TR_PendingLoad";

        private void Start()
        {
            if (PlayerPrefs.GetInt(PendingLoadKey, 0) != 1) return;

            PlayerPrefs.DeleteKey(PendingLoadKey);

            var data = SaveSystem.Instance?.Load();
            if (data == null) return;

            SaveSystem.ApplyLoadedData(data);
        }
    }
}