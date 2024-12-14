using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Check_Danger : MonoBehaviour
{
    [Header("Danger Chemical List")]
    public List<string> Danger_list = new List<string>();

    [Space(15)]
    [Header("Score")]
    public int Max_score;
    [SerializeField] int Current_score;

    public static Check_Danger instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void Add_Score(int score)
    {
        Current_score += score;
        if (Current_score >= Max_score)
        {
            Current_score = 0;
            //เปลี่ยนด่าน
        }
    }
}
