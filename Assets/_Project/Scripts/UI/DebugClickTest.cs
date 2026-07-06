using UnityEngine;
using UnityEngine.EventSystems;

namespace TwilightRemnant
{
    /// <summary>
    /// DEBUG TẠM — gắn vào Continue Button để kiểm tra click chuột có thực sự
    /// chạm tới nút này hay không, tách biệt hoàn toàn khỏi logic Yarn Spinner.
    /// Nếu click mà KHÔNG thấy log này in ra -> click không hề chạm tới nút
    /// (do bị vật gì đó che/chặn phía trước, hoặc EventSystem không hoạt động).
    /// Nếu THẤY log in ra -> click có chạm tới nút, vấn đề nằm ở hàm được gọi.
    /// XOÁ SCRIPT NÀY sau khi debug xong.
    /// </summary>
    public class DebugClickTest : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[DEBUG-CLICK] OnPointerClick NHẬN ĐƯỢC trên " + gameObject.name);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("[DEBUG-CLICK] OnPointerDown NHẬN ĐƯỢC trên " + gameObject.name);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("[DEBUG-CLICK] Chuột ĐANG DI VÀO vùng " + gameObject.name);
        }
    }
}
