using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Log vị trí Companion mỗi giây để xác nhận có thực sự di chuyển hay không.
/// XOÁ SAU KHI TEST XONG.
/// </summary>
public class CompanionPositionLogger : MonoBehaviour
{
    private NavMeshAgent agent;
    private float timer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            Debug.Log($"[PosLogger] Position={transform.position} | IsOnNavMesh={agent.isOnNavMesh} | HasPath={agent.hasPath} | RemainingDistance={agent.remainingDistance} | Destination={agent.destination}");
        }
    }
}