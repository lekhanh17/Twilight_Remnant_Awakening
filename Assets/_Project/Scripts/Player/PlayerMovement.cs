using UnityEngine;
using UnityEngine.InputSystem;

namespace TwilightRemnant
{
    /// <summary>
    /// Di chuyển WASD dùng New Input System + Rigidbody2D.
    /// Cần 1 Input Actions asset có action "Move" (Vector2) trong action map "Player".
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        public float moveSpeed = 4.5f;

        [Tooltip("Kéo thả Input Actions asset vào đây, action Move (Vector2)")]
        public InputActionReference moveAction;

        private Rigidbody2D rb;
        private Vector2 inputDir;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable() => moveAction?.action.Enable();
        private void OnDisable() => moveAction?.action.Disable();

        private void Update()
        {
            inputDir = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;
        }

        private void FixedUpdate()
        {
            rb.MovePosition(rb.position + inputDir.normalized * moveSpeed * Time.fixedDeltaTime);
        }
    }
}
