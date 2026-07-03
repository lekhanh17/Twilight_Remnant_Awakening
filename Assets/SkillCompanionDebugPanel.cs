using UnityEngine;
using TwilightRemnant;

/// <summary>
/// Panel debug tạm test Skill/Companion/Vessel — XOÁ SAU KHI TEST XONG GIAI ĐOẠN C.
/// Nhấn F3 để ẩn/hiện.
/// </summary>
public class SkillCompanionDebugPanel : MonoBehaviour
{
    public SkillTree skillTree;
    public SkillData testSkill;
    public VesselFormController vesselController;

    private bool visible = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3)) visible = !visible;
    }

    private void OnGUI()
    {
        if (!visible) return;

        GUILayout.BeginArea(new Rect(340, 10, 320, 400), GUI.skin.box);

        GUILayout.Label("=== SKILL TREE === (F3 để ẩn)");
        if (PlayerStats.Instance != null)
        {
            GUILayout.Label("RES hiện tại: " + PlayerStats.Instance.stats.RES.ToString("F1"));
            GUILayout.Label("CON hiện tại: " + PlayerStats.Instance.stats.CON.ToString("F1"));

            if (GUILayout.Button("Thử Unlock Skill (cần RES >= " + testSkill.resRequired + ")"))
            {
                bool ok = skillTree.TryUnlock(testSkill, PlayerStats.Instance.stats);
                Debug.Log("[SkillDebug] TryUnlock = " + ok);
            }

            if (GUILayout.Button("Dùng Skill (tốn " + testSkill.conCost + " CON)"))
            {
                skillTree.UseSkill(testSkill, PlayerStats.Instance.stats);
            }

            GUILayout.Label("Đã unlock: " + skillTree.unlockedSkills.Count + " skill");
        }
        else
        {
            GUILayout.Label("Chưa có PlayerStats trong scene!");
        }

        GUILayout.Space(15);
        GUILayout.Label("=== VESSEL FORM ===");
        if (vesselController != null && PlayerStats.Instance != null)
        {
            GUILayout.Label("IsActive: " + vesselController.IsActive);
            if (GUILayout.Button("Activate Vessel Form (-10 CON)"))
            {
                vesselController.Activate(PlayerStats.Instance.stats);
            }
        }

        GUILayout.EndArea();
    }
}