using UnityEngine;
using UnityEngine.SceneManagement;

namespace TwilightRemnant
{
    /// <summary>
    /// Singleton trung tâm để chuyển scene trong toàn game (Bootstrap/Hub ↔
    /// AnatomySite/ScarZone...). Gọi SceneTransitionManager.Instance.GoToScene(...)
    /// từ bất kỳ đâu (ví dụ VetSeoEntrance khi Player chạm vào cửa Vết Sẹo).
    ///
    /// Sau khi scene mới load xong, tự tìm đúng ScenePlayerSpawnPoint khớp
    /// spawnId đã yêu cầu, rồi đặt Player vào đúng vị trí đó.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        private string pendingSpawnId = "Default";

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        /// <summary>Gọi hàm này để chuyển sang scene khác.</summary>
        /// <param name="sceneName">Tên scene đích (phải có trong Build Settings).</param>
        /// <param name="spawnId">Khớp với spawnId của 1 ScenePlayerSpawnPoint trong scene đích.</param>
        public void GoToScene(string sceneName, string spawnId = "Default")
        {
            pendingSpawnId = spawnId;
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            var spawnPoints = FindObjectsByType<ScenePlayerSpawnPoint>(FindObjectsSortMode.None);
            ScenePlayerSpawnPoint match = null;
            foreach (var sp in spawnPoints)
            {
                if (sp.spawnId == pendingSpawnId) { match = sp; break; }
            }

            if (match == null && spawnPoints.Length > 0)
                match = spawnPoints[0]; // fallback: dùng điểm spawn đầu tiên tìm được nếu không khớp id

            if (match != null && PlayerStats.Instance != null)
            {
                PlayerStats.Instance.transform.position = match.transform.position;
                Debug.Log($"[SceneTransitionManager] Đặt Player vào spawn point '{match.spawnId}' tại {match.transform.position}");
            }
            else if (PlayerStats.Instance == null)
            {
                Debug.LogWarning("[SceneTransitionManager] Không tìm thấy PlayerStats.Instance để đặt vị trí spawn.");
            }
        }
    }
}
