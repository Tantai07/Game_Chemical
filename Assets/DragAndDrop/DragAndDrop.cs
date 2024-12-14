using Unity.Burst.CompilerServices;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    [Header("Status")]
    public string Name;
    public int PH;
    public int Mass_grams;
    public States State;

    [Header("State of object")]
    public Sprite normal_state;
    public Sprite water_state;
    public Sprite not_clean_tool_state;
    public Sprite cleaned_tool_state;
    public Sprite packed_state;

    [Space(10)]
    public Sprite danger_state;

    [Header("Setting")]
    public TargetTag targetTag; // แท็กเป้าหมายที่ต้องการตรวจสอบ
    public float detectionRadius = 0.5f; // รัศมีสำหรับตรวจสอบการชน

    [Header("Stamp")]
    public bool Over_Weight;
    public bool Normal_Solid;
    public bool Danger;

    [Space(10)]
    public bool Tool_Danger;

    private Vector3 originalPosition;
    private Vector3 offset;
    private Collider2D gameObjectCol;
    SpriteRenderer spriteRenderer;

    public enum States
    {
        Solid,
        Liquid,
        Bucket,
        Danger,
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
        Dry_Box,
        Pack_Box
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = normal_state;

        switch (State)
        {
            case States.Tool:
                spriteRenderer.sprite = not_clean_tool_state;
                targetTag = TargetTag.Sink;
                break;

            case States.Liquid:
                if (PH == 7)
                {
                    targetTag = TargetTag.Trash_water;
                }
                else if (PH > 7 || PH < 7)
                {
                    targetTag = TargetTag.Water_Bucket;
                }
                break;

            case States.Danger:
                targetTag = TargetTag.Trash_Danger;
                Tool_Danger = true;
                break;

            case States.Bucket:
                targetTag = TargetTag.Sink;
                break;

            case States.Solid:
                targetTag = TargetTag.Pack_Box;

                if(Mass_grams >= 1000)
                {
                    gameObject.tag = "Over_Weight";
                    Over_Weight = true;
                }
                else
                {
                    gameObject.tag = "Normal";
                    Normal_Solid = true;
                }
                // ตรวจสอบใน Danger_list
                foreach (var name in Check_Danger.instance.Danger_list)
                {
                    if (Name == name)
                    {
                        State = States.Danger;
                        Danger = true;
                        break;
                    }
                }
                break;
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
                switch (State)
                {
                    case States.Liquid:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Water_Bucket)
                        {
                            DragAndDrop hit = hitCollider.GetComponent<DragAndDrop>();
                            Check_Mixed check = hitCollider.GetComponent<Check_Mixed>();
                            hit.tag = "Mixed_Bucket";
                            hit.targetTag = TargetTag.Trash_water;
                            hit.spriteRenderer.sprite = check.CheckMixed(Name);
                        }
                        spriteRenderer.sprite = not_clean_tool_state;
                        State = States.Tool;
                        targetTag = TargetTag.Sink;
                        break;

                    case States.Bucket:
                        if (gameObject.tag == "Bucket" && targetTag == TargetTag.Sink)
                        {
                            spriteRenderer.sprite = water_state;
                            gameObject.tag = "Water_Bucket";
                        }
                        else if (gameObject.tag == "Mixed_Bucket" && targetTag == TargetTag.Trash_water)
                        {
                            spriteRenderer.sprite = normal_state;
                            gameObject.tag = "Bucket";
                            targetTag = TargetTag.Sink;
                        }
                        break;

                    case States.Solid:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = packed_state;
                            gameObject.tag = "Packed";
                        }
                        /* else if (gameObject.tag == "Packed" && targetTag == TargetTag.SomeTarget && Normal_Solid) 
                        {
                            gameObject.SetActive(false);
                        } */
                        else if (gameObject.tag == "Over_Weight" && targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = packed_state;
                        }
                        /* else if (gameObject.tag == "Over_Weight" && targetTag == TargetTag.SomeTarget && Over_Weight) 
                        {
                            gameObject.SetActive(false);
                        } */
                        break;

                    case States.Danger:
                        if (targetTag == TargetTag.Trash_Danger && (Tool_Danger || Danger))
                        {
                            gameObject.SetActive(false);
                        }
                        else if (targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = danger_state;
                            targetTag = TargetTag.Trash_Danger;
                        }
                        break;

                    case States.Tool:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Sink)
                        {
                            spriteRenderer.sprite = cleaned_tool_state;
                            targetTag = TargetTag.Dry_Box;
                        }
                        else
                        {
                            gameObject.SetActive(false);
                        }
                        break;

                    default:
                        Debug.LogWarning("ไม่พบข้อมูล");
                        break;
                }
            }
            else
            {
                // หากไม่ตรงกัน กลับไปตำแหน่งเดิม
                transform.position = originalPosition;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
