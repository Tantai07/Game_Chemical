using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [Header("Status")]
    public string Name;
    public int PH;
    public int Mass;
    public int Volume;
    public States State;

    [Header("State of object")]
    public Sprite normal_state;
    public Sprite water_state;
    public Sprite tool_state;

    [Header("Setting")]
    public TargetTag targetTag; // แท็กเป้าหมายที่ต้องการตรวจสอบ
    public float detectionRadius = 0.5f; // รัศมีสำหรับตรวจสอบการชน

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;

    public enum States
    {
        Solid,
        Liquid,
        Tool
    }

    public enum TargetTag
    {
        Sink,
        Enemy,
        Collectible,
        Obstacle,
        Trash
    }

    private void Start()
    {
        //SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = normal_state;
        // บันทึกตำแหน่งเริ่มต้น
        originalPosition = transform.position;
        gameObjectCol = GetComponent<Collider2D>();
    }
    private void OnMouseDown()
    {
        // คำนวณ offset ระหว่างตำแหน่งของ GameObject กับตำแหน่งเมาส์
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
    }

    private void OnMouseDrag()
    {
        // อัปเดตตำแหน่ง GameObject ตามตำแหน่งของเมาส์
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mousePosition.x + offset.x, mousePosition.y + offset.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        // ใช้ OverlapCircle หรือวิธีการตรวจจับการชนจาก Collider2D ที่มี Trigger
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(targetTag.ToString())) // เช็คแท็ก
            {
                if (State == States.Liquid)
                {
                    if (gameObject.tag == "Normal")
                    {
                        gameObject.tag = "Water";

                    }
                    else if (gameObject.tag == "Water")
                    {
                        gameObject.tag = "Finished";
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
                else if (State == States.Solid)
                {

                }
                else
                {

                }
            }
            else
            {
                // หากไม่ตรงกัน กลับไปตำแหน่งเดิม
                transform.position = originalPosition;
            }
        }
    }
    public void SetTargetTag(TargetTag newTargetTag)
    {
        targetTag = newTargetTag;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
