using UnityEngine;
using UnityEngine.InputSystem;

namespace TwilightRemnant
{
    /// <summary>
    /// Giữ con trỏ trong vùng cân bằng vài giây để khống chế Lõi bất ổn (Phần 7.2 GDD).
    ///
    /// Độ khó theo Tier (Phần 7.3 GDD):
    /// - Tier F-E: vùng an toàn cố định, nhịp chậm (hành vi gốc, không đổi).
    /// - Tier D-C (Kháng Cự Sinh Học): vùng an toàn tự "giật cục" đổi vị trí ngẫu nhiên
    ///   theo chu kỳ; đứng lâu trong vùng nguy hiểm sẽ bị nổ nhẹ trừ HP.
    /// - Tier B-S (Cộng Hưởng Ngược): giống D-C (giật nhanh hơn), CỘNG THÊM CON tự giảm
    ///   liên tục suốt giai đoạn — vừa giữ nhịp vừa phải lo CON không tụt.
    ///
    /// Gắn vào cùng GameObject hoặc kéo reference tới AnatomySiteManager.
    /// </summary>
    public class MinigameStage1 : MonoBehaviour
    {
        public AnatomySiteManager manager;

        [Header("Vùng an toàn cơ bản (Tier F-E)")]
        [Range(0f, 1f)] public float safeZoneMin = 0.4f;
        [Range(0f, 1f)] public float safeZoneMax = 0.6f;
        public float requiredTime = 3f;

        [Header("Tier D-C / B-S — Kháng Cự Sinh Học")]
        [Tooltip("Khoảng thời gian ngẫu nhiên (giây) giữa 2 lần vùng an toàn 'giật cục' đổi vị trí")]
        public float jerkIntervalMin = 1f;
        public float jerkIntervalMax = 2.5f;
        [Tooltip("Đứng trong vùng nguy hiểm bao lâu (giây liên tục) thì bị nổ nhẹ 1 lần")]
        public float hazardDwellThreshold = 1f;
        public float hazardDamage = 5f;
        [Tooltip("Sau khi nổ 1 lần, bao lâu mới có thể nổ tiếp (tránh trừ máu dồn dập)")]
        public float hazardCooldown = 1.5f;

        [Header("Tier B-S — Cộng Hưởng Ngược")]
        [Tooltip("CON tự giảm mỗi giây suốt giai đoạn (chỉ áp dụng Tier B-S)")]
        public float conDrainPerSecond = 1.5f;

        private float progress;
        private bool isActive;
        private float jerkTimer;
        private float hazardDwellTimer;
        private float hazardCooldownTimer;
        private EchoSpeciesData.TierGroup tierGroup;

        public void BeginStage()
        {
            isActive = true;
            progress = 0f;
            hazardDwellTimer = 0f;
            hazardCooldownTimer = 0f;
            tierGroup = manager.species != null
                ? manager.species.GetTierGroup()
                : EchoSpeciesData.TierGroup.FE;
            ResetJerkTimer();
            EventBus.Emit(GameEvents.OnStabilizeBegin);
        }

        private void ResetJerkTimer()
        {
            jerkTimer = Random.Range(jerkIntervalMin, jerkIntervalMax);
        }

        private void Update()
        {
            if (!isActive) return;

            bool isHardTier = tierGroup != EchoSpeciesData.TierGroup.FE;

            if (isHardTier)
            {
                jerkTimer -= Time.deltaTime;
                if (jerkTimer <= 0f)
                {
                    JerkSafeZone();
                    ResetJerkTimer();
                    // Tier B-S giật nhanh hơn D-C (khó hơn 1 bậc)
                    if (tierGroup == EchoSpeciesData.TierGroup.BS)
                        jerkTimer *= 0.5f;
                }

                if (hazardCooldownTimer > 0f)
                    hazardCooldownTimer -= Time.deltaTime;
            }

            if (tierGroup == EchoSpeciesData.TierGroup.BS)
            {
                PlayerStats.Instance?.stats.ModifyCON(-conDrainPerSecond * Time.deltaTime);
            }

            float cursorPos = ReadCursorAxis();
            bool inSafeZone = cursorPos >= safeZoneMin && cursorPos <= safeZoneMax;

            if (inSafeZone)
            {
                progress += Time.deltaTime;
                hazardDwellTimer = 0f;
            }
            else
            {
                progress = Mathf.Max(0f, progress - Time.deltaTime * 0.5f);

                if (isHardTier)
                {
                    hazardDwellTimer += Time.deltaTime;
                    if (hazardDwellTimer >= hazardDwellThreshold && hazardCooldownTimer <= 0f)
                    {
                        TriggerHazardExplosion();
                        hazardDwellTimer = 0f;
                        hazardCooldownTimer = hazardCooldown;
                    }
                }
            }

            EventBus.Emit(GameEvents.OnStabilizeProgress, progress / requiredTime);

            if (progress >= requiredTime)
            {
                isActive = false;
                manager.CompleteStabilize(progress / requiredTime);
            }
        }

        /// <summary>Đổi vùng an toàn sang 1 vị trí ngẫu nhiên mới, giữ nguyên bề rộng ban đầu.</summary>
        private void JerkSafeZone()
        {
            float width = safeZoneMax - safeZoneMin;
            float newMin = Random.Range(0f, 1f - width);
            safeZoneMin = newMin;
            safeZoneMax = newMin + width;
            EventBus.Emit(GameEvents.OnStabilizeZoneChanged);
        }

        private void TriggerHazardExplosion()
        {
            PlayerStats.Instance?.stats.ModifyHP(-hazardDamage);
            EventBus.Emit(GameEvents.OnAnatomyHazardHit, hazardDamage);
        }

        /// <summary>
        /// TODO: thay bằng input thật (ví dụ vị trí chuột theo trục X quy về 0..1,
        /// hoặc giá trị joystick). Placeholder trả về giữa màn hình.
        /// </summary>
        private float ReadCursorAxis()
        {
            if (Mouse.current == null) return 0.5f;
            float x = Mouse.current.position.ReadValue().x;
            return Mathf.Clamp01(x / Screen.width);
        }
    }
}
