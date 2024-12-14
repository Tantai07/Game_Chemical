using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check_Mixed : MonoBehaviour
{
    [Header("Info")]
    public List<Info_Information> info = new List<Info_Information>();

    public Sprite CheckMixed(string name)
    {
        foreach (var item in info)
        {
            if (item.Name == name) // เช็คว่าชื่อใน List ตรงกับชื่อที่รับเข้ามาหรือไม่
            {
                return item.sprite;
            }
        }
        return null;
    }
}
