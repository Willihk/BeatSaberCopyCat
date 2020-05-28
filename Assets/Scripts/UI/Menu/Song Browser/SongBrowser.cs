using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;

public class SongBrowser : MonoBehaviour
{
    public List<AvailableSongData> AvailableSongs = new List<AvailableSongData>();

    [SerializeField]
    GameObject songEntryPrefab;
    [SerializeField]
    Transform songEntryHolder;
    [SerializeField]
    TabGroup tabGroup;

    string customSongFolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Temp\BeatSaber Songs\";

    private void Start()
    {
        LoadSongs();
        DisplaySongs();
    }

    public void SongSelected(TabButton tabButton)
    {

    }

    void DisplaySongs()
    {
        foreach (Transform child in songEntryHolder)
        {
            child.gameObject.SetActive(false);
        }

        foreach (var item in AvailableSongs)
        {
            var songEntryObject = Instantiate(songEntryPrefab, songEntryHolder);
            songEntryObject.GetComponent<SongEntryController>().Initizalize(item);
            songEntryObject.GetComponent<TabButton>().SetTabGroup(tabGroup);
        }
        
    }

    void LoadSongs()
    {
        List<string> directoryPaths = Directory.GetDirectories(customSongFolderPath, "*", SearchOption.TopDirectoryOnly).ToList();

        foreach (var item in directoryPaths)
        {
            if (IsSongUsable(item, out AvailableSongData availableSong))
            {
                AvailableSongs.Add(availableSong);
            }
        }
    }

    bool IsSongUsable(string path, out AvailableSongData availableSong)
    {
        availableSong = new AvailableSongData();
        string infoFilePath = path + "\\info.dat";
        if (File.Exists(infoFilePath))
        {
            SongInfoFileData infoFileData = JsonConvert.DeserializeObject<SongInfoFileData>(File.ReadAllText(infoFilePath));

            availableSong.DirectoryPath = path;
            availableSong.SongInfoFileData = infoFileData;

            return true;
        }

        return false;
    }


}
