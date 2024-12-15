using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Stamp : MonoBehaviour
{
    [Header("Setting")]
    public Type_Stamp type; // แท็กเป้าหมายที่ต้องการตรวจสอบ

    [Header("Detection Area")]
    public float width = 1.0f;
    public float height = 0.5f;

    [Header("Stamp Sprite")]
    public Sprite OverWeight_State;
    public Sprite Danger_State;

    [Header("Stamp Script")]
    public Stamp_UI stamp;

    [Header("Stamp UI")]
    public GameObject stamp_UI;

    [Space(10)]
    public Color color;

    [Space(10)]
    public Image stamp_color;

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;

    public enum Type_Stamp
    {
        None,
        OverWeight_Stamp,
        Danger_Stamp
    }
    private void Start()
    {
        SpriteRenderer Sprite = GetComponent<SpriteRenderer>();
        if(type == Type_Stamp.OverWeight_Stamp)
        {
            Sprite.sprite = OverWeight_State;
        }
        else
        {
            Sprite.sprite = Danger_State;
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
        // กำหนดขนาดของสี่เหลี่ยมผืนผ้า
        Vector2 rectangleSize = new Vector2(width, height);

        // ตรวจสอบการชนด้วย OverlapBox
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, rectangleSize, 0);

        // คืนตำแหน่งกลับไปที่ตำแหน่งเริ่มต้น
        transform.position = originalPosition;

        foreach (Collider2D hitCollider in hitColliders)
        {
            DragAndDrop dragComponent = hitCollider.GetComponent<DragAndDrop>();
            if (dragComponent != null)
            {
                if (type == Type_Stamp.OverWeight_Stamp)
                {
                    if (dragComponent.tag == "Over_Weight")
                    {
                        stamp_UI.SetActive(true);
                        stamp_color.color = color;
                        stamp.EnterData_Danger(dragComponent.Name, dragComponent.Mass_grams, type, dragComponent.Liquid, dragComponent);
                    }
                }
                else if (type == Type_Stamp.Danger_Stamp)
                {
                    if (dragComponent.State == DragAndDrop.States.Danger && dragComponent.targetTag == DragAndDrop.TargetTag.Trash_Danger)
                    {
                        foreach (var name in Check_Danger.instance.Danger_list)
                        {
                            if (dragComponent.Name == name)
                            {
                                stamp_UI.SetActive(true);
                                stamp_color.color = color;
                                stamp.EnterData_Danger(dragComponent.Name, dragComponent.Mass_grams,type,dragComponent.Liquid,dragComponent);
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("None Stamp");
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 rectangleSize = new Vector2(width, height);
        Gizmos.DrawWireCube(transform.position, rectangleSize);
    }
}