using UnityEngine;
using System.Collections;
using System;

namespace BeatGame.Data
{
    [Serializable]
    public class AvailableSongData
    {
        public SongInfoFileData SongInfoFileData;
        public string DirectoryPath;
        public AudioClip AudioClip;
    }
}