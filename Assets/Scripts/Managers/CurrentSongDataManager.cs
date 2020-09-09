using BeatGame.Data;
using BeatGame.Data.Map;
using BeatGame.Data.Map.Modified;
using BeatGame.Data.Map.Raw;
using BeatGame.Utility;
using BeatGame.Utility.ModSupport;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
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

        public void LoadLevelData()
        {
            HasLoadedData = false;

            SetSpawningData();

            if (LoadDataFromJson())
            {
                AssignDataToSystems();

                HasLoadedData = true;
            }
        }

        bool LoadDataFromJson()
        {
            if (!File.Exists(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename))
                return false;

            MapData = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));

            MapJsonObject = JObject.Parse(File.ReadAllText(SelectedSongData.DirectoryPath + "\\" + SelectedDifficultyMap.BeatmapFilename));

            if (SelectedDifficultyMap.CustomData.Requirements == null)
            {
                var data = SelectedDifficultyMap.CustomData;
                data.Requirements = new string[1] { "" };
                SelectedDifficultyMap.CustomData = data;
            }

            bool usesNoodleExtensions = false;
            if (SelectedDifficultyMap.CustomData.Requirements != null)
                usesNoodleExtensions = SelectedDifficultyMap.CustomData.Requirements.Any(x => x == "Noodle Extensions");


            NativeArray<RawEventData> rawEventDatas = new NativeArray<RawEventData>(MapJsonObject["_events"].ToObject<RawEventData[]>(), Allocator.TempJob);
            NativeArray<EventData> eventDatas = new NativeArray<EventData>(rawEventDatas.Length, Allocator.TempJob);
            var convertEventJob = new ConvertEventDatas
            {
                RawData = rawEventDatas,
                LineOffset = SettingsManager.LineOffset,
                ConvertedData = eventDatas,
            };
            var eventJobHandle = convertEventJob.Schedule();


            NativeArray<RawNoteData> rawNoteDatas = new NativeArray<RawNoteData>(MapJsonObject["_notes"].ToObject<RawNoteData[]>(), Allocator.TempJob);
            NativeArray<NoteData> noteDatas = new NativeArray<NoteData>(rawNoteDatas.Length, Allocator.TempJob);
            var convertNoteJob = new ConvertNoteDatas
            {
                RawData = rawNoteDatas,
                NoArrows = SettingsManager.Instance.Settings["Modifiers"]["NoArrows"].IntValue == 1,
                NoteJumpSpeed = SongSpawningInfo.NoteJumpSpeed,
                UsesNoodleExtensions = usesNoodleExtensions,
                LineOffset = SettingsManager.LineOffset,
                ConvertedData = noteDatas,
            };
            var noteJobHandle = convertNoteJob.Schedule();


            NativeArray<RawObstacleData> rawObstacleDatas = new NativeArray<RawObstacleData>(MapJsonObject["_obstacles"].ToObject<RawObstacleData[]>(), Allocator.TempJob);
            NativeArray<ObstacleData> obstacleDatas = new NativeArray<ObstacleData>(rawObstacleDatas.Length, Allocator.TempJob);

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

            MapData.Events = eventDatas.ToArray();
            MapData.Notes = noteDatas.ToArray();
            MapData.Obstacles = obstacleDatas.ToArray();

            rawEventDatas.Dispose();
            rawNoteDatas.Dispose();
            rawObstacleDatas.Dispose();

            eventDatas.Dispose();
            noteDatas.Dispose();
            obstacleDatas.Dispose();

            return true;
        }

        void AssignDataToSystems()
        {
            // Assign Notes
            NoteSpawningSystem noteSpawningSystem = (NoteSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(NoteSpawningSystem));
            NativeArray<NoteData> noteSpawnDatas = new NativeArray<NoteData>(Instance.MapData.Notes, Allocator.TempJob);
            noteSpawningSystem.notes.Clear();
            noteSpawningSystem.notes.AddRange(noteSpawnDatas);
            noteSpawnDatas.Dispose();

            // Assign Obstacles
            ObstacleSpawningSystem obstacleSpawningSystem = (ObstacleSpawningSystem)World.DefaultGameObjectInjectionWorld.GetOrCreateSystem(typeof(ObstacleSpawningSystem));
            NativeArray<ObstacleData> obstacleSpawnDatas = new NativeArray<ObstacleData>(Instance.MapData.Obstacles, Allocator.TempJob);
            obstacleSpawningSystem.obstacles.Clear();
            obstacleSpawningSystem.obstacles.AddRange(obstacleSpawnDatas);
            obstacleSpawnDatas.Dispose();

            // Assign Events
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

            float HalfJumpDuration = num8 + SongSpawningInfo.NoteJumpStartBeatOffset;

            if ((double)HalfJumpDuration < num4)
            {
                HalfJumpDuration = num4;
            }

            SongSpawningInfo.HalfJumpDuration = HalfJumpDuration;
            SongSpawningInfo.DistanceToMove = (float)SongSpawningInfo.SecondEquivalentOfBeat * 2.0f * 150.0f;
            SongSpawningInfo.JumpDistance = SongSpawningInfo.NoteJumpSpeed * (((float)SongSpawningInfo.SecondEquivalentOfBeat) * (HalfJumpDuration * 2));
            SongSpawningInfo.JumpDistance += 1.4f;
        }

        [BurstCompile]
        struct ConvertNoteDatas : IJob
        {
            [ReadOnly]
            public bool UsesNoodleExtensions;
            [ReadOnly]
            public bool NoArrows;
            [ReadOnly]
            public float NoteJumpSpeed;
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

                    if (NoArrows)
                    {
                        note.TransformData.LocalRotation = new quaternion(0, 0, 0.0008726948f, 0.9999996f);
                        note.CutDirection = 8;
                    }

                    note.TransformData.Speed = NoteJumpSpeed;

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
                    obstacle.TransformData.Speed = NoteJumpSpeed;

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