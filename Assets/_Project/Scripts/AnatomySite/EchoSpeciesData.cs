using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// Định nghĩa 1 loài Vọng Thú. Tạo asset: chuột phải trong Project window
    /// -> Create -> TwilightRemnant -> Echo Species Data.
    /// </summary>
    [CreateAssetMenu(fileName = "NewEchoSpecies", menuName = "TwilightRemnant/Echo Species Data")]
    public class EchoSpeciesData : ScriptableObject
    {
        /// <summary>
        /// 3 nhóm độ khó theo Phần 7.3 GDD:
        /// FE = Tier F-E (nhịp chậm, vùng an toàn lớn)
        /// DC = Tier D-C (Kháng Cự Sinh Học — giật cục/đảo chiều, chạm nguy hiểm bị nổ nhẹ)
        /// BS = Tier B-S (Cộng Hưởng Ngược — như DC, cộng thêm CON tự giảm liên tục)
        /// </summary>
        public enum TierGroup { FE, DC, BS }

        public string speciesId;
        public string displayName;

        [Tooltip("F, E, D, C, B, A, S")]
        public string tier = "F";

        public float maxHP = 50f;
        public float armor = 2f;

        [Header("Kinh tế / Phần thưởng")]
        public int sellPrice = 20;
        public float resGain = 5f;
        public float conCostOnKeep = 5f;

        [Header("Carving QTE")]
        public KeyCode[] carvingSequence;

        /// <summary>Quy đổi chữ tier (F/E/D/C/B/A/S) sang nhóm độ khó (Phần 7.3 GDD).</summary>
        public TierGroup GetTierGroup()
        {
            switch (tier)
            {
                case "F":
                case "E":
                    return TierGroup.FE;
                case "D":
                case "C":
                    return TierGroup.DC;
                case "B":
                case "A":
                case "S":
                    return TierGroup.BS;
                default:
                    Debug.LogWarning($"[EchoSpeciesData] '{displayName}' có tier lạ '{tier}', mặc định về FE.");
                    return TierGroup.FE;
            }
        }
    }
}
