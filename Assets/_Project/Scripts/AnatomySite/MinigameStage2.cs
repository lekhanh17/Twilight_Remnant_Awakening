using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// QTE theo loài — chuỗi phím lấy từ EchoSpeciesData.carvingSequence (Phần 7.2 GDD).
    ///
    /// Độ khó theo Tier (Phần 7.3 GDD):
    /// - Tier F-E: bấm sai chỉ tính miss, không phạt gì thêm (hành vi gốc).
    /// - Tier D-C / B-S: bấm sai bị "nổ nhẹ" trừ HP.
    /// - Tier B-S (Cộng Hưởng Ngược): CỘNG THÊM CON tự giảm liên tục suốt giai đoạn Carving.
    /// </summary>
    public class MinigameStage2 : MonoBehaviour
    {
        public AnatomySiteManager manager;

        [Header("Tier D-C / B-S — phạt khi bấm sai")]
        public float missHazardDamage = 4f;

        [Header("Tier B-S — Cộng Hưởng Ngược")]
        public float conDrainPerSecond = 1.5f;

        private Queue<KeyCode> qteSequence;
        private int correctHits;
        private int totalHits;
        private bool isActive;
        private EchoSpeciesData.TierGroup tierGroup;

        public void BeginStage(EchoSpeciesData species)
        {
            qteSequence = new Queue<KeyCode>(species.carvingSequence);
            correctHits = 0;
            totalHits = 0;
            isActive = true;
            tierGroup = species.GetTierGroup();
            EventBus.Emit(GameEvents.OnCarvingBegin, qteSequence.Count);
            if (qteSequence.Count > 0)
                EventBus.Emit(GameEvents.OnCarvingKeyExpected, qteSequence.Peek());
        }

        private void Update()
        {
            if (!isActive || qteSequence.Count == 0) return;

            if (tierGroup == EchoSpeciesData.TierGroup.BS)
                PlayerStats.Instance?.stats.ModifyCON(-conDrainPerSecond * Time.deltaTime);

            KeyCode expected = qteSequence.Peek();
            if (Input.GetKeyDown(expected))
            {
                totalHits++;
                correctHits++;
                qteSequence.Dequeue();
                EventBus.Emit(GameEvents.OnCarvingHit, true);
            }
            else
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (k == expected) continue;
                    if (Input.GetKeyDown(k))
                    {
                        totalHits++;

                        if (tierGroup != EchoSpeciesData.TierGroup.FE)
                        {
                            PlayerStats.Instance?.stats.ModifyHP(-missHazardDamage);
                            EventBus.Emit(GameEvents.OnAnatomyHazardHit, missHazardDamage);
                        }

                        EventBus.Emit(GameEvents.OnCarvingHit, false);
                        break;
                    }
                }
            }

            if (qteSequence.Count == 0)
            {
                isActive = false;
                manager.CompleteCarving(correctHits == totalHits);
            }
            else
            {
                EventBus.Emit(GameEvents.OnCarvingKeyExpected, qteSequence.Peek());
            }
        }
    }
}
