using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PH_Meter : MonoBehaviour
{
    [Header("Setting")]
    public TargetTag targetTag; // แท็กเป้าหมายที่ต้องการตรวจสอบ
    public float detectionRadius = 0.5f; // รัศมีสำหรับตรวจสอบการชน

    [Header("Script")]
    [Space(10)]
    public Data_UI data;

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;

    public enum TargetTag
    {
        Player,
        Enemy,
        Collectible,
        Obstacle
    }

    private void Start()
    {
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
        transform.position = originalPosition;

        foreach (Collider2D hitCollider in hitColliders)
        {
            DragAndDrop dragComponent = hitCollider.GetComponent<DragAndDrop>();
            if(dragComponent != null)
            {
                string dragName = dragComponent.Name;
                int ph = dragComponent.PH;
                data.Check_Data(dragName,ph);
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
