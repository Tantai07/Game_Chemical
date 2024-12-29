using UnityEngine;

public class Zoom : MonoBehaviour
{
    public Camera mainCamera;  // กล้องหลัก
    public float zoomSpeed = 10f;  // ความเร็วในการซูม
    public float minZoom = 10f;  // ซูมเข้าต่ำสุด
    public float maxZoom = 50f;  // ซูมออกสูงสุด
    public float moveSpeed = 5f;  // ความเร็วในการเคลื่อนที่ของกล้อง

    private Vector3 targetPosition;  // ตำแหน่งเป้าหมายที่กล้องจะเคลื่อนที่ไป
    private float targetZoom;  // การซูมเป้าหมาย
    private Vector3 initialPosition;  // ตำแหน่งเริ่มต้นของกล้อง

    // ขอบเขตการเคลื่อนที่ของกล้อง
    public float minX = -10f, maxX = 10f;  // ขอบเขตแกน X
    public float minY = -10f, maxY = 10f;  // ขอบเขตแกน Y

    private void Start()
    {
        // กำหนดตำแหน่งเริ่มต้น
        initialPosition = transform.position;
        targetPosition = initialPosition;
        targetZoom = mainCamera.orthographicSize;
    }

    private void Update()
    {
        if (!Check_Danger.instance.canPlay)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = transform.position.z;  // คงค่าของแกน Z
            targetPosition = mouseWorldPosition;  // ตั้งเป้าหมายการเคลื่อนที่ของกล้อง
            targetZoom = Mathf.Clamp(mainCamera.orthographicSize - 2f, minZoom, maxZoom); // ซูมเข้าด้วยคลิกขวา
        }

        // เช็คการปล่อยคลิกขวา
        if (Input.GetMouseButtonUp(1))
        {
            targetPosition = initialPosition;  // กลับไปตำแหน่งเริ่มต้น
            targetZoom = maxZoom;  // รีเซ็ตซูมกลับไปสูงสุด
        }

        // เคลื่อนที่กล้องไปยังตำแหน่งเป้าหมาย
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);

        // ซูมกล้องไปยังค่าที่ตั้งเป้าหมาย
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);

        // ตรวจสอบและจำกัดตำแหน่งกล้องในขอบเขตที่กำหนด
        float clampedX = Mathf.Clamp(transform.position.x, minX + mainCamera.orthographicSize * mainCamera.aspect, maxX - mainCamera.orthographicSize * mainCamera.aspect);
        float clampedY = Mathf.Clamp(transform.position.y, minY + mainCamera.orthographicSize, maxY - mainCamera.orthographicSize);

        // อัปเดตตำแหน่งกล้องให้อยู่ในขอบเขต
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void OnDrawGizmos()
    {
        // วาดกรอบขอบเขตใน Unity Editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(new Vector3((minX + maxX) / 2, (minY + maxY) / 2, transform.position.z),
            new Vector3(maxX - minX, maxY - minY, 0));

        // วาดตำแหน่งเริ่มต้นของกล้อง
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(initialPosition, 0.5f);
    }
}
