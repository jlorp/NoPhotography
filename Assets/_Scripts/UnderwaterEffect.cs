using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffect : MonoBehaviour
{

    [SerializeField] GameObject waterFX;

    private void OnTriggerEnter(Collider other)
    {
        RenderSettings.fog = true;
        waterFX.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        RenderSettings.fog = false;
        waterFX.SetActive(false);
    }
}
