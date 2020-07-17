using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class CurrentSongDataManager : MonoBehaviour
{
    public static CurrentSongDataManager Instance;

    public AvailableSongData SelectedSongData;
    public DifficultyBeatmap SelectedDifficultyMap;

    public MapData MapData;
    public JObject MapJsonObject;

    public float3 SpawnPointOffset = new float3(.8f, .8f, 0);

    public SongSpawningInfo SongSpawningInfo;

    public bool HasLoadedData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public async void LoadLevelDataAsync()
    {
        if (File.Exists(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename))
        {
            MapData = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));

            MapJsonObject = JObject.Parse(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));
            Stopwatch stopwatch = new Stopwatch();

            if (SelectedDifficultyMap.CustomData.Requirements == null)
            {
                var data = SelectedDifficultyMap.CustomData;
                data.Requirements = new string[1] { "" };
                SelectedDifficultyMap.CustomData = data;
            }

            bool usesNoodleExtensions = false;
            if (SelectedDifficultyMap.CustomData.Requirements != null)
                usesNoodleExtensions = SelectedDifficultyMap.CustomData.Requirements.Any(x => x == "Noodle Extensions");

            stopwatch.Start();

            NativeArray<RawNoteData> rawNoteDatas = new NativeArray<RawNoteData>(await Task.Run(() => { return MapJsonObject["_notes"].ToObject<RawNoteData[]>(); }), Allocator.TempJob);
            NativeArray<NoteData> noteDatas = new NativeArray<NoteData>(rawNoteDatas.Length, Allocator.TempJob);
            Debug.Log("note job assigned : " + stopwatch.ElapsedMilliseconds);
            var convertNoteJob = new ConvertNoteDatas
            {
                RawData = rawNoteDatas,
                UsesNoodleExtensions = usesNoodleExtensions,
                SpawnPointOffset = SpawnPointOffset,
                ConvertedData = noteDatas,
            };
            var noteJobHandle = convertNoteJob.Schedule();

            NativeArray<RawObstacleData> rawObstacleDatas = new NativeArray<RawObstacleData>(await Task.Run(() => { return MapJsonObject["_obstacles"].ToObject<RawObstacleData[]>(); }), Allocator.TempJob);
            NativeArray<ObstacleData> obstacleDatas = new NativeArray<ObstacleData>(rawObstacleDatas.Length, Allocator.TempJob);
            Debug.Log("obstacle job assigned : " + stopwatch.ElapsedMilliseconds);

            var convertObstacleJob = new ConvertObstacleDatas
            {
                RawData = rawObstacleDatas,
                UsesNoodleExtensions = usesNoodleExtensions,
                SpawnPointOffset = SpawnPointOffset,
                ConvertedData = obstacleDatas,
            };
            var obstacleJobHandle = convertObstacleJob.Schedule();

            noteJobHandle.Complete();
            obstacleJobHandle.Complete();
            stopwatch.Stop();
            Debug.Log("jobs completed : " + stopwatch.ElapsedMilliseconds);

            MapData.Notes = noteDatas.ToArray();
            MapData.Obstacles = obstacleDatas.ToArray();

            rawNoteDatas.Dispose();
            rawObstacleDatas.Dispose();

            noteDatas.Dispose();
            obstacleDatas.Dispose();

            Debug.Log("Using Noodle Extensions: " + usesNoodleExtensions);


            // Load Notes
            NoteSpawningSystem noteSpawningSystem = (NoteSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(NoteSpawningSystem));
            NativeArray<NoteData> noteSpawnDatas = new NativeArray<NoteData>(Instance.MapData.Notes, Allocator.TempJob);
            noteSpawningSystem.notesToSpawn.AddRange(noteSpawnDatas);
            noteSpawnDatas.Dispose();

            // Load Obstacles
            ObstacleSpawningSystem obstacleSpawningSystem = (ObstacleSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(ObstacleSpawningSystem));
            NativeArray<ObstacleData> obstacleSpawnDatas = new NativeArray<ObstacleData>(Instance.MapData.Obstacles, Allocator.TempJob);
            obstacleSpawningSystem.obstaclesToSpawn.AddRange(obstacleSpawnDatas);
            obstacleSpawnDatas.Dispose();

            // Load Events
            EventPlayingSystem eventPlayingSystem = (EventPlayingSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(EventPlayingSystem));
            NativeArray<EventData> eventsToPlay = new NativeArray<EventData>(Instance.MapData.Events, Allocator.TempJob);
            eventPlayingSystem.eventsToPlay.AddRange(eventsToPlay);
            eventsToPlay.Dispose();

            Debug.Log("Notes: " + Instance.MapData.Notes.Length);
            Debug.Log("Obstacles: " + Instance.MapData.Obstacles.Length);
            Debug.Log("Events: " + Instance.MapData.Events.Length);

            HasLoadedData = true;
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

    [BurstCompile]
    struct ConvertNoteDatas : IJob
    {
        [ReadOnly]
        public bool UsesNoodleExtensions;
        [ReadOnly]
        public NativeArray<RawNoteData> RawData;
        [ReadOnly]
        public float3 SpawnPointOffset;

        public NativeArray<NoteData> ConvertedData;

        public void Execute()
        {
            for (int i = 0; i < ConvertedData.Length; i++)
            {
                NoteData note;
                if (UsesNoodleExtensions)
                    note = PlacementHelper.ConvertNoteDataWithNoodleExtensionsMethod(RawData[i], SpawnPointOffset);
                else
                    note = PlacementHelper.ConvertNoteDataWithVanillaMethod(RawData[i], SpawnPointOffset);

                ConvertedData[i] = note;
            }
        }
    }

    [BurstCompile]
    struct ConvertObstacleDatas : IJob
    {
        [ReadOnly]
        public bool UsesNoodleExtensions;
        [ReadOnly]
        public NativeArray<RawObstacleData> RawData;
        [ReadOnly]
        public float3 SpawnPointOffset;

        public NativeArray<ObstacleData> ConvertedData;

        public void Execute()
        {
            for (int i = 0; i < ConvertedData.Length; i++)
            {
                ObstacleData obstacle;
                if (UsesNoodleExtensions)
                    obstacle = PlacementHelper.ConvertObstacleDataWithNoodleExtensionsMethod(RawData[i], SpawnPointOffset);
                else
                    obstacle = PlacementHelper.ConvertObstacleDataWithVanillaMethod(RawData[i], SpawnPointOffset);

                ConvertedData[i] = obstacle;
            }
        }
    }
}
