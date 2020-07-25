using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;

public class SongBrowser : MonoBehaviour
{
    public List<AvailableSongData> AvailableSongs = new List<AvailableSongData>();

    [SerializeField]
    GameObject songEntryPrefab;
    [SerializeField]
    Transform songEntryHolder;
    [SerializeField]
    TabGroup tabGroup;
    [SerializeField]
    SongInfoController infoController;

    readonly string customSongFolderPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Beat Saber Songs\";

    private void Start()
    {
        EnsureSongFolderExists();
        LoadSongs();
        DisplaySongs();
    }

    private void OnDestroy()
    {
        AvailableSongs.Remove(CurrentSongDataManager.Instance.SelectedSongData);

        foreach (var item in AvailableSongs)
        {
            if (item.AudioClip != null)
                item.AudioClip.UnloadAudioData();
        }
    }

    public void SongSelected(TabButton tabButton)
    {
        infoController.DisplaySong(AvailableSongs[tabGroup.TabButtons.IndexOf(tabButton)], tabButton.gameObject);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            infoController.DisplaySong(AvailableSongs[0], tabGroup.TabButtons[0].gameObject);
        }
    }

    void EnsureSongFolderExists()
    {
        if (!Directory.Exists(customSongFolderPath))
        {
            Directory.CreateDirectory(customSongFolderPath);
        }
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
        if (!Directory.Exists(customSongFolderPath))
            return;

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
