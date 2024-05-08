using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    public GameObject player;
    public Image Image;
    private float fadeDuration = 3;
    enum fadeType { None, FadeIn, FadeOut }
    fadeType fadeMode = fadeType.None;
    float timer = 0;

    public void fadeToBlack(float fadeTime)
    {
        fadeDuration = fadeTime;
        timer = 0;
        fadeMode = fadeType.FadeIn;
    }
    private void fade(Color start, Color end)
    {
        // Increment timer and update fade colour
        timer += Time.deltaTime;
        float lerpFactor = timer / fadeDuration;
        Image.color = Color.Lerp(start, end, lerpFactor);

        // If fade duration has passed, reset timer and correct vars
        if (timer < fadeDuration) return;
        timer = 0;
        switch (fadeMode)
        {
            case fadeType.None:
                break;
            case fadeType.FadeIn:
                fadeMode = fadeType.FadeOut;
                player.transform.Rotate(-player.transform.rotation.eulerAngles);
                CharacterController controller = player.GetComponent<CharacterController>();
                controller.enabled = false;
                controller.transform.position = new Vector3(0.13f, 3.5f, -29.0f);
                controller.enabled = true;
                player.GetComponent<DayCycle>().exhaustionMeter = 0;
                break;
            case fadeType.FadeOut:
                fadeMode = fadeType.None;
                break;
            default:
                Debug.LogError("INVALID FADEMODE: Fader.fade()");
                break;
        }
    }
    public void Update()
    {
        switch (fadeMode)
        {
            case fadeType.None:
                break;
            case fadeType.FadeIn:
                fade(Color.clear, Color.black);
                break;
            case fadeType.FadeOut:
                fade(Color.black, Color.clear);
                break;
            default:
                Debug.LogError("INVALID FADEMODE: Fader.Update()");
                break;
        }
    }
}