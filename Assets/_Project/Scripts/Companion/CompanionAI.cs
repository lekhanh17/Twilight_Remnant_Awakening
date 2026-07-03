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
    public class CompanionAI : MonoBehaviour
    {
        public Transform player;
        public float detectRadius = 6f;
        public LayerMask enemyLayer;

        private NavMeshAgent agent;
        private Transform currentTarget;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            currentTarget = FindNearestEnemy();

            if (currentTarget != null)
            {
                agent.SetDestination(currentTarget.position);
                // TODO: gọi tấn công thật khi trong tầm, dùng lại DamageCalculator nếu cần
            }
            else if (player != null)
            {
                agent.SetDestination(player.position);
            }
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

        /// <summary>
        /// Duy trì liên kết tốn -1 CON/ngày (Phần 7.4 GDD).
        /// Gọi từ DayCycleManager.EndDay() hoặc EventBus.On(GameEvents.OnDayStarted, ...).
        /// </summary>
        public void OnDailyUpkeep(StatBlock ownerStats)
        {
            ownerStats.ModifyCON(-1f);
        }
    }
}
