using BeatGame.Data;
using BeatGame.Utility;
using BeatGame.Utility.ModSupport;
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
using Debug = UnityEngine.Debug;

namespace BeatGame.Logic.Managers
{
    public class CurrentSongDataManager : MonoBehaviour
    {
        public static CurrentSongDataManager Instance;

        public AvailableSongData SelectedSongData;
        public DifficultyBeatmap SelectedDifficultyMap;
        public string Difficulity;

        public MapData MapData;
        public JObject MapJsonObject;

        public SongSpawningInfo SongSpawningInfo;

        public bool HasLoadedData;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        public async void LoadLevelDataAsync()
        {
            HasLoadedData = false;

            SetSpawningData();
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


                NativeArray<RawEventData> rawEventDatas = new NativeArray<RawEventData>(MapJsonObject["_events"].ToObject<RawEventData[]>(), Allocator.TempJob);
                NativeArray<EventData> eventDatas = new NativeArray<EventData>(rawEventDatas.Length, Allocator.TempJob);
                Debug.Log("event job assigned : " + stopwatch.ElapsedMilliseconds);
                var convertEventJob = new ConvertEventDatas
                {
                    RawData = rawEventDatas,
                    LineOffset = SettingsManager.LineOffset,
                    ConvertedData = eventDatas,
                };
                var eventJobHandle = convertEventJob.Schedule();


                RawNoteData[] rawNoteDataArray = new RawNoteData[MapJsonObject["_notes"].Count()];

                await new WaitForBackgroundThread();
                rawNoteDataArray = await Task.Run(() => { return MapJsonObject["_notes"].ToObject<RawNoteData[]>(); });
                await new WaitForUpdate();

                NativeArray<RawNoteData> rawNoteDatas = new NativeArray<RawNoteData>(rawNoteDataArray, Allocator.TempJob);
                NativeArray<NoteData> noteDatas = new NativeArray<NoteData>(rawNoteDatas.Length, Allocator.TempJob);
                Debug.Log("note job assigned : " + stopwatch.ElapsedMilliseconds);
                var convertNoteJob = new ConvertNoteDatas
                {
                    RawData = rawNoteDatas,
                    UsesNoodleExtensions = usesNoodleExtensions,
                    LineOffset = SettingsManager.LineOffset,
                    ConvertedData = noteDatas,
                };
                var noteJobHandle = convertNoteJob.Schedule();


                RawObstacleData[] rawObstacleDataArray = new RawObstacleData[MapJsonObject["_obstacles"].Count()];
                await new WaitForBackgroundThread();
                rawObstacleDataArray = MapJsonObject["_obstacles"].ToObject<RawObstacleData[]>();
                await new WaitForUpdate();

                NativeArray<RawObstacleData> rawObstacleDatas = new NativeArray<RawObstacleData>(rawObstacleDataArray, Allocator.TempJob);
                NativeArray<ObstacleData> obstacleDatas = new NativeArray<ObstacleData>(rawObstacleDatas.Length, Allocator.TempJob);
                Debug.Log("obstacle job assigned : " + stopwatch.ElapsedMilliseconds);

                var convertObstacleJob = new ConvertObstacleDatas
                {
                    RawData = rawObstacleDatas,
                    UsesNoodleExtensions = usesNoodleExtensions,
                    LineOffset = SettingsManager.LineOffset,
                    NoteJumpSpeed = SongSpawningInfo.NoteJumpSpeed,
                    SecondEquivalentOfBeat = (float)SongSpawningInfo.SecondEquivalentOfBeat,
                    ConvertedData = obstacleDatas,
                };
                var obstacleJobHandle = convertObstacleJob.Schedule();

                eventJobHandle.Complete();
                noteJobHandle.Complete();
                obstacleJobHandle.Complete();
                stopwatch.Stop();
                Debug.Log("jobs completed : " + stopwatch.ElapsedMilliseconds);

                MapData.Events = eventDatas.ToArray();
                MapData.Notes = noteDatas.ToArray();
                MapData.Obstacles = obstacleDatas.ToArray();

                rawEventDatas.Dispose();
                rawNoteDatas.Dispose();
                rawObstacleDatas.Dispose();

                eventDatas.Dispose();
                noteDatas.Dispose();
                obstacleDatas.Dispose();

                Debug.Log("Using Noodle Extensions: " + usesNoodleExtensions);

                AssignDataToSystems();

                await new WaitForSeconds(.1f);
                HasLoadedData = true;
            }
        }

