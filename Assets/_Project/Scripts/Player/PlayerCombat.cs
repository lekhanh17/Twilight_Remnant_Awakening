using UnityEngine;

namespace TwilightRemnant
{
    public class PlayerCombat : MonoBehaviour
    {
        public static PlayerCombat Instance { get; private set; }

        public float attackRange = 1.2f;
        public float baseDamage = 10f;
        public LayerMask enemyLayer;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void ExecuteAttack(Vector2 direction)
        {
            Vector2 origin = transform.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, direction, attackRange, enemyLayer);
            if (hit.collider == null) return;

            var target = hit.collider.GetComponent<IDamageable>();
            if (target == null) return;

            var stats = PlayerStats.Instance != null ? PlayerStats.Instance.stats : new StatBlock();
            float dmg = DamageCalculator.Calculate(stats, baseDamage, target.Armor);
            target.TakeDamage(dmg);
        }
    }

    /// <summary>Interface chuẩn cho bất kỳ thứ gì nhận sát thương (dummy enemy, Vọng Thú...).</summary>
    public interface IDamageable
    {
        float Armor { get; }
        void TakeDamage(float amount);
    }
}
