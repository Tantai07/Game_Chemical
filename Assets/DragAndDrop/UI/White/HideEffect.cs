using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideEffect : MonoBehaviour
{
    public Animator animator;
    public void Show()
    {
        animator.Play("Show_Up");
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
