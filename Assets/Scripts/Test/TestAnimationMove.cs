using UnityEngine;

public class TestAnimationMove : MonoBehaviour
{
    public float moveSpeed = 5f; // ความเร็วการเคลื่อนที่
    public float rotationSpeed = 700f; // ความเร็วในการหมุน

    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        // ดึง Animator และ Rigidbody ของตัวละคร
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // ตรวจสอบการเคลื่อนที่
        float horizontal = Input.GetAxis("Horizontal"); // ปุ่ม A/D หรือ Arrow Left/Right
        float vertical = Input.GetAxis("Vertical"); // ปุ่ม W/S หรือ Arrow Up/Down

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // ถ้ามีการกดปุ่มเพื่อเคลื่อนที่
        if (moveDirection.magnitude >= 0.1f)
        {
            // การหมุนตัวละครไปตามทิศทางที่ผู้เล่นกด
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSpeed, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // การเคลื่อนที่ของตัวละคร
            Vector3 move = transform.forward * moveSpeed * Time.deltaTime;
            rb.MovePosition(transform.position + move);

            // เล่นอนิเมชั่นวิ่ง
            animator.SetFloat("Speed", move.magnitude);
        }
        else
        {
            // ถ้าไม่กดปุ่มให้หยุดการเล่นอนิเมชั่น
            animator.SetFloat("Speed", 0f);
        }
    }
}
