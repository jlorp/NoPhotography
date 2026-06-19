using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IntroUI : MonoBehaviour
{
    public TextMeshProUGUI introText;
    Color textColor;
    public GameObject startPrompt;
    bool startReady = false;

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
        if(!startReady) return;

        StartCoroutine(TextFadeOut(1f));
        UIManager.Instance.IntroFade();

        this.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Shutter") || Input.GetButtonDown("Shutter Mouse") || Input.GetButtonDown("Jump"))
        {
            CloseIntro();
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
    }

    IEnumerator TextFadeIn(float duration)
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
    }
}
