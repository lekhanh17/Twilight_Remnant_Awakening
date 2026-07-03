using System;
using System.Collections.Generic;

namespace TwilightRemnant
{
    /// <summary>
    /// Xương sống để các system không cần giữ reference lẫn nhau.
    /// Ví dụ: CONController.ModifyCON() emit OnCONDanger, HUD tự nghe và đổi màu
    /// mà không cần biết CONController tồn tại.
    /// Dùng hằng số trong GameEvents.cs thay vì gõ tay chuỗi.
    /// </summary>
    public static class EventBus
    {
        private static readonly Dictionary<string, Action<object>> listeners = new();

        public static void On(string eventName, Action<object> callback)
        {
            if (!listeners.ContainsKey(eventName))
                listeners[eventName] = delegate { };
            listeners[eventName] += callback;
        }

        public static void Off(string eventName, Action<object> callback)
        {
            if (listeners.ContainsKey(eventName))
                listeners[eventName] -= callback;
        }

        public static void Emit(string eventName, object data = null)
        {
            if (listeners.TryGetValue(eventName, out var callback))
                callback?.Invoke(data);
        }

        /// <summary>Gọi khi đổi scene / restart game để tránh rò rỉ listener cũ.</summary>
        public static void ClearAll()
        {
            listeners.Clear();
        }
    }
}
