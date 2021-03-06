using System.Collections;
using UnityEngine;

public class DeeJay : MonoBehaviour
{
    [SerializeField] private AudioSource boombastic_music;
    [SerializeField] private AudioSource boombastic_ambient;
    [SerializeField] private AudioSource boombastic_wind;
    [SerializeField] private AudioSource autoradio; 

    [SerializeField] private AudioClip musica;
    [SerializeField] private AudioClip ambient;
    [SerializeField] private AudioClip wind;
    [SerializeField] private AudioClip engine;

    [SerializeField] private float fadeInTime;

    // Start is called before the first frame update
    void Start()
    {
        AmbientSFXStart();
       
        
        EngineInit();
    }

    // Update is called once per frame
    void Update()
    {
        EngineRoar();
    }

    public void AmbientSFXStart()
    {
        boombastic_ambient.clip = ambient;
        boombastic_ambient.volume = 0;
        boombastic_ambient.loop = true;
        boombastic_ambient.Play();
        StartCoroutine(FadeToMax(boombastic_ambient, fadeInTime));
    }

    public void AmbientWindStart()
    {
        boombastic_wind.clip = wind;
        boombastic_wind.volume = 0;
        boombastic_wind.loop = true;
        boombastic_wind.Play();
        StartCoroutine(FadeToMax(boombastic_wind, fadeInTime));
    }

    public void AmbientMusicStart()
    {
        boombastic_music.clip = musica;
        boombastic_music.volume = 0;
        boombastic_music.loop = true;
        boombastic_music.Play();
        StartCoroutine(FadeToMax(boombastic_music, fadeInTime));
    }

    private void EngineInit()
    {
        autoradio.clip = engine;
        autoradio.volume = 0.3f;
        autoradio.loop = true;
        autoradio.Play();
    }
    private void EngineRoar()
    {
        if (Input.GetAxis("Vertical") == 0)
        {
            StartCoroutine(BackFadeTo(autoradio, 0.6f, 0.01f));
        }
        if (Input.GetAxis("Vertical") != 0)
        {
            StartCoroutine(FadeToMax(autoradio, 0.3f));
        }

    }

    IEnumerator FadeToMax(AudioSource sorgente, float secondi)
    {
        while (sorgente.volume < 1)
        {
            sorgente.volume += 0.01f;
            sorgente.pitch = Mathf.Clamp(sorgente.pitch + 0.01f, 1f, 1.5f);
            yield return new WaitForSeconds(secondi);
        }
    }

    IEnumerator BackFadeTo(AudioSource sorgente, float volume, float secondi)
    {
        while (sorgente.volume > volume)
        {
            sorgente.volume -= 0.01f;
            sorgente.pitch = Mathf.Clamp(sorgente.pitch - 0.01f, 1f, 1.5f);
            yield return new WaitForSeconds(secondi);
        }
    }
}
