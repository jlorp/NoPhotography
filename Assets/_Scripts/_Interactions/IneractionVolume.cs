using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IneractionVolume : MonoBehaviour
{
    public bool isActive = false;
    Transform player;
    Rigidbody body;
    public Transform clawDownPosition;
    public AnimationCurve _smoothCurve;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            player = _player.transform;
            body = _player.body;

            if (_player.body.isKinematic) return;
            isActive = true;
            UIManager.Instance.AddInteractPrompt("   to dock Sub");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<MovingSphere>(out MovingSphere _player))
        {
            isActive = false;
            UIManager.Instance.RemoveInteractPrompt();
        }
    }
    
    public void Update()
    {
        if(Input.GetButtonDown("Jump") && isActive)
        {
            UIManager.Instance.RemoveInteractPrompt();
            isActive = false;

            StartCoroutine(LerpToPoint(1.5f));
        }
    }

    IEnumerator LerpToPoint(float duration)
    {
        Vector3 startPoint = player.position;
        Quaternion startRotation = player.localRotation;
        body.isKinematic = true;

        float time = 0;
        bool playedanimation = false;
        while (time < duration) 
        {
            float percentComplete = time/duration;
            float smoothedPosition = _smoothCurve.Evaluate(percentComplete);
            player.position = Vector3.Lerp(startPoint, clawDownPosition.position, smoothedPosition);
            player.localRotation = Quaternion.Lerp(startRotation, clawDownPosition.localRotation, smoothedPosition);

            if(time > .5f && !playedanimation)
            {
                ClawLogic.Instance._animaiton.Play("SubGrab");
                playedanimation=true;
            }
            time += Time.deltaTime;
            yield return null; 
        }

        OrbitCamera.Instance.DeactivateCamera();
        OrbitCamera.Instance.GoToStartPosition();

        UIManager.Instance.subControlPrompts.SetActive(false);
		UIManager.Instance.pauseMenuUI.SetActive(false);
    }
}
