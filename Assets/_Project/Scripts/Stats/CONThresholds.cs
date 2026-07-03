namespace TwilightRemnant
{
    /// <summary>
    /// Bảng ngưỡng CON theo Phần 3.5 GDD:
    /// 100-70 bình thường | 70-40 nghe tiếng vọng | 40-20 di chuyển bất ổn, NPC nghi ngờ
    /// | 20-5 có thể tấn công nhầm | 5-0 ngất | =0 chủ động -> Điểm Không Quay Đầu.
    /// </summary>
    public static class CONThresholds
    {
        public static void CheckThreshold(float oldValue, float newValue)
        {
            if (newValue <= 20f && oldValue > 20f)
                EventBus.Emit(GameEvents.OnCONDanger);

            if (newValue <= 0f)
                EventBus.Emit(GameEvents.OnCONZero);
        }

        public static string DescribeState(float con)
        {
            if (con > 70f) return "Bình thường, toàn quyền kiểm soát";
            if (con > 40f) return "Bắt đầu nghe tiếng vọng, mắt đổi màu nhẹ";
            if (con > 20f) return "Di chuyển bất ổn, NPC nhận ra điều khác lạ";
            if (con > 5f)  return "Có thể tấn công nhầm, đồng minh có thể mất kiểm soát";
            if (con > 0f)  return "Sắp ngất";
            return "CON = 0 — Điểm Không Quay Đầu";
        }
    }
}
