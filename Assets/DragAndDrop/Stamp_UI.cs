using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static Stamp;

public class Stamp_UI : MonoBehaviour
{
    [Header("Input Data")]
    public string Name;
    public int Mass;
    public int Value;
    public Type_Stamp Type;
    public DragAndDrop Component;

    [Header("Image")]
    public Image image;

    [Header("Stamp Object")]
    public GameObject Danger_Stamp_gameobject;
    public GameObject OverWeight_Stamp_gameobject;

    [Header("Color")]
    public Color correct_color;
    public Color incorrect_color;

    [Header("Set to Hide")]
    public GameObject Danger_Zone;
    public GameObject Solid_Zone;

    [Header("Dropdown TextMeshPro")]
    public TMP_Dropdown drop;

    [Header("Check liquid")]
    [SerializeField] bool liquid;

    [Header("Input field And Text For Check")]
    [SerializeField] TMP_InputField input_Name;
    [SerializeField] TMP_InputField input_Mass;
    [SerializeField] TextMeshProUGUI text_Name;
    [SerializeField] TextMeshProUGUI text_Mass;
    [SerializeField] TextMeshProUGUI text_Dropdown;

    [Header("Check Box")]
    public bool check_Name;
    public bool check_Mass;
    public bool check_Dropdown;

    private Dictionary<string, int> chemicalValues = new Dictionary<string, int>()
    {
        { "Na", 1 },
        { "C2H6O", 1 },
        { "C3H8", 1 },
        { "H2SO4", 4 },
        { "NaOH", 4 },
        { "Cl", 6 }
    };
    public void Dropdown(int index)
    {
        if (index >= 0 && index < drop.options.Count)
        {
            // กำหนด Sprite จาก option ที่เลือก
            image.sprite = drop.options[drop.value].image;
        }
    }
    public void EnterData_Danger(string name,int mass, Type_Stamp type, bool isliquid,DragAndDrop component)
    {
        Name = name;
        Mass = mass;
        Component = component;
        Type = type;
        // เช็คค่าจาก Dictionary โดยใช้ชื่อสาร
        if(type == Type_Stamp.Danger_Stamp)
        {
            Check_Dic(name);
        }
        else
        {
            Danger_Zone.SetActive(false);
            check_Dropdown = true;
        }

        if (isliquid)
        {
            liquid = true;
            Solid_Zone.SetActive(false);
            check_Mass = true;
        }
    }
    private void Check_Dic(string name)
    {
        if (chemicalValues.ContainsKey(name))
        {
            Value = chemicalValues[name];
        }
        else
        {
            Debug.Log("สารที่เลือกไม่อยู่ใน Dictionary");
        }
    }
    public void Sumbit()
    {

        string input_name = input_Name.text;
        if(Name == input_name)
        {
            text_Name.text = "ถูกต้อง";
            text_Name.color = correct_color;
            check_Name = true;
        }
        else
        {
            text_Name.text = "ผิด";
            text_Name.color = incorrect_color;
            check_Name = false;
        }

        if(Type == Type_Stamp.Danger_Stamp)
        {
            if (liquid)
            {
                Check_Value();
            }
            else
            {
                Check_Value();
                Check_Mass();
            }
        }
        else
        {
            Check_Mass();
        }

        //หลังจากทุกอย่างเสร็จ
        if(check_Mass && check_Name && check_Dropdown)
        {
            GameObject stamp;
            if (Type == Type_Stamp.Danger_Stamp)
            {
                Component.Danger = true;
                stamp =  Instantiate(Danger_Stamp_gameobject, Component.gameObject.transform);
            }
            else
            {
                Component.targetTag = DragAndDrop.TargetTag.Close_Box;
                stamp =  Instantiate(OverWeight_Stamp_gameobject, Component.gameObject.transform);
            }

            Component.gameObject.transform.SetParent(stamp.transform);
            CloseUI();
        }
    }
    public void CloseUI()
    {
        Name = null;
        Mass = 0;
        Value = 0;
        Type = 0;

        text_Name.text = "";
        text_Dropdown.text = "";
        text_Mass.text = "";

        check_Dropdown = false;
        check_Name = false;
        check_Mass = false;

        liquid = false;
        Component = null;

        gameObject.SetActive(false);
    }

    private void Check_Value()
    {
        if (Value == drop.value)
        {
            text_Dropdown.text = "ถูกต้อง";
            text_Dropdown.color = correct_color;
            check_Dropdown = true;
        }
        else
        {
            text_Dropdown.text = "ผิด";
            text_Dropdown.color = incorrect_color;
            check_Dropdown = false;
        }
    }
    private void Check_Mass()
    {
        string input_mass = input_Mass.text;
        if (Mass.ToString() == input_mass)
        {
            text_Mass.text = "ถูกต้อง";
            text_Mass.color = correct_color;
            check_Mass = true;
        }
        else
        {
            text_Mass.text = "ผิด";
            text_Mass.color = incorrect_color;
            check_Mass = false;
        }
    }
}
