using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Khối vuông di chuyển ngẫu nhiên — dùng để test PlayerCombat/DamageCalculator
    /// ở Bước 5-6 trước khi có AI/art thật (xem LỖI 3, Phần 12 tài liệu kỹ thuật).
    /// </summary>
    public class DummyEnemy : MonoBehaviour, IDamageable
    {
        public float maxHP = 50f;
        public float currentHP;
        public float armor = 2f;
        public float wanderSpeed = 1.5f;

        public float Armor => armor;

        private Vector2 wanderTarget;

        private void Start()
        {
            currentHP = maxHP;
            PickNewWanderTarget();
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, wanderTarget, wanderSpeed * Time.deltaTime);
            if (Vector2.Distance(transform.position, wanderTarget) < 0.1f)
                PickNewWanderTarget();
        }

        private void PickNewWanderTarget()
        {
            wanderTarget = (Vector2)transform.position + Random.insideUnitCircle * 3f;
        }

        public void TakeDamage(float amount)
        {
            currentHP -= amount;
            Debug.Log($"[DummyEnemy] Nhận {amount:F1} sát thương, còn {currentHP:F1} HP");
            if (currentHP <= 0f) Destroy(gameObject);
        }
    }
}
