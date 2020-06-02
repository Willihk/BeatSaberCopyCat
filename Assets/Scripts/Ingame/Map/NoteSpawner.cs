using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [SerializeField]
    Transform noteHolder;
    [SerializeField]
    Vector3 spawnPointOffset = new Vector3(1, 1);
    [SerializeField]
    GameObject notePrefab;

    [SerializeField]
    Transform spawnPointHolder;

    Vector3[] spawnPoints = new Vector3[12];

    bool hasSpawned;
    MapData mapData;

    float currentTime;

    private void Awake()
    {
        if (noteHolder == null)
            noteHolder = new GameObject("NoteHolder").transform;

        noteHolder.position = spawnPointHolder.position;


        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                var pointObject = new GameObject("Spawnpoint");
                pointObject.transform.SetParent(spawnPointHolder);
                pointObject.transform.localPosition = new Vector3(x * spawnPointOffset.x, y * spawnPointOffset.y);
                spawnPoints[y * 4 + x] = new Vector3(x * spawnPointOffset.x, y * spawnPointOffset.y);
            }
        }
    }

    //private void Update()
    //{
    //    if (!hasSpawned)
    //        SpawnAllNotes();
    //}


    void SpawnNeededNotes()
    {
        for (int i = 0; i < mapData.Notes.Count; i++)
        {
            var note = mapData.Notes[i];

            if (note.Time - 10 <= currentTime)
            {
                SpawnNote(note);
                mapData.Notes.RemoveAt(i);
                i--;
            }
        }
    }

    void SpawnAllNotes()
    {
        mapData = MapLoadingManager.Instance.MapData;

        foreach (var note in mapData.Notes)
        {
            SpawnNote(note);
        }
        hasSpawned = true;
    }

    void SpawnNote(NoteData note)
    {
        var noteObject = Instantiate(notePrefab);
        noteObject.transform.SetParent(noteHolder);

        noteObject.transform.position = GetSpawnPosition(note.LineIndex, note.LineLayer);
        noteObject.transform.position += new Vector3(0, 0, GetNeededOffset(note.Time, 18));

        noteObject.GetComponent<NoteController>().Setup(note);
    }

    Vector3 GetSpawnPosition(int lineIndex, int lineLayer)
    {
        return spawnPoints[lineLayer * 4 + lineIndex];
    }

    float GetNeededOffset(double timeToSpawn, float movementSpeed)
    {
        return (float)(timeToSpawn * movementSpeed);
    }
}
