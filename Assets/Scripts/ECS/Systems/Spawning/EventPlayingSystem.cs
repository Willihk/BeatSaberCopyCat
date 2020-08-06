using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Jobs;
using Unity.Burst;
using BeatGame.Data;
using BeatGame.Logic.Managers;

public class EventPlayingSystem : SystemBase
{
    /// <summary>
    /// Value 1 is Event Type.
    /// Value 2 is Event Value.
    /// </summary>
    public event Action<int, int> OnPlayEvent;

    public NativeList<EventData> eventsToPlay;
    NativeQueue<int> eventsToSpawnIndexQueue;

    // Used to always run the system
    EntityQuery defaultQuery;

    protected override void OnCreate()
    {
        eventsToPlay = new NativeList<EventData>(Allocator.Persistent);
        eventsToSpawnIndexQueue = new NativeQueue<int>(Allocator.Persistent);
        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });
    }

    protected override void OnDestroy()
    {
        eventsToPlay.Dispose();
        eventsToSpawnIndexQueue.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsPlaying)
        {
            SpawnNeededEvents();
        }
    }

    void SpawnNeededEvents()
    {
        var job = new GetEventsToSpawn
        {
            CurrentBeat = GameManager.Instance.CurrentBeat,
            LastBeat = GameManager.Instance.LastBeat,
            HalfJumpDuration = CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration,
            EventDatas = eventsToPlay,
            EventsToSpawnIndexQueue = eventsToSpawnIndexQueue.AsParallelWriter()
        };

        job.Schedule().Complete();

        while (eventsToSpawnIndexQueue.TryDequeue(out int index))
        {
            PlayEvent(eventsToPlay[index]);
        }
    }

    private void PlayEvent(EventData eventInfo)
    {
        OnPlayEvent?.Invoke(eventInfo.Type, eventInfo.Value);
    }

    [BurstCompile]
    struct GetEventsToSpawn : IJob
    {
        [ReadOnly]
        public NativeArray<EventData> EventDatas;
        [ReadOnly]
        public double CurrentBeat;
        [ReadOnly]
        public double LastBeat;
        [ReadOnly]
        public double HalfJumpDuration;

        public NativeQueue<int>.ParallelWriter EventsToSpawnIndexQueue;

        public void Execute()
        {
            for (int i = 0; i < EventDatas.Length; i++)
            {
                if (EventDatas[i].Time + HalfJumpDuration >= LastBeat && EventDatas[i].Time + HalfJumpDuration <= CurrentBeat)
                {
                    EventsToSpawnIndexQueue.Enqueue(i);
                }
            }
        }
    }
}
