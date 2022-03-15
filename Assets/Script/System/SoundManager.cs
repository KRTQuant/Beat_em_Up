using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance = null;
    public List<AudioClip> audioClips;
    /* 
     [0] --> MainMenu
     [1] --> NormalMode
     [2] --> PersonaMode
     [3] --> PlayerDead
     */
    public AudioSource audioSource;
    public PlayerController player;
    public bool isMusicPlay = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        FindRef();
        ActiveMusic();
    }

    public void StopMusic()
    {
        audioSource.Stop();
        isMusicPlay = false;
    }

    void OnSceneLoad(Scene aScene, LoadSceneMode aMode)
    {
        isMusicPlay = false;
        if(player == null && SceneManager.GetActiveScene().buildIndex != 0)
        {
            StopMusic();
            player = GameObject.Find("Player").GetComponent<PlayerController>();
            ActiveMusic();
        }
        if(player == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            return;
        }
        if(audioSource == null && SceneManager.GetActiveScene().buildIndex == 0)
        {
            StopMusic();
            audioSource = GameObject.Find("Camera").GetComponent<AudioSource>();
            ActiveMusic();
        }
        if(audioSource == null && SceneManager.GetActiveScene().buildIndex != 0)
        {
            StopMusic();
            audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
            ActiveMusic();
        }
    }

    void ActiveMusic()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0 && !isMusicPlay)
        {
            audioSource.clip = audioClips[0];
            audioSource.Play();
            isMusicPlay = true;
        }
        if(player != null)
        {
            if (player.usingPersona)
            {
                if (!isMusicPlay)
                {
                    StopMusic();
                    audioSource.clip = audioClips[2];
                    audioSource.Play();
                    isMusicPlay = true;
                }
                
                if (isMusicPlay)
                {
                    StopMusic();
                    audioSource.clip = audioClips[2];
                    audioSource.Play();
                    isMusicPlay = true;
                }

            }
            if (!player.usingPersona)
            {
                if (!isMusicPlay)
                {
                    audioSource.clip = audioClips[1];
                    audioSource.Play();
                    isMusicPlay = true;
                }

            }

            if (player.currentHp <= 0)
            {
                audioSource.clip = audioClips[3];
                audioSource.Play();
                isMusicPlay = true;
            }
        }
        
    }

    void FindRef()
    {
        if(player == null)
        {
            if (player == null && SceneManager.GetActiveScene().buildIndex != 0)
            {
                player = GameObject.Find("Player").GetComponent<PlayerController>();
                ActiveMusic();
            }
            if (player == null && SceneManager.GetActiveScene().buildIndex == 0)
            {
                return;
            }
            Debug.Log("player null");
        }

        if(audioSource == null)
        {
            Debug.Log("audio null");
            if (audioSource == null && SceneManager.GetActiveScene().buildIndex == 0)
            {
                audioSource = GameObject.Find("Camera").GetComponent<AudioSource>();
                ActiveMusic();
            }
            if (audioSource == null && SceneManager.GetActiveScene().buildIndex != 0)
            {
                audioSource = GameObject.Find("Player").GetComponent<AudioSource>();
                ActiveMusic();
            }
        }
        if (audioSource != null)
        {
            Debug.Log("audio !null");
           // return;
        }
        if (player != null)
        {
            //Debug.Log("player !null");
            return;
        }

    }

}
