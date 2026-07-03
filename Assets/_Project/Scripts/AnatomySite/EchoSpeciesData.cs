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
    }
}
