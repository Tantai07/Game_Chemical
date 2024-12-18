﻿using System.Collections;
using TMPro;
using UnityEngine;

public class Count_To_Start : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI countdownText;

    [Header("Wait Time")]
    public float Time;

    public void Start_Count()
    {
        StartCoroutine(CountdownCoroutine(Time));
    }
    public IEnumerator CountdownCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Check_Danger.instance.Finish_Black();
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!!";
        yield return new WaitForSeconds(1f);

        Check_Danger.instance.StartTimer();
        gameObject.SetActive(false);
    }
}
