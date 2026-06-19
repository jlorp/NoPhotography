using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroUI : MonoBehaviour
{
    public TextMeshProUGUI introText, promptText;
    Color textColor;
    public GameObject startPrompt;
    bool startReady = false;
    bool endReady=false;
    
    public bool introDone= false;
    
    public void EndGamePopup()
    {
        StartCoroutine(TextFadeIn(1.5f, true));
        introText.text = "You found all the Items! Thanks for Playing!";
        promptText.text = "  end game";
        UIManager.Instance.EndFade();
    }

    void CloseGame()
    {
        if(!endReady) return;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    void Start()
    {
        textColor = introText.color;
        textColor.a = 0f;
        introText.color = textColor;
        StartCoroutine(TextFadeIn(2));

        Color deathColor = UIManager.Instance.deathImage.color;
        UIManager.Instance.deathImage.color = new Color(deathColor.r, deathColor.g, deathColor.b, 1); 
    }

    public void CloseIntro()
    {
        if(!startReady || introDone) return;

        StartCoroutine(TextFadeOut(1f));
        UIManager.Instance.IntroFade();

        startPrompt.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Open Camera"))
        {
            CloseIntro();
            CloseGame();
        }
    }

    IEnumerator TextFadeOut(float duration)
    {
        float time = 0f;
        yield return null; 

        while (time < duration) 
        {
            time += Time.deltaTime;
            float percentComplete = time/duration;
            textColor.a = 1 - percentComplete;
            introText.color = textColor;
            yield return null; 
        }
        textColor.a = 0;
        introText.color = textColor;
        introDone=true;
    }

    IEnumerator TextFadeIn(float duration, bool _endReady=false)
    {
        float time = 0f;
        yield return null; 

        while (time < duration) 
        {
            time += Time.deltaTime;
            float percentComplete = time/duration;
            textColor.a = percentComplete;
            introText.color = textColor;
            yield return null; 
        }
        textColor.a = 1;
        introText.color = textColor;

        startPrompt.SetActive(true);
        startReady=true;

        if (_endReady) endReady = true;
    }
}
