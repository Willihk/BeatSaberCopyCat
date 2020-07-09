using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CurrentSongDataManager : MonoBehaviour
{
    public static CurrentSongDataManager Instance;

    public AvailableSongData SelectedSongData;
    public DifficultyBeatmap SelectedDifficultyMap;

    public MapData MapData;
    public JObject MapJsonObject;

    public float3 SpawnPointOffset = new float3(.8f, .8f, 0);

    public SongSpawningInfo SongSpawningInfo;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void LoadLevelData()
    {
        if (File.Exists(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename))
        {
            MapData = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));

            MapJsonObject = JObject.Parse(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));
            ConvertNotes();
            ConvertObstacles();
        }
    }

    void ConvertNotes()
    {
        bool usesNoodleExtensions = false;
        if (SelectedDifficultyMap.CustomData.Requirements != null)
            usesNoodleExtensions = SelectedDifficultyMap.CustomData.Requirements.Any(x => x == "Noodle Extensions");

        Debug.Log(usesNoodleExtensions);

        var rawData = ((JArray)MapJsonObject["_notes"]).ToObject<RawNoteData[]>();
        var notedata = new NoteData[rawData.Length];

        for (int i = 0; i < rawData.Length; i++)
        {
            NoteData note;
            if (usesNoodleExtensions)
                note = PlacementHelper.ConvertNoteDataWithNoodleExtensionsMethod(rawData[i], SpawnPointOffset);
            else
                note = PlacementHelper.ConvertNoteDataWithVanillaMethod(rawData[i], SpawnPointOffset);

            notedata[i] = note;
        }

        MapData.Notes = notedata;
    }

    void ConvertObstacles()
    {
        bool usesNoodleExtensions = false;
        if (SelectedDifficultyMap.CustomData.Requirements != null)
            usesNoodleExtensions = SelectedDifficultyMap.CustomData.Requirements.Any(x => x == "Noodle Extensions");

        var rawData = ((JArray)MapJsonObject["_obstacles"]).ToObject<RawObstacleData[]>();
        var obstacleDatas = new ObstacleData[rawData.Length];

        for (int i = 0; i < rawData.Length; i++)
        {
            ObstacleData obstacle;
            if (usesNoodleExtensions)
                obstacle = PlacementHelper.ConvertObstacleDataWithNoodleExtensionsMethod(rawData[i], SpawnPointOffset);
            else
                obstacle = PlacementHelper.ConvertObstacleDataWithVanillaMethod(rawData[i], SpawnPointOffset);

            obstacleDatas[i] = obstacle;
        }

        MapData.Obstacles = obstacleDatas;
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

        SongSpawningInfo = new SongSpawningInfo
        {
            BPM = SelectedSongData.SongInfoFileData.BeatsPerMinute,
            NoteJumpSpeed = SelectedDifficultyMap.NoteJumpMovementSpeed,
            NoteJumpStartBeatOffset = (float)SelectedDifficultyMap.NoteJumpStartBeatOffset,
            SecondEquivalentOfBeat = (double)60 / SelectedSongData.SongInfoFileData.BeatsPerMinute,
        };
        // Taken from SpawnDistanceCalc by kyle1413

        float num4 = 1f;
        float num5 = 18f;
        float num6 = 4f;
        float num8 = num6;
        while (SongSpawningInfo.NoteJumpSpeed * SongSpawningInfo.SecondEquivalentOfBeat * num8 > num5)
        {
            num8 /= 2f;
        }

        float num9 = num8 + SongSpawningInfo.NoteJumpStartBeatOffset;

        if ((double)num9 < num4)
        {
            num9 = num4;
        }

        SongSpawningInfo.HalfJumpDuration = num9;
        SongSpawningInfo.DistanceToMove = (float)SongSpawningInfo.SecondEquivalentOfBeat * 2.0f * 150.0f;
        SongSpawningInfo.JumpDistance = SongSpawningInfo.NoteJumpSpeed * (float)SongSpawningInfo.SecondEquivalentOfBeat * num9 * 2;
    }
}
