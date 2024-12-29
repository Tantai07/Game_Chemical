using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cut_Scene_Controller : MonoBehaviour
{
    [Header("CutScene Image")]
    public List<Image> images = new List<Image>();

    [Header("Sprite Slot")]
    public List<Sprite> sprites = new List<Sprite>();

    [Header("Click Cooldown Settings")]
    public float clickCooldown;
    public float AnimationTime;
    private float nextClickTime = 0f;

    [Header("Count To Start Script")]
    public Count_To_Start count;
    public GameObject text_count;

    private int currentImageIndex = 0;

    // Singleton Instance
    public static Cut_Scene_Controller instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of Cut_Scene_Controller detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Check_Danger.instance.Finish_Black();
        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = sprites[i];
            images[i].gameObject.SetActive(false);
        }

        ShowImage(currentImageIndex);
    }

    private void Update()
    {
        if (Time.time >= nextClickTime && Input.GetMouseButtonDown(0))
        {
            nextClickTime = Time.time + clickCooldown;
            ShowNextImage();
        }
    }

    private void ShowNextImage()
    {
        currentImageIndex++;
        if (currentImageIndex > 0 && currentImageIndex % 3 == 0)
        {
            // เรียก Coroutine เพื่อรอจนกว่า FadeOut จะเสร็จ
            StartCoroutine(HidePreviousImagesAndShowNext(currentImageIndex - 3));
        }
        else if (currentImageIndex < images.Count)
        {
            ShowImage(currentImageIndex);
        }
        else
        {
            CloseCutScene();
        }
    }

    private void ShowImage(int index)
    {
        images[index].gameObject.SetActive(true);

        Animator animator = images[index].GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("Image_FadeIn");
        }
    }

    private IEnumerator HidePreviousImagesAndShowNext(int startIndex)
    {
        for (int i = startIndex; i < startIndex + 3 && i < images.Count; i++)
        {
            Animator animator = images[i].GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Image_FadeOut");
            }
        }

        yield return new WaitForSeconds(1f);

        for (int i = startIndex; i < startIndex + 3 && i < images.Count; i++)
        {
            images[i].gameObject.SetActive(false);
        }

        if (currentImageIndex < images.Count)
        {
            ShowImage(currentImageIndex);
        }
    }

    public void CloseCutScene()
    {
        Check_Danger.instance.Start_Black();

        StartCoroutine(HideGameObject());
    }

    private IEnumerator HideGameObject()
    {
        yield return new WaitForSeconds(1f);
        count.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
        Check_Danger.instance.Finish_Black();
        yield return new WaitForSeconds(AnimationTime);

    }
}
