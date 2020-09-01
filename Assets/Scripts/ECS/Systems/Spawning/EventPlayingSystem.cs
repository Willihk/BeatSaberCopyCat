using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Jobs;
using Unity.Burst;
using BeatGame.Data;
using BeatGame.Logic.Managers;
using BeatGame.Data.Map.Modified;

public class EventPlayingSystem : SystemBase
{
    public static EventPlayingSystem Instance;

    /// <summary>
    /// Value 1 is Event Type.
    /// Value 2 is Event Value.
    /// </summary>
    public event Action<int, EventData> OnPlayEvent;

    public NativeList<EventData> Events;
    NativeQueue<int> eventsToSpawnIndexQueue;

    protected override void OnCreate()
    {
        if (Instance == null)
            Instance = this;

        Events = new NativeList<EventData>(Allocator.Persistent);
        eventsToSpawnIndexQueue = new NativeQueue<int>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        Events.Dispose();
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
            EventDatas = Events,
            EventsToSpawnIndexQueue = eventsToSpawnIndexQueue.AsParallelWriter()
        };

        job.Schedule(Events.Length, 64, Dependency).Complete();

        while (eventsToSpawnIndexQueue.TryDequeue(out int index))
        {
            PlayEvent(Events[index]);
        }
    }

    private void PlayEvent(EventData eventInfo)
    {
        OnPlayEvent?.Invoke(eventInfo.Type, eventInfo);
    }

    [BurstCompile]
    struct GetEventsToSpawn : IJobParallelFor
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

        public void Execute(int index)
        {
            if (EventDatas[index].Time + HalfJumpDuration >= LastBeat && EventDatas[index].Time + HalfJumpDuration <= CurrentBeat)
            {
                EventsToSpawnIndexQueue.Enqueue(index);
            }
        }
    }
}
