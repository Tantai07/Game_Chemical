using Unity.Burst.CompilerServices;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [Header("Status")]
    public string Name;
    public int PH;
    public int Mass;
    public States State;

    [Header("State of object")]
    public Sprite normal_state;
    public Sprite water_state;
    public Sprite tool_state;
    public Sprite mixed_state;

    [Header("Setting")]
    public TargetTag targetTag; // แท็กเป้าหมายที่ต้องการตรวจสอบ
    public float detectionRadius = 0.5f; // รัศมีสำหรับตรวจสอบการชน

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;
    SpriteRenderer spriteRenderer;

    public enum States
    {
        Solid,
        Liquid,
        Bucket,
        Tool
    }

    public enum TargetTag
    {
        Sink,
        Water_Bucket,
        Trash_water,
        Trash_normal,
        Trash_Danger,
        Close_Box,
        Dry_Box
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normal_state;

        if(PH == 7)
        {
            SetTargetTag(TargetTag.Trash_water);
        }
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
                    if (gameObject.tag == "Normal" && targetTag == TargetTag.Water_Bucket)
                    {
                        DragAndDrop hit = hitCollider.GetComponent<DragAndDrop>();
                        hit.spriteRenderer.sprite = hit.mixed_state;
                        hit.tag = "Mixed_Bucket";
                        hit.targetTag = TargetTag.Trash_water;
                    }

                    spriteRenderer.sprite = tool_state;
                    State = States.Tool;
                    SetTargetTag(TargetTag.Dry_Box);

                }
                else if (State == States.Bucket)
                {
                    if (gameObject.tag == "Bucket" && targetTag == TargetTag.Sink)
                    {
                        spriteRenderer.sprite = water_state;
                        gameObject.tag = "Water_Bucket";
                    }
                    else if(gameObject.tag == "Mixed_Bucket" && targetTag == TargetTag.Trash_water)
                    {
                        spriteRenderer.sprite = normal_state;
                        gameObject.tag = "Bucket";
                        targetTag = TargetTag.Sink;
                    }
                }
                else if (State == States.Solid)
                {

                }
                else
                {
                    gameObject.SetActive(false);
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
