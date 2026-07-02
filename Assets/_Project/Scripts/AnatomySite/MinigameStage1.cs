using UnityEngine;
using UnityEngine.InputSystem;

namespace TwilightRemnant
{
    /// <summary>
    /// Giữ con trỏ trong vùng cân bằng vài giây để khống chế Lõi bất ổn (Phần 7.2 GDD).
    /// Gắn vào cùng GameObject hoặc kéo reference tới AnatomySiteManager.
    /// </summary>
    public class MinigameStage1 : MonoBehaviour
    {
        public AnatomySiteManager manager;

        [Range(0f, 1f)] public float safeZoneMin = 0.4f;
        [Range(0f, 1f)] public float safeZoneMax = 0.6f;
        public float requiredTime = 3f;

        private float progress;
        private bool isActive;

        public void BeginStage() { isActive = true; progress = 0f; }

        private void Update()
        {
            if (!isActive) return;

            float cursorPos = ReadCursorAxis();
            if (cursorPos >= safeZoneMin && cursorPos <= safeZoneMax)
                progress += Time.deltaTime;
            else
                progress = Mathf.Max(0f, progress - Time.deltaTime * 0.5f);

            if (progress >= requiredTime)
            {
                isActive = false;
                manager.CompleteStabilize(progress / requiredTime);
            }
        }

        /// <summary>
        /// TODO: thay bằng input thật (ví dụ vị trí chuột theo trục X quy về 0..1,
        /// hoặc giá trị joystick). Placeholder trả về giữa màn hình.
        /// </summary>
        private float ReadCursorAxis()
        {
            if (Mouse.current == null) return 0.5f;
            float x = Mouse.current.position.ReadValue().x;
            return Mathf.Clamp01(x / Screen.width);
        }
    }
}
