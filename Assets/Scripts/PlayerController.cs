using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Thông số Di chuyển")]
    public float moveSpeed = 5f; // Tốc độ di chuyển của Đồ Tể

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        // Tự động tìm và liên kết với component Rigidbody2D đang gắn trên Ren
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 1. NHẬN TÍN HIỆU ĐIỀU KHIỂN (Nhạy bén từng frame)
        // Dùng GetAxisRaw thay vì GetAxis để nhân vật dừng lại ngay lập tức khi nhả phím 
        // (Rất quan trọng cho game Pixel 2D để tránh cảm giác bị trượt băng)
        movement.x = Input.GetAxisRaw("Horizontal"); // Phím A/D hoặc Trái/Phải
        movement.y = Input.GetAxisRaw("Vertical");   // Phím W/S hoặc Lên/Xuống

        // Chuẩn hóa vector: Đảm bảo khi Ren đi chéo (nhấn cùng lúc W và D) 
        // thì tốc độ không bị cộng dồn nhanh hơn đi thẳng
        movement = movement.normalized;
    }

    void FixedUpdate()
    {
        // 2. THỰC THI VẬT LÝ (Chạy ở tốc độ khung hình cố định)
        // Di chuyển nhân vật dựa trên vị trí hiện tại + hướng đi * tốc độ * thời gian vật lý
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}