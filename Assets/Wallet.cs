using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;

    public float cash = 0;

    void Awake()
    {
        Instance = this;
    }
    public void AddCash(float cashToAdd)
    {
        cash += cashToAdd;
        UIManager.Instance.UpdateWallet(cash);
    }
}
