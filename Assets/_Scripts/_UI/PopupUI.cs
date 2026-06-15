using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupUI : MonoBehaviour
{
    public TextMeshProUGUI popupText;
    public Image popupBackground;

    public Color failColor, heldColor, completeColor;

    public void UpdatePopupText(string _text, int _color)
    {
        popupText.text = _text;

        if (_color == 0)
        {
            popupBackground.color = failColor;
        }
        else if (_color == 1)
        {
            popupBackground.color = heldColor;
        }
        else if (_color == 2)
        {
            popupBackground.color = completeColor;
        }
    }
}
