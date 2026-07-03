using System.Collections.Generic;
using UnityEngine;

namespace TwilightRemnant
{
    /// <summary>
    /// VESSEL — Vay Mượn Bản Năng: chỉ mimic skill của loài ĐÃ mổ xẻ qua Anatomy Site
    /// (Phần 5 GDD).
    /// </summary>
    public class MimicHandler : MonoBehaviour
    {
        private readonly HashSet<string> dissectedSpeciesIds = new();

        public void MarkDissected(string speciesId) => dissectedSpeciesIds.Add(speciesId);

        public bool CanMimic(string speciesId) => dissectedSpeciesIds.Contains(speciesId);
    }
}
