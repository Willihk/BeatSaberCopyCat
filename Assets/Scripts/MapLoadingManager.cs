using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

public class MapLoadingManager : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        MapData data = JsonConvert.DeserializeObject<MapData>(File.ReadAllText(@"C:\Users\will1400\Documents\Temp\BeatSaber Songs\46859b08c6398b6d16e4301b9d6ce4b25ee6ba71\ExpertPlusStandard.dat"));

        Debug.Log(data.Notes.Count);

    }

   
}
