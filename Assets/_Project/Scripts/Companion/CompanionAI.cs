using UnityEngine;
using UnityEngine.AI;

namespace TwilightRemnant
{
    /// <summary>
    /// AI đơn giản cho 1 Đồng Minh Cộng Hưởng (Phần 7.4 GDD):
    /// theo sau player, tấn công mục tiêu gần nhất khi có.
    /// Cần package AI Navigation (com.unity.ai.navigation) và NavMesh đã bake trong scene.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class CompanionAI : MonoBehaviour, IDamageable
    {
        public Transform player;
        public float detectRadius = 6f;
        public float attackRange = 1.2f;
        public float attackCooldown = 1.5f;
        public float baseDamage = 8f;
        public LayerMask enemyLayer;

        [Header("Duy trì liên kết (Phần 7.4 GDD)")]
        [Tooltip("Tự trừ CON của Ren mỗi khi ngày mới bắt đầu — không cần gọi tay.")]
        public bool autoUpkeepOnDayStart = true;
        public float dailyUpkeepCost = 1f;

        [Header("HP của Đồng Minh (không phải HP của Ren)")]
        public float maxCompanionHP = 50f;
        public float currentCompanionHP;

        private NavMeshAgent agent;
        private Transform currentTarget;
        private float lastAttackTime = -999f;

        public float Armor => 0f; // đồng minh cấp thấp, chưa có giáp riêng

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            currentCompanionHP = maxCompanionHP;
        }

        private void OnEnable()
        {
            if (autoUpkeepOnDayStart)
                EventBus.On(GameEvents.OnDayStarted, OnDayStarted);
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnDayStarted, OnDayStarted);
        }

        private void OnDayStarted(object data)
        {
            if (PlayerStats.Instance != null)
                OnDailyUpkeep(PlayerStats.Instance.stats);
        }

        private void Update()
        {
            currentTarget = FindNearestEnemy();

            if (currentTarget != null)
            {
                float dist = Vector2.Distance(transform.position, currentTarget.position);
                if (dist > attackRange)
                {
                    agent.SetDestination(currentTarget.position);
                }
                else
                {
                    agent.ResetPath(); // đủ gần, đứng lại đánh, không lấn vào mục tiêu
                    TryAttack(currentTarget);
                }
            }
            else if (player != null)
            {
                agent.SetDestination(player.position);
            }
        }

        private void TryAttack(Transform target)
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
            var damageable = target.GetComponent<IDamageable>();
            if (damageable == null) return;

            lastAttackTime = Time.time;
            var stats = PlayerStats.Instance != null ? PlayerStats.Instance.stats : new StatBlock();
            float dmg = DamageCalculator.Calculate(stats, baseDamage, damageable.Armor);
            damageable.TakeDamage(dmg);
        }

        private Transform FindNearestEnemy()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectRadius, enemyLayer);
            Transform nearest = null;
            float minDist = float.MaxValue;
            foreach (var hit in hits)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist) { minDist = dist; nearest = hit.transform; }
            }
            return nearest;
        }

        /// <summary>Đồng minh cũng có thể bị đánh trúng (Vọng Thú địch tấn công lại).</summary>
        public void TakeDamage(float amount)
        {
            currentCompanionHP = Mathf.Max(0f, currentCompanionHP - amount);
            if (currentCompanionHP <= 0f)
            {
                Debug.Log("[CompanionAI] Đồng minh đã kiệt sức, không thể chiến đấu tiếp.");
                // Chưa xử lý "đồng minh chết" chi tiết — để dành nếu Episode sau cần,
                // hiện tại Episode 1 chỉ có tối đa 1 đồng minh nên tắt AI tạm là đủ.
                enabled = false;
                agent.ResetPath();
            }
        }

        /// <summary>
        /// Duy trì liên kết tốn -1 CON/ngày (Phần 7.4 GDD).
        /// Tự động gọi qua OnDayStarted nếu autoUpkeepOnDayStart = true — vẫn có thể
        /// gọi tay nếu cần (ví dụ test qua DevConsole).
        /// </summary>
        public void OnDailyUpkeep(StatBlock ownerStats)
        {
            ownerStats.ModifyCON(-dailyUpkeepCost);
        }
    }
}
