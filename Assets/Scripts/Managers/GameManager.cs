using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using UnityEditor.Build.Pipeline;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlaying;

    public float CurrentSongTime;

    [SerializeField]
    AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "Map")
        {
            IsPlaying = true;
        }
    }

    private void Update()
    {
        if (IsPlaying)
        {
            CurrentSongTime += 1 * (CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat + 1) * Time.deltaTime;
        }
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

        CurrentSongDataManager.Instance.LoadLevelData();

        // Load Notes
        NoteSpawningSystem noteSpawningSystem = (NoteSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(NoteSpawningSystem));
        NativeArray<NoteSpawnData> noteSpawnDatas = new NativeArray<NoteSpawnData>(CurrentSongDataManager.Instance.MapData.Notes.ToArray(), Allocator.TempJob);
        noteSpawningSystem.notesToSpawn.AddRange(noteSpawnDatas);
        noteSpawnDatas.Dispose();

        // Load Obstacles
        ObstacleSpawningSystem obstacleSpawningSystem = (ObstacleSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(ObstacleSpawningSystem));
        NativeArray<ObstacleData> obstacleSpawnDatas = new NativeArray<ObstacleData>(CurrentSongDataManager.Instance.MapData.Obstacles.ToArray(), Allocator.TempJob);
        obstacleSpawningSystem.obstaclesToSpawn.AddRange(obstacleSpawnDatas);
        obstacleSpawnDatas.Dispose();

        Debug.Log("Notes: " + CurrentSongDataManager.Instance.MapData.Notes.Count);
        Debug.Log("Obstacles: " + CurrentSongDataManager.Instance.MapData.Obstacles.Count);

        StartCoroutine(Loading());
    }

    IEnumerator Loading()
    {
        bool isLoaded = false;

        while (!isLoaded)
        {
            if (audioSource.clip != null)
                isLoaded = true;

            if (!isLoaded)
                yield return null;
        }

        var mapLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.Map, LoadSceneMode.Additive);
        mapLoad.completed += (AsyncOperation operation) =>
        {
            SceneManager.UnloadSceneAsync((int)SceneIndexes.Loading);
            Invoke("PlaySong", CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration);
        };
    }

    void PlaySong()
    {
        audioSource.Play();
    }

    IEnumerator GetAudioClip()
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
