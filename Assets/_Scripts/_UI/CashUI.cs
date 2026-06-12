using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CashUI : MonoBehaviour
{
    public TextMeshProUGUI cashText;

    public void UpdateWallet(float cashAmount)
    {
        cashText.text = cashAmount.ToString("C");
    }
}
