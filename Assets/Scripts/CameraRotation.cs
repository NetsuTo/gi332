using Unity.Cinemachine;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float moveSpeed = 5f; // ความเร็วในการเคลื่อนที่
    public CinemachineCamera freeLookCamera; // กล้อง Cinemachine
    private Transform cameraTransform; // ตำแหน่งของกล้อง
    private Rigidbody rb; // Rigidbody ของตัวละคร

    void Start()
    {
        cameraTransform = freeLookCamera.transform; // รับข้อมูลตำแหน่งกล้อง
        rb = GetComponent<Rigidbody>(); // รับการอ้างอิง Rigidbody
    }

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // รับ input ของผู้เล่น
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // คำนวณทิศทางที่ตัวละครจะเคลื่อนที่
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // คำนวณทิศทางที่ต้องการเคลื่อนที่ไปตามมุมของกล้อง
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);

            // หมุนตัวละครไปในทิศทางที่ต้องการ
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);

            // เคลื่อนที่ไปข้างหน้าตามทิศทางของกล้อง
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.MovePosition(transform.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }
}
