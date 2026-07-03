using UnityEngine;
using TwilightRemnant;

/// <summary>
/// Panel debug tạm — XOÁ SAU KHI TEST XONG GIAI ĐOẠN A.
/// Nhấn F1 để ẩn/hiện.
/// </summary>
public class DebugPanel : MonoBehaviour
{
    private float conSlider = 100f;
    private bool visible = true;

    private void Start()
    {
        EventBus.On("Test", data => Debug.Log("[DebugPanel] EventBus OK, nhận: " + data));
        EventBus.Emit("Test", 123);

        if (PlayerStats.Instance != null)
            conSlider = PlayerStats.Instance.stats.CON;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1)) visible = !visible;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, Screen.height - 25, 500, 25),
        "F1=DayCycle(" + visible + ")   F2=Anatomy   F3=Skill/Vessel", GUI.skin.box);
        if (!visible) return;

        GUILayout.BeginArea(new Rect(10, 10, 260, 560), GUI.skin.box);

        GUILayout.Label("=== DAY CYCLE === (F1 để ẩn)");
        if (GUILayout.Button("Morning")) DayCycleManager.Instance.AdvancePhase(DayPhase.Morning);
        if (GUILayout.Button("WorkShift")) DayCycleManager.Instance.AdvancePhase(DayPhase.WorkShift);
        if (GUILayout.Button("Lunch")) DayCycleManager.Instance.AdvancePhase(DayPhase.Lunch);
        if (GUILayout.Button("TurnIn")) DayCycleManager.Instance.AdvancePhase(DayPhase.TurnIn);
        if (GUILayout.Button("EveningChoice (19:00/21:00)")) DayCycleManager.Instance.AdvancePhase(DayPhase.EveningChoice);
        if (GUILayout.Button("NightScar")) DayCycleManager.Instance.AdvancePhase(DayPhase.NightScar);
        if (GUILayout.Button("Sleep (kết thúc ngày)")) DayCycleManager.Instance.AdvancePhase(DayPhase.Sleep);
        GUILayout.Label("Ngày: " + DayCycleManager.Instance.currentDay + " | Phase: " + DayCycleManager.Instance.currentPhase);

        GUILayout.Space(10);
        GUILayout.Label("=== STAMINA ===");
        GUILayout.Label("Hiện tại: " + StaminaSystem.Instance.current + "/" + StaminaSystem.Instance.max);
        if (GUILayout.Button("Tốn 10 (giả lập hành động)"))
        {
            bool ok = StaminaSystem.Instance.TrySpend(10f);
            Debug.Log("[DebugPanel] TrySpend(10) = " + ok);
        }

        GUILayout.Space(10);
        GUILayout.Label("=== CON ===");
        GUILayout.Label("CON: " + PlayerStats.Instance.stats.CON.ToString("F1") + " | " + CONThresholds.DescribeState(PlayerStats.Instance.stats.CON));
        float newSlider = GUILayout.HorizontalSlider(conSlider, 0f, 100f);
        if (!Mathf.Approximately(newSlider, conSlider))
        {
            float delta = newSlider - conSlider;
            conSlider = newSlider;
            PlayerStats.Instance.stats.ModifyCON(delta);
        }

        GUILayout.Space(10);
        GUILayout.Label("=== SAVE/LOAD ===");
        if (GUILayout.Button("AutoSave")) SaveSystem.Instance.AutoSave();
        if (GUILayout.Button("Load (xem log)"))
        {
            var data = SaveSystem.Instance.Load();
            if (data != null) Debug.Log("[DebugPanel] Load OK — Day=" + data.currentDay + " Gold=" + data.gold);
        }

        GUILayout.EndArea();
    }
}