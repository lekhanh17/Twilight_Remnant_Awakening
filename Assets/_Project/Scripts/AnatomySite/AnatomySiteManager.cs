using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Điều phối quy trình thu hoạch 3 giai đoạn (Phần 7.2 GDD):
    /// Ổn Định -> Cắt Tách -> Quyết Định (Nộp công ty / Giữ lại).
    /// </summary>
    public class AnatomySiteManager : MonoBehaviour
    {
        public enum Stage { Stabilize, Carving, TheChoice, Done }

        public EchoSpeciesData species;
        public DecayTimer decay;
        public Stage currentStage = Stage.Stabilize;

        public void CompleteStabilize(float qualityScore)
        {
            currentStage = Stage.Carving;
            EventBus.Emit(GameEvents.OnStabilizeDone, qualityScore);
        }

        public void CompleteCarving(bool perfect)
        {
            currentStage = Stage.TheChoice;
            EventBus.Emit(GameEvents.OnCarvingDone, perfect);
        }

        /// <summary>true = Giữ lại (RES tăng, CON giảm, tăng flag chợ đen) | false = Nộp công ty (tiền).</summary>
        public void MakeChoice(bool keepCore)
        {
            var stats = PlayerStats.Instance?.stats;

            if (keepCore)
            {
                if (stats != null)
                {
                    stats.RES += species.resGain;
                    stats.ModifyCON(-species.conCostOnKeep);
                }
                BlackMarketFlags.Instance?.RegisterCoreKept();
                EventBus.Emit(GameEvents.OnCoreKeep);
            }
            else
            {
                Wallet.Instance?.Add(species.sellPrice);
            }

            currentStage = Stage.Done;
        }
    }
}
