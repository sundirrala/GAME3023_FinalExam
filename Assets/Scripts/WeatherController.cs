using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[System.Serializable]
public class WeatherController : MonoBehaviour
{
    // --------- states defined -------------------- //
    public enum WEATHERSTATES
    {
        RAIN,
        THUNDERSTORM,
        OVERCAST,
        SUNNY
    }

    // --------- VARIABLES defined ------------------- //

    [SerializeField]
    GameObject clouds1;
    [SerializeField]
    GameObject clouds2;

    [Header("Managing States")]
    [SerializeField]
    private float speedTime;
    [SerializeField]
    private WEATHERSTATES states = WEATHERSTATES.SUNNY;
    [SerializeField]
    float timer = 10.0f;
    [SerializeField]
    private bool isTransitioningWeather;
    [SerializeField]
    private bool isTransitioning;
    [SerializeField]
    private float transitionTime;
    [SerializeField]
    private WEATHERSTATES[] difWeather;
    private int weatherStates;
    private float weatherTime;
    private float weatherCycle;
    [SerializeField]
    private float transitionLength;

    [Header("Rain Section")]
    [SerializeField]
    ParticleSystem rain;
    [SerializeField]
    private AudioSource dripdrop;

    [Header("Sunny Section")]
    [SerializeField]
    private Color sunColor;
    [SerializeField]
    private Light2D sunlight;
    [SerializeField]
    private AudioSource birds;

    [Header("Overcast Section")]
    [SerializeField]
    private ParticleSystem windParticle;
    [SerializeField]
    private Color overcast;
    [SerializeField]
    private AudioSource wind;

    [Header("Thunderstorm Section")]
    [SerializeField]
    private Light2D lightning;
    [SerializeField]
    private AudioSource zap;

    [SerializeField]
    private AudioSource[] weatherAudioSources;

   

    // ------------- Start is called before the first frame update ------------ //
    void Start()
    {
        speedTime = 1.0f;
    }

    private void Update()
    {
        Time.timeScale = speedTime;
    }

    // ----------  Update is called once per frame ------------ //
    void FixedUpdate()
    {
        float deltatime = Time.deltaTime;
        timer -= Time.deltaTime;

        if (!isTransitioning)
        {
            weatherTime += Time.deltaTime;
            if(weatherTime > weatherCycle)
            {
                weatherTime = 0.0f;
                Transitions();
                isTransitioningWeather = true;
            }
        }
        else
        {
            transitionTime += Time.deltaTime;
            if(transitionTime > transitionLength)
            {
                transitionTime = 0.0f;
                isTransitioning = false;
                isTransitioningWeather = false;
            }
        }

        Debug.Log("Calling AUDIO");
        StartCoroutine(WeatherAudioManager());
    }

    private void Transitions()
    {
        isTransitioning = true;
        if(weatherStates == difWeather.Length -1)
        {
            weatherStates = 0;
        }
        else
        {
            weatherStates += 1;
        }

        states = difWeather[weatherStates];

        DifferentWeatherStates();
    }

    private void DifferentWeatherStates()
    {
        float deltatime = Time.deltaTime;

        if (timer <= 0.0f)
        {
            switch (states)
            {
                case WEATHERSTATES.RAIN:
                    rain.gameObject.SetActive(true);
                    windParticle.gameObject.SetActive(false);
                    clouds1.SetActive(true);
                    clouds2.SetActive(true);
                    break;
                case WEATHERSTATES.THUNDERSTORM:
                    LightningEffect();
                    rain.gameObject.SetActive(true);
                    clouds1.SetActive(true);
                    clouds2.SetActive(true);
                    sunlight.color = overcast;
                    break;
                case WEATHERSTATES.OVERCAST:
                    windParticle.gameObject.SetActive(true);
                    rain.gameObject.SetActive(false);
                    clouds1.SetActive(true);
                    clouds2.SetActive(true);
                    sunlight.color = overcast;
                    break;
                case WEATHERSTATES.SUNNY:
                    windParticle.gameObject.SetActive(false);
                    rain.gameObject.SetActive(false);
                    sunlight.color = sunColor;
                    clouds1.SetActive(false);
                    clouds2.SetActive(false);
                    break;
            }
            timer = 10.0f;
        }

       
    }
  
    private void LightningEffect()
    {
        float chance = UnityEngine.Random.Range(0.0f, 1.0f);
        if(chance < 0.01)
        {
            Instantiate(lightning, new Vector3(1, 1, 1), Quaternion.identity);
        }
    }

    IEnumerator WeatherAudioManager()
    {
        float deltatime = Time.deltaTime;

        switch (states)
        {
            case WEATHERSTATES.RAIN:
            case WEATHERSTATES.THUNDERSTORM:
                weatherAudioSources[(int)WEATHERSTATES.RAIN].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.RAIN].volume, .5f, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume, 0, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume, 0, deltatime);
                break;
            case WEATHERSTATES.OVERCAST:
                weatherAudioSources[(int)WEATHERSTATES.RAIN].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.RAIN].volume, 0, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume, .5f, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume, 0, deltatime);
                break;
            case WEATHERSTATES.SUNNY:
                weatherAudioSources[(int)WEATHERSTATES.RAIN].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.RAIN].volume, 0, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.OVERCAST].volume, 0, deltatime);
                weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume = Mathf.Lerp(weatherAudioSources[(int)WEATHERSTATES.SUNNY].volume, .5f, deltatime);
                break;
        }
        
        yield return new WaitForSeconds(1.0f);
    }
}
