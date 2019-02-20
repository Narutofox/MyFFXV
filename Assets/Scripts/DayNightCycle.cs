using System.Collections;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Transform Sun;
    public Transform Moon;
    public Light SunLight;

    public int OrbitAngel = 0;
    public int SunPosition = 0;
    public int DayProgress = 1;

    private bool fadeInAndOutCoroutineRunning;
    private readonly int _distanceFromOrigin = 10;
    private float _daySpeedCycle = 0.01f;
    private float _nightSpeedCycle = 0.01f;
    private NightEnemySpawnPoint[] nightEnemySpawnPoints;
    public bool Day
    {
        get { return _day; }
        set
        {
            OnDayTimeChange(value);
            _day = value;
        }
    }

    private void OnDayTimeChange(bool isDay)
    {
        if (_day != isDay)
        {
            if (_daySpeedCycle > 0 && _nightSpeedCycle > 0)
            {
                _daySpeedCycle += 0.001f;
                _nightSpeedCycle -= 0.001f;
            }

            if (nightEnemySpawnPoints == null)
            {
                nightEnemySpawnPoints = FindObjectsOfType<NightEnemySpawnPoint>();
            }

            foreach (var nightEnemySpawnPoint in nightEnemySpawnPoints)
            {
                if (isDay)
                {
                    nightEnemySpawnPoint.DestroySpawendEnemies();
                }

                nightEnemySpawnPoint.gameObject.SetActive(!isDay);
            }
        }
    }

    private bool _day;

    // Start is called before the first frame update
    void Start()
    {
        SetUpOrbitals();
        SetUpOrbitalPath();
        fadeInAndOutCoroutineRunning = false;
        nightEnemySpawnPoints = null;
    }

    private void SetUpOrbitalPath()
    {
        transform.rotation = Quaternion.Euler(OrbitAngel,SunPosition,DayProgress);        
    }

    private void SetUpOrbitals()
    {
        Vector3 distanceFromOriginVector = new Vector3(_distanceFromOrigin, 0,0);
        Sun.position = distanceFromOriginVector;
        Moon.position = -distanceFromOriginVector;

        Sun.rotation = Quaternion.Euler(0, -90, 0);
        Moon.rotation = Quaternion.Euler(0, 90, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Day = IsDayTime();

        transform.Rotate(0, 0, Day ? _daySpeedCycle : _nightSpeedCycle);

        if (!fadeInAndOutCoroutineRunning)
        {
            float zPosition = WrapAngle(transform.localEulerAngles.z);
            if (zPosition > 0 && SunLight.intensity < 0.5)
            {
                StartCoroutine(FadeInAndOut(SunLight, true, 2f));
            }
            else if (zPosition < 0 && SunLight.intensity > 0)
            {
                StartCoroutine(FadeInAndOut(SunLight, false, 2f));
            }
        }
        
    }

    public bool IsDayTime()
    {
        return WrapAngle(transform.localEulerAngles.z) >= 0;
    }

    private float WrapAngle(float angle)
    {
        angle %= 360;
        if (angle > 180)
            return angle - 360;

        return angle;
    }

    IEnumerator FadeInAndOut(Light lightToLerp, bool fadeIn, float duration)
    {
        fadeInAndOutCoroutineRunning = true;
        float minLuminosity = 0f; // min intensity
        float maxLuminosity = 1f; // max intensity

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

            lightToLerp.intensity = Mathf.Lerp(a, b, counter / duration);

            yield return null;
        }

        fadeInAndOutCoroutineRunning = false;
    }
}
