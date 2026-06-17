using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterEffect : MonoBehaviour
{

    [SerializeField] GameObject waterFX;
    public Collider waterFogTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other == waterFogTrigger)
        {
            RenderSettings.fog = true;
            waterFX.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other == waterFogTrigger)
        {
            RenderSettings.fog = false;
            waterFX.SetActive(false);
        }
    }
}
