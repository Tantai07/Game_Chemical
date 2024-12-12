using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Data_UI : MonoBehaviour
{
    [Header("Menu")]
    public GameObject Doc_Menu;

    [Space(15)]
    [Header("Text")]
    [Space(10)]
    public TextMeshProUGUI[] textArray = new TextMeshProUGUI[3];

    public void Check_Data(string Name,int PH)
    {
        var matchedText = FindInTextArray(Name);
        if (matchedText != null)
        {
           matchedText.text = PH.ToString();
        }
    }
    public void OpenMenu()
    {
        Doc_Menu.SetActive(true);
    }
    public void CloseMenu()
    {
        Doc_Menu.SetActive(false);
    }
    private TextMeshProUGUI FindInTextArray(string Name)
    {
        foreach (var textElement in textArray)
        {
            if (textElement != null)
            {
                Text_Name text = textElement.GetComponent<Text_Name>();
                if (text.text_Name == Name)
                {
                    return textElement; //ส่งกลับ text
                }
            }
        }
        return null; //ในกรณีไม่เจอ
    }
}
