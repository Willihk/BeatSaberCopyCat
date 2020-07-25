using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action OnLoadingFinished;

    public bool IsPlaying;

    public double CurrentBeat;
    public double LastBeat;

    [SerializeField]
    public AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Map" && loadSceneMode == LoadSceneMode.Additive)
        {
            IsPlaying = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            IsPlaying = false;
            CurrentBeat = 0;
            audioSource.Stop();
            ReturnToMenu();
        }

        if (IsPlaying)
        {
            LastBeat = CurrentBeat;
            CurrentBeat += 1 / CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * Time.deltaTime;
            if (CurrentBeat >= (float)(CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration + 5) && !audioSource.isPlaying)
            {
                IsPlaying = false;
                CurrentBeat = 0;
                audioSource.Stop();
                Invoke("ReturnToMenu", 5);
            }
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.UnloadSceneAsync((int)SceneIndexes.Map);
        SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
    }

    public void PlayLevel()
    {
        SceneManager.UnloadSceneAsync((int)SceneIndexes.MainMenu);

        SceneManager.LoadScene((int)SceneIndexes.Loading, LoadSceneMode.Additive);

        Instance.StartLoading();
    }

    public void StartLoading()
    {
        StartCoroutine(GetAudioClip());

        CurrentSongDataManager.Instance.LoadLevelDataAsync();

        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        bool isLoaded = false;

        while (!isLoaded)
        {
            if (audioSource.clip != null && CurrentSongDataManager.Instance.HasLoadedData)
                isLoaded = true;

            if (!isLoaded)
                yield return null;
        }

        var mapLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.Map, LoadSceneMode.Additive);
        mapLoad.completed += (AsyncOperation operation) =>
        {
            SceneManager.UnloadSceneAsync((int)SceneIndexes.Loading);
            OnLoadingFinished?.Invoke();
            Invoke("PlaySong", (float)(CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration));
        };
    }

    void PlaySong()
    {
        audioSource.Play();
    }

    IEnumerator GetAudioClip()
    {
        if (CurrentSongDataManager.Instance.SelectedSongData.AudioClip != null)
        {
            audioSource.clip = CurrentSongDataManager.Instance.SelectedSongData.AudioClip;
        }
        else
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                $"file://{CurrentSongDataManager.Instance.SelectedSongData.DirectoryPath}/{CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongFilename}",
                AudioType.OGGVORBIS))
            {
                audioSource.clip = null;

                yield return www.SendWebRequest();

                if (www.isNetworkError)
                {
                    Debug.Log(www.error);
                }
                else
                {
                    AudioClip songClip = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = songClip;
                }
            }
        }
    }
}
