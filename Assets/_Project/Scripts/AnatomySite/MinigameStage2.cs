using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// QTE theo loài — chuỗi phím lấy từ EchoSpeciesData.carvingSequence (Phần 7.2 GDD).
    /// </summary>
    public class MinigameStage2 : MonoBehaviour
    {
        public AnatomySiteManager manager;

        private Queue<KeyCode> qteSequence;
        private int correctHits;
        private int totalHits;
        private bool isActive;

        public void BeginStage(EchoSpeciesData species)
        {
            qteSequence = new Queue<KeyCode>(species.carvingSequence);
            correctHits = 0;
            totalHits = 0;
            isActive = true;
        }

        private void Update()
        {
            if (!isActive || qteSequence.Count == 0) return;

            KeyCode expected = qteSequence.Peek();
            if (Input.GetKeyDown(expected))
            {
                totalHits++;
                correctHits++;
                qteSequence.Dequeue();
            }
            else
            {
                foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
                {
                    if (k == expected) continue;
                    if (Input.GetKeyDown(k))
                    {
                        totalHits++;
                        // TODO: trừ điểm / hiệu ứng nổ nhẹ nếu Tier D-C trở lên (Phần 7.3 GDD)
                        break;
                    }
                }
            }

            if (qteSequence.Count == 0)
            {
                isActive = false;
                manager.CompleteCarving(correctHits == totalHits);
            }
        }
    }
}
