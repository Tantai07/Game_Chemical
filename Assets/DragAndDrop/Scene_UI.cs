using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene_UI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void GoToFirstScene()
    {
        SceneManager.LoadScene(0);
    }

    public void GoToNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            GoToFirstScene();
        }
    }

    public void GoToNextScene_For_Start()
    {
        StartCoroutine(ShowLoadingAndStart());
    }

    private IEnumerator ShowLoadingAndStart()
    {
        // แสดงข้อความ "Loading."
        text.text = "Loading";
        int dotCount = 0;

        for (int i = 0; i < 8; i++) // Loop รวมเวลา 8 วินาที
        {
            if (dotCount < 3)
            {
                text.text += " .";
                dotCount++;
            }
            else
            {
                // Reset " . " เมื่อครบสามครั้ง
                text.text = "Loading";
                dotCount = 0;
            }

            yield return new WaitForSeconds(1);
        }

        // เปลี่ยนข้อความเป็น "Success"
        text.text = "success";
        yield return new WaitForSeconds(1);

        // เรียกใช้ Start_Black() และเปลี่ยน Scene
        Check_Danger.instance.Start_Black();
        Invoke("GoToNextScene", 2);
    }
}
