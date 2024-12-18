﻿using Unity.Burst.CompilerServices;
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
    public Vector2 detectionSize;

    [Header("Stamp")]
    public bool Over_Weight;
    public bool Danger;

    [Space(10)]
    public bool Normal_Solid;

    [Header("Past State")]
    public bool Solid;
    public bool Liquid;

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
        Tool,
        Save
    }

    public enum TargetTag
    {
        Sink,
        Water_Bucket,
        Trash_Over_Weight,
        Trash_water,
        Trash_Danger,
        Close_Box,
        Dry_Box,
        Pack_Box,
        Save_Box
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

            case States.Save:
                spriteRenderer.sprite = normal_state;
                targetTag = TargetTag.Save_Box;
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

                foreach (var name in Check_Danger.instance.Danger_list)
                {
                    if (Name == name)
                    {
                        State = States.Danger;
                        targetTag = TargetTag.Pack_Box;
                        Liquid = true;
                        break;
                    }
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
                        gameObject.tag = "Normal";
                        State = States.Danger;
                        Solid = true;
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
        Collider2D[] hitColliders = Physics2D.OverlapBoxAll(transform.position, detectionSize, 0f);

        bool found = false;  // ตัวแปรนี้ใช้เช็คว่าเจอ object แล้วหรือยัง

        foreach (Collider2D hitCollider in hitColliders)
        {
            // เช็คว่า collider ที่เจอไม่ใช่ตัวมันเองและตรงตามแท็กที่ต้องการ
            if (!found && hitCollider.gameObject != gameObject && hitCollider.CompareTag(targetTag.ToString()))
            {
                found = true;  // เมื่อเจอแล้วให้บันทึกว่าเจอแล้ว

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
                        transform.position = originalPosition;
                        break;

                    case States.Bucket:
                        if (gameObject.tag == "Bucket" && targetTag == TargetTag.Sink)
                        {
                            spriteRenderer.sprite = water_state;
                            gameObject.tag = "Water_Bucket";
                            transform.position = originalPosition;
                        }
                        else if (gameObject.tag == "Mixed_Bucket" && targetTag == TargetTag.Trash_water)
                        {
                            spriteRenderer.sprite = normal_state;
                            gameObject.tag = "Bucket";
                            targetTag = TargetTag.Sink;
                            transform.position = originalPosition;
                        }
                        break;

                    case States.Solid:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = packed_state;
                            gameObject.tag = "Packed";
                            targetTag = TargetTag.Close_Box;
                            transform.position = originalPosition;
                        }
                        else if (gameObject.tag == "Packed" && targetTag == TargetTag.Close_Box && Normal_Solid)
                        {
                            Check_Danger.instance.Add_Score(1);
                            gameObject.SetActive(false);
                        }
                        else if (gameObject.tag == "Over_Weight" && targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = packed_state;
                            transform.position = originalPosition;
                        }
                        else if (gameObject.tag == "Over_Weight" && targetTag == TargetTag.Trash_Over_Weight && Over_Weight)
                        {
                            Check_Danger.instance.Add_Score(1);
                            gameObject.SetActive(false);
                        }
                        break;

                    case States.Danger:
                        if (targetTag == TargetTag.Trash_Danger && (Tool_Danger || Danger))
                        {
                            Check_Danger.instance.Add_Score(1);
                            gameObject.SetActive(false);
                        }
                        else if (targetTag == TargetTag.Pack_Box)
                        {
                            spriteRenderer.sprite = danger_state;
                            targetTag = TargetTag.Trash_Danger;
                            transform.position = originalPosition;
                        }
                        break;

                    case States.Tool:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Sink)
                        {
                            spriteRenderer.sprite = cleaned_tool_state;
                            targetTag = TargetTag.Dry_Box;
                            transform.position = originalPosition;
                        }
                        else
                        {
                            Check_Danger.instance.Add_Score(1);
                            gameObject.SetActive(false);
                        }
                        break;

                    case States.Save:
                        if (gameObject.tag == "Normal" && targetTag == TargetTag.Save_Box)
                        {
                            Check_Danger.instance.Add_Score(1);
                            gameObject.SetActive(false);
                        }
                        break;

                    default:
                        Debug.LogWarning("ไม่พบข้อมูล");
                        break;
                }

                break;  // ออกจากลูปทันทีเมื่อเจอ object ตัวแรกที่ตรงเงื่อนไข
            }
        }

        if (!found)
        {
            Check_Danger.instance.ReduceTime(10);
            // หากไม่เจอ object ใดในวงนี้ กลับไปตำแหน่งเดิม
            transform.position = originalPosition;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, detectionSize);
    }
}
