using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct AvailableSongData
{
    public SongInfoFileData SongInfoFileData { get; set; }
    public string DirectoryPath;
}
