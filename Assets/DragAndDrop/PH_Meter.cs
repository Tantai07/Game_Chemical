using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PH_Meter : MonoBehaviour
{
    [Header("Setting")]
    public float detectionRadius = 0.5f; // รัศมีสำหรับตรวจสอบการชน

    [Header("Script")]
    [Space(10)]
    public Data_UI data;

    [Header("Point")]
    public Transform point;

    [Header("Color")]
    public Color acid_low_color;
    public Color acid_high_color;
    public Color base_low_color;
    public Color base_high_color;
    public Color mid_color;

    [Header("pH Test GameObject")]
    public Image ph_object;

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;

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
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(point.position, detectionRadius);
        transform.position = originalPosition;

        foreach (Collider2D hitCollider in hitColliders)
        {
            DragAndDrop dragComponent = hitCollider.GetComponent<DragAndDrop>();
            if(dragComponent != null && dragComponent.Name != null && dragComponent.State == DragAndDrop.States.Liquid)
            {
                string dragName = dragComponent.Name;
                int ph = dragComponent.PH;
                data.Check_Data(dragName,ph);

                UpdateColor(ph);
            }
        }
    }
    private void UpdateColor(int ph)
    {
        if (ph < 7)
        {
            if (ph < 4)
            {
                ph_object.color = acid_low_color;
            }
            else
            {
                ph_object.color = acid_high_color;
            }
        }
        else if (ph > 8)
        {
            if (ph < 12)
            {
                ph_object.color = base_low_color;
            }
            else
            {
                ph_object.color = base_high_color;
            }
        }
        else
        {
            ph_object.color = mid_color;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(point.position, detectionRadius);
    }
}
