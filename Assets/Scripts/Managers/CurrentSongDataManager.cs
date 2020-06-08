using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CurrentSongDataManager : MonoBehaviour
{
    public static CurrentSongDataManager Instance;

    public AvailableSongData SelectedSongData;
    public DifficultyBeatmap SelectedDifficultyMap;

    public MapData MapData;

    [SerializeField]
    AudioSource audioSource;

    AudioClip songAudioClip;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        DontDestroyOnLoad(this);
    }

    public void PlayLevel()
    {
        SceneManager.UnloadSceneAsync((int)SceneIndexes.MainMenu);

        SceneManager.LoadScene((int)SceneIndexes.Loading, LoadSceneMode.Additive);

        LoadLevelData();
        var mapLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.Map, LoadSceneMode.Additive);


        mapLoad.completed += (AsyncOperation operation) =>
        {
            SceneManager.UnloadSceneAsync((int)SceneIndexes.Loading);
            NoteSpawningSystem spawningSystem = (NoteSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(NoteSpawningSystem));

            NativeArray<NoteSpawnData> noteSpawnDatas = new NativeArray<NoteSpawnData>(MapData.Notes.ToArray(), Allocator.TempJob);
            spawningSystem.notesToSpawn.AddRange(noteSpawnDatas);

            noteSpawnDatas.Dispose();

            GameManager.Instance.IsPlaying = true;
            audioSource.Play();
        };

        StartCoroutine(GetAudioClip());
    }


    IEnumerator GetAudioClip()
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip($"file://{SelectedSongData.DirectoryPath}/{SelectedSongData.SongInfoFileData.SongFilename}", AudioType.OGGVORBIS))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip songClip = DownloadHandlerAudioClip.GetContent(www);
                songAudioClip = songClip;
                audioSource.clip = songClip;
            }
        }
    }

    public void LoadLevelData()
    {
        if (File.Exists(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename))
        {
            MapData = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));
        }
    }

    public void SetData(AvailableSongData songData, string difficulty)
    {
        SelectedSongData = songData;

        foreach (var item in SelectedSongData.SongInfoFileData.DifficultyBeatmapSets[0].DifficultyBeatmaps)
        {
            if (item.Difficulty == difficulty)
            {
                SelectedDifficultyMap = item;
                break;
            }
        }
    }
}
