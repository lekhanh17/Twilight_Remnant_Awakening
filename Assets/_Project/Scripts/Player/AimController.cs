using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Giữ chuột phải để ngắm hướng đánh/bắn, nhả ra để thực hiện đòn
    /// (tương tự cơ chế bắn cung Minecraft — Phần 8.1 GDD).
    /// </summary>
    public class AimController : MonoBehaviour
    {
        public Camera mainCamera;
        private bool isAiming;

        private void Awake()
        {
            if (mainCamera == null) mainCamera = Camera.main;
        }

        private void Update()
        {
            // Camera cũ có thể đã bị huỷ sau khi đổi scene (Player persist qua scene
            // nhưng Main Camera thì không) — tự lấy lại camera mới nếu tham chiếu cũ mất.
            if (mainCamera == null) mainCamera = Camera.main;

            if (Input.GetMouseButtonDown(1))
            {
                isAiming = true;
            }
            if (Input.GetMouseButtonUp(1) && isAiming)
            {
                isAiming = false;
                PlayerCombat.Instance?.ExecuteAttack(GetAimDirection());
            }
        }

        public Vector2 GetAimDirection()
        {
            if (mainCamera == null) mainCamera = Camera.main;
            Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;
            return ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
        }
    }
}
