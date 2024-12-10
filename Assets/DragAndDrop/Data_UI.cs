using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Data_UI : MonoBehaviour
{
    [Header("Text")]
    [Space(10)]
    public TextMeshProUGUI[] textArray = new TextMeshProUGUI[3];

    [Header("Info")]
    [Space(15)]
    public List<Info_Information> infoList = new List<Info_Information>();

    public void Check_Data(string Name)
    {
        var matchedText = FindInTextArray(Name);
        if (matchedText != null)
        {
           var matchedInfo = FindInInfoList(Name);
           if (matchedInfo != null)
           {
                matchedText.text = matchedInfo.Description;
           }
        }
    }
    private TextMeshProUGUI FindInTextArray(string Name)
    {
        foreach (var textElement in textArray)
        {
            if (textElement != null && textElement.text == Name)
            {
                return textElement; //ส่งกลับ text
            }
        }
        return null; //ในกรณีไม่เจอ
    }
    private Info_Information FindInInfoList(string Name)
    {
        foreach (var info in infoList)
        {
            if (info.Name == Name)
            {
                return info;//ส่งกลับ info
            }
        }
        return null;//ในกรณีไม่เจอ
    }
}