        void AssignDataToSystems()
        {

            // Load Notes
            NoteSpawningSystem noteSpawningSystem = (NoteSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(NoteSpawningSystem));
            NativeArray<NoteData> noteSpawnDatas = new NativeArray<NoteData>(Instance.MapData.Notes, Allocator.TempJob);
            noteSpawningSystem.notesToSpawn.Clear();
            noteSpawningSystem.notesToSpawn.AddRange(noteSpawnDatas);
            noteSpawnDatas.Dispose();

            // Load Obstacles
            ObstacleSpawningSystem obstacleSpawningSystem = (ObstacleSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(ObstacleSpawningSystem));
            NativeArray<ObstacleData> obstacleSpawnDatas = new NativeArray<ObstacleData>(Instance.MapData.Obstacles, Allocator.TempJob);
            obstacleSpawningSystem.obstacles.Clear();
            obstacleSpawningSystem.obstacles.AddRange(obstacleSpawnDatas);
            obstacleSpawnDatas.Dispose();

            // Load Events
            NativeArray<EventData> eventsToPlay = new NativeArray<EventData>(Instance.MapData.Events, Allocator.TempJob);
            EventPlayingSystem.Instance.Events.Clear();
            EventPlayingSystem.Instance.Events.AddRange(eventsToPlay);
            eventsToPlay.Dispose();

            Debug.Log("Notes: " + Instance.MapData.Notes.Length);
            Debug.Log("Obstacles: " + Instance.MapData.Obstacles.Length);
            Debug.Log("Events: " + Instance.MapData.Events.Length);
        }

        public void SetSpawningData()
        {

            foreach (var item in SelectedSongData.SongInfoFileData.DifficultyBeatmapSets[0].DifficultyBeatmaps)
            {
                if (item.Difficulty == Difficulity)
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
            public float3 LineOffset;

            public NativeArray<NoteData> ConvertedData;

            public void Execute()
            {
                for (int i = 0; i < ConvertedData.Length; i++)
                {
                    NoteData note = PlacementHelper.ConvertNoteDataWithVanillaMethod(RawData[i], LineOffset);

                    note.Color = ChromaSupport.GetColorForObstacle(RawData[i].CustomData);

                    if (UsesNoodleExtensions)
                        note = NoodleExtensions.ConvertNoteDataToNoodleExtensions(note, RawData[i], LineOffset);

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
            public float3 LineOffset;
            [ReadOnly]
            public float NoteJumpSpeed;
            [ReadOnly]
            public float SecondEquivalentOfBeat;
            [WriteOnly]
            public NativeArray<ObstacleData> ConvertedData;

            public void Execute()
            {
                for (int i = 0; i < ConvertedData.Length; i++)
                {
                    ObstacleData obstacle = PlacementHelper.ConvertObstacleDataWithVanillaMethod(RawData[i], NoteJumpSpeed, SecondEquivalentOfBeat, LineOffset);

                    obstacle.Color = ChromaSupport.GetColorForObstacle(RawData[i].CustomData);

                    if (UsesNoodleExtensions)
                        obstacle = NoodleExtensions.ConvertObstacleDataToNoodleExtensions(obstacle, RawData[i], NoteJumpSpeed, SecondEquivalentOfBeat, LineOffset);

                    ConvertedData[i] = obstacle;
                }
            }
        }

        [BurstCompile]
        struct ConvertEventDatas : IJob
        {
            [ReadOnly]
            public NativeArray<RawEventData> RawData;
            [ReadOnly]
            public float3 LineOffset;
            [WriteOnly]
            public NativeArray<EventData> ConvertedData;

            public void Execute()
            {
                for (int i = 0; i < ConvertedData.Length; i++)
                {
                    EventData eventData = new EventData
                    {
                        Time = RawData[i].Time,
                        Type = RawData[i].Type,
                        Value = RawData[i].Value,
                        PropID = RawData[i].CustomData.PropID,
                        Color = new float4
                        {
                            xyz = ChromaSupport.GetColorForEvent(RawData[i]),
                            w = 1
                        }
                    };

                    ConvertedData[i] = eventData;
                }
            }
        }
    }
}