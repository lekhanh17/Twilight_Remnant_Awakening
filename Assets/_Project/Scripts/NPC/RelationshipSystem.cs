using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    public class RelationshipSystem : MonoBehaviour
    {
        public static RelationshipSystem Instance { get; private set; }

        private readonly Dictionary<string, int> relationshipPoints = new();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void AddPoints(string npcId, int amount)
        {
            relationshipPoints[npcId] = relationshipPoints.GetValueOrDefault(npcId, 0) + amount;
            EventBus.Emit(GameEvents.OnRelationshipChanged, npcId);
        }

        public int GetPoints(string npcId) => relationshipPoints.GetValueOrDefault(npcId, 0);

        /// <summary>
        /// Yuki phát hiện Ren dính líu Cô Tư -> phản ứng lo lắng, không phán xét
        /// (Phần 4, Hồi 2 — GDD bản 3.1). Gọi khi trigger điều kiện tương ứng trong kịch bản.
        /// TODO: gắn DialogueRunner thật khi hệ thống dialogue đã dựng xong.
        /// </summary>
        public void OnBlackMarketDiscoveredByYuki()
        {
            Debug.Log("[RelationshipSystem] TODO: play dialogue node Yuki_Worry_BlackMarket");
        }
    }
}
