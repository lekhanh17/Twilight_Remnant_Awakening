using UnityEngine;

namespace TwilightRemnant
{
    public class PlayerCombat : MonoBehaviour
    {
        public static PlayerCombat Instance { get; private set; }

        public float attackRange = 1.2f;
        public float baseDamage = 10f;
        public LayerMask enemyLayer;

        [Header("Hiệu ứng Tàn Hưởng (chỉ hiện sau khi thức tỉnh)")]
        public GameObject tanHuongSlashPrefab;
        public string awakeningFlag = "Canh04_HoanThanh";
        public float slashEffectLifetime = 0.2f;

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

            SpawnSlashEffectIfAwakened(hit.point, direction);
        }

        private void SpawnSlashEffectIfAwakened(Vector2 point, Vector2 direction)
        {
            if (tanHuongSlashPrefab == null) return;
            bool awakened = StoryFlagManager.Instance != null && StoryFlagManager.Instance.HasFlag(awakeningFlag);
            if (!awakened) return; // chưa thức tỉnh -> chưa có hiệu ứng Tàn Hưởng, đúng lore

            var effect = Instantiate(tanHuongSlashPrefab, point, Quaternion.identity);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            effect.transform.rotation = Quaternion.Euler(0, 0, angle);
            Destroy(effect, slashEffectLifetime);
        }
    }

    /// <summary>Interface chuẩn cho bất kỳ thứ gì nhận sát thương (dummy enemy, Vọng Thú...).</summary>
    public interface IDamageable
    {
        float Armor { get; }
        void TakeDamage(float amount);
    }
}
