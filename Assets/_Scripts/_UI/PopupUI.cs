using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI popupText;

    public void UpdatePopupText(string _text)
    {
        popupText.text = _text;
    }
}
