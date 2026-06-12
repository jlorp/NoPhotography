using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractPromptUI : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public GameObject promptParent;

    public void AddPrompt(string _text)
    {
        promptParent.SetActive(true);
        popupText.text = _text;
    }

    public void RemovePrompt()
    {
        promptParent.SetActive(false);
    }
}
