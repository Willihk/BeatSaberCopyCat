using UnityEngine;
using System.Collections;
using Unity.Entities;
using Unity.Collections;
using System;

public class EventPlayingSystem : SystemBase
{
    /// <summary>
    /// Value 1 is Event Type.
    /// Value 2 is Event Value.
    /// </summary>
    public event Action<int, int> OnPlayEvent;

    public NativeList<EventData> eventsToPlay;

    // Used to always run the system
    EntityQuery defaultQuery;

    protected override void OnCreate()
    {
        eventsToPlay = new NativeList<EventData>(Allocator.Persistent);
        defaultQuery = GetEntityQuery(new EntityQueryDesc { All = new ComponentType[] { typeof(Entity) } });
    }

    protected override void OnDestroy()
    {
        eventsToPlay.Dispose();
    }

    protected override void OnUpdate()
    {
        if (GameManager.Instance.IsPlaying)
        {
            SpawnNeededEvents();
        }
    }

    void SpawnNeededEvents()
    {
        if (eventsToPlay.IsCreated == false)
            return;

        for (int i = 0; i < eventsToPlay.Length; i++)
        {
            var eventInfo = eventsToPlay[i];

            if (eventInfo.Time + CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration >= GameManager.Instance.LastBeat &&eventInfo.Time + CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration <= GameManager.Instance.CurrentBeat)
            {
                PlayEvent(eventInfo);
            }
        }
    }

    private void PlayEvent(EventData eventInfo)
    {
        OnPlayEvent?.Invoke(eventInfo.Type, eventInfo.Value);
    }
}
