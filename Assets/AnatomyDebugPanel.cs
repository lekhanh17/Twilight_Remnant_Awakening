using UnityEngine;
using TwilightRemnant;

/// <summary>
/// Panel debug tạm test Anatomy Site — XOÁ SAU KHI TEST XONG GIAI ĐOẠN B.
/// Nhấn F2 để ẩn/hiện.
/// </summary>
public class AnatomyDebugPanel : MonoBehaviour
{
    public AnatomySiteManager manager;
    public MinigameStage1 stage1;
    public MinigameStage2 stage2;
    public MinigameStage3 stage3;

    private bool visible = true;

    private void OnEnable()
    {
        EventBus.On(GameEvents.OnStabilizeDone, d => Debug.Log("[AnatomyDebug] OnStabilizeDone, quality=" + d));
        EventBus.On(GameEvents.OnCarvingDone, d => Debug.Log("[AnatomyDebug] OnCarvingDone, perfect=" + d));
        EventBus.On(GameEvents.OnCoreKeep, d => Debug.Log("[AnatomyDebug] OnCoreKeep bắn ra — đã GIỮ LÕI"));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) visible = !visible;
    }

    private void OnGUI()
    {
        if (!visible) return;

        GUILayout.BeginArea(new Rect(280, 10, 260, 420), GUI.skin.box);

        GUILayout.Label("=== ANATOMY SITE DEBUG === (F2 để ẩn)");
        GUILayout.Label("Current Stage: " + manager.currentStage);

        GUILayout.Space(10);
        GUILayout.Label("STAGE 1 — Ổn Định");
        GUILayout.Label("Giữ chuột trong vùng an toàn (giữa màn hình theo trục X)");
        if (GUILayout.Button("Bắt đầu Stage 1")) stage1.BeginStage();

        GUILayout.Space(10);
        GUILayout.Label("STAGE 2 — Cắt Tách (QTE)");
        GUILayout.Label("Bấm đúng thứ tự phím: A, S, D");
        if (GUILayout.Button("Bắt đầu Stage 2")) stage2.BeginStage(manager.species);

        GUILayout.Space(10);
        GUILayout.Label("STAGE 3 — Quyết Định");
        if (GUILayout.Button("Chọn: Nộp công ty")) stage3.ChooseSubmit();
        if (GUILayout.Button("Chọn: Giữ lại (Lõi)")) stage3.ChooseKeep();

        GUILayout.Space(10);
        GUILayout.Label("=== KẾT QUẢ ===");
        GUILayout.Label("Wallet: " + (Wallet.Instance != null ? Wallet.Instance.gold.ToString() : "N/A"));
        GUILayout.Label("PlayerStats: " + (PlayerStats.Instance != null
            ? $"RES={PlayerStats.Instance.stats.RES:F1} CON={PlayerStats.Instance.stats.CON:F1}"
            : "N/A — chưa có PlayerStats trong scene"));
        GUILayout.Label("BlackMarket timesKeptCore: " + (BlackMarketFlags.Instance != null ? BlackMarketFlags.Instance.timesKeptCore.ToString() : "N/A"));

        GUILayout.EndArea();
    }
}