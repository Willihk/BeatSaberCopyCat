using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

public class MapLoadingManager : MonoBehaviour
{
    public static MapLoadingManager Instance;

    public MapData MapData;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        MapData data = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(@"C:\Users\will1400\Documents\Temp\BeatSaber Songs\Current\CurrentData.dat"));

        Debug.Log(data.Notes.Count);
        MapData = data;
    }
}
