using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

namespace TwilightRemnant
{
    /// <summary>
    /// Lắng nghe DayCycleManager, tự động chạy đúng DayEventData khớp
    /// ngày/phase hiện tại (nếu có) qua DialogueRunner.
    /// </summary>
    public class DayEventManager : MonoBehaviour
    {
        public List<DayEventData> allEvents = new();
        public DialogueRunner dialogueRunner;

        private void OnEnable()
        {
            EventBus.On(GameEvents.OnPhaseChanged, OnPhaseChanged);
        }

        private void OnDisable()
        {
            EventBus.Off(GameEvents.OnPhaseChanged, OnPhaseChanged);
        }

        private void OnPhaseChanged(object data)
        {
            if (data is not DayPhase phase) return;
            if (DayCycleManager.Instance == null) return;

            int currentDay = DayCycleManager.Instance.currentDay;

            foreach (var ev in allEvents)
            {
                if (ev == null) continue;

                if (ev.dayIndex == currentDay && ev.phaseTrigger == phase && ev.CanTrigger())
                {
                    Debug.Log($"[DayEventManager] Trigger event: Day={ev.dayIndex} Phase={ev.phaseTrigger} Node={ev.yarnNodeName}");
                    dialogueRunner.StartDialogue(ev.yarnNodeName);
                    return; // chỉ chạy 1 event mỗi lần đổi phase
                }
            }
        }
    }
}