using UnityEngine;
using Unity.Cinemachine;

namespace TwilightRemnant
{
    /// <summary>
    /// Gắn vào GameObject Cinemachine Camera (CM_FollowRen) ở MỖI scene có Player
    /// đi lại (Hub_TwilightCity, ScarZone...). Vì Player giờ persist qua scene
    /// (PlayerPersistence.cs), camera ở scene MỚI không tự biết theo ai nếu chỉ
    /// kéo-thả tay 1 lần ở Bootstrap — script này tự tìm Player lúc Start().
    /// </summary>
    [RequireComponent(typeof(CinemachineCamera))]
    public class CameraAutoFollowPlayer : MonoBehaviour
    {
        private void Start()
        {
            var vcam = GetComponent<CinemachineCamera>();
            if (vcam == null || PlayerStats.Instance == null) return;
            vcam.Follow = PlayerStats.Instance.transform;
        }
    }
}
