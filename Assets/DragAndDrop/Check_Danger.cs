using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;  // เพิ่มสำหรับการใช้ TextMeshPro

public class Check_Danger : MonoBehaviour
{
    [Header("Danger Chemical List")]
    public List<string> Danger_list = new List<string>();

    [Space(15)]
    [Header("Score")]
    public int Max_score;
    [SerializeField] int Current_score;

    [Header("Bool For Can Play")]
    public bool canPlay;

    [Header("Animator of Block")]
    public Animator anim1;
    public Animator anim2;

    [Header("Timer Settings")]
    public float maxTime;
    private float currentTime;

    [Header("UI Elements")]
    public Image timerFillImage;
    public TMP_Text timerText;
    public TMP_Text elapsedTimeText;

    [Header("Timer Status")]
    public bool isRunning = false;
    public bool isTimerStopped = false;

    [Header("Text Score")]
    public TextMeshProUGUI score_text;

    [Header("Game Over UI")]
    public TextMeshProUGUI text_gameOver;
    public Image image_in_button;
    public GameObject gameover_Button;

    public static Check_Danger instance;

    private Animator anim;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    void Start()
    {
        anim = elapsedTimeText.GetComponent<Animator>();
        UpdateScore();
    }

    private void UpdateScore()
    {
        score_text.text = $"{Current_score.ToString()} / { Max_score.ToString()}";
    }

    void Update()
    {
        if (isRunning && !isTimerStopped)
        {
            UpdateTimer();
        }
    }

    public void StartTimer()
    {
        // ตั้งค่าเริ่มต้น
        currentTime = maxTime;
        timerFillImage.fillAmount = 1f;
        isRunning = true;
    }

    private void UpdateTimer()
    {
        if (currentTime >= 0 && !isTimerStopped)  // Ensure timer only decreases if it's not stopped
        {
            currentTime -= Time.deltaTime;

            timerFillImage.fillAmount = currentTime / maxTime;

            // Display time in minute:second format
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (timerFillImage.fillAmount == 0)
        {
            currentTime = 0;
            timerFillImage.fillAmount = 0f;
            isRunning = false;

            timerText.text = "00:00";

            GameOver();
        }
    }
    public void TimeStop()
    {
        isTimerStopped = true;
    }
    public void TimeResume()
    {
        isTimerStopped = false;
    }

    private void GameOver()
    {
        Animator anim_over = text_gameOver.GetComponent<Animator>();
        anim_over.Play("GameOver");
        Start_Black();
        gameover_Button.SetActive(true);
    }
    public void ShowButton()
    {
        Animator anim_button = gameover_Button.GetComponent<Animator>();
        Animator anim_image = image_in_button.GetComponent<Animator>();
        anim_button.Play("Return_Show_Up");
        anim_image.Play("Return_Show_Up");
    }

    public void ReduceTime(float amount)
    {
        anim.Play("RiseText");
        if (isRunning && currentTime > 0)
        {
            currentTime -= amount;

            // ป้องกันไม่ให้เวลาติดลบ
            if (currentTime < 0)
            {
                currentTime = 0;
            }

            timerFillImage.fillAmount = currentTime / maxTime;
        }
    }

    public void Add_Score(int score)
    {
        Current_score += score;
        UpdateScore();
        if (Current_score >= Max_score)
        {
            Current_score = 0;
            TimeStop();
            Invoke("Start_Black", 3);
        }
    }

    public void Finish_Black()
    {
        anim1.Play("Block_1_Back");
        anim2.Play("Block_2_Back");
    }

    public void Start_Black()
    {
        anim1.Play("Block_1_Start");
        anim2.Play("Block_2_Start");
    }
}
