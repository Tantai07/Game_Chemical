using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Menu_Main : MonoBehaviour
{
    [Header("UI")]
    public GameObject UI;

    [Header("Apply_For_Time_Stop")]
    public bool check;

    public void OpenUI()
    {
        if (check)
        {
            Time.timeScale = 0f;
        }
        UI.SetActive(true);
    }
    public void CloseUI()
    {
        if (check)
        {
            Time.timeScale = 1f;
        }
        UI.SetActive(false);
    }
}
