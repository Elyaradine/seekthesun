using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WhiteFlash : MonoBehaviour
{
    public string AnimName = "whiteFadeReverse";
    private Animation _anim;

    void Awake()
    {
        _anim = GetComponent<Animation>();
    }

    public void PlayAnimation()
    {
        _anim.Play(AnimName);
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("PlantScene");
    }
}
