using System.Collections;
using UnityEngine;

public class NightLight : MonoBehaviour
{
    private Light _light;

    private DayNightCycle _dayNightCycle;

    private bool fadeInAndOutCoroutineRunning;
    // Start is called before the first frame update
    void Start()
    {
        _light = GetComponent<Light>();
        _dayNightCycle = FindObjectOfType<DayNightCycle>();
        if (_light == null)
        {
            _light = GetComponentInChildren<Light>();
        }
        fadeInAndOutCoroutineRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fadeInAndOutCoroutineRunning)
        {
            if (_dayNightCycle.IsDayTime() && _light.intensity > 0f)
            {
                StartCoroutine(FadeInAndOut(false, 2f));
            }
            else if (!_dayNightCycle.IsDayTime() && _light.intensity <= 0f)
            {
                StartCoroutine(FadeInAndOut(true, 2f));
            }
        }

    }

    IEnumerator FadeInAndOut(bool fadeIn, float duration)
    {
        fadeInAndOutCoroutineRunning = true;
        float minLuminosity = 0f; // min intensity
        float maxLuminosity = 10f; // max intensity

        float counter = 0f;

        float a, b;

        if (fadeIn)
        {
            a = minLuminosity;
            b = maxLuminosity;
        }
        else
        {
            a = maxLuminosity;
            b = minLuminosity;
        }
       

        while (counter < duration)
        {
            counter += Time.deltaTime;

            _light.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }

        fadeInAndOutCoroutineRunning = false;
    }
}
