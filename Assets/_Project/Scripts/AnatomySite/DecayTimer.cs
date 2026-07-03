using UnityEngine;

namespace TwilightRemnant
{
    public enum DecayStage { Fresh, EarlyDecay, PartialRevive, EchoHusk }

    /// <summary>
    /// Đồng hồ phân hủy theo Phần 7.1 GDD:
    /// 0-6h Xác còn nóng | 6-24h Phân hủy sớm | 24-48h Hồi sinh một phần | 48h+ Echo Husk.
    /// </summary>
    public class DecayTimer : MonoBehaviour
    {
        public float hoursSinceDeath = 0f;

        public DecayStage CurrentStage
        {
            get
            {
                if (hoursSinceDeath < 6f) return DecayStage.Fresh;
                if (hoursSinceDeath < 24f) return DecayStage.EarlyDecay;
                if (hoursSinceDeath < 48f) return DecayStage.PartialRevive;
                return DecayStage.EchoHusk;
            }
        }

        private void Update()
        {
            float hourPerSecond = GameManager.Instance != null ? GameManager.Instance.hourPerSecond : 1f;
            hoursSinceDeath += Time.deltaTime * hourPerSecond;
        }
    }
}
