using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongInfoController : MonoBehaviour
{
    [SerializeField]
    private Image songBackgroundImage;
    [SerializeField]
    private TextMeshProUGUI songNameText;
    [SerializeField]
    TabGroup difficultyTabGroup;
    [SerializeField]
    GameObject difficultyTabPrefab;

    AvailableSongData songData;

    public void DisplaySong(AvailableSongData songData, GameObject selectedSongObject)
    {
        this.songData = songData;

        songBackgroundImage.sprite = selectedSongObject.GetComponent<SongEntryController>().CoverImage.sprite;
        songNameText.text = songData.SongInfoFileData.SongName;


        SetupDifficulty(songData.SongInfoFileData.DifficultyBeatmapSets[0].DifficultyBeatmaps.Select(x => x.Difficulty).ToArray());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (songData.SongInfoFileData.SongFilename != string.Empty)
                PlayLevel();
        }
    }

    void SetupDifficulty(string[] availableDifficulties)
    {
        for (int i = difficultyTabGroup.transform.childCount; i < availableDifficulties.Length ; i++)
        {
            var tabButtonObject = Instantiate(difficultyTabPrefab, difficultyTabGroup.transform);
            tabButtonObject.GetComponent<TabButton>().SetTabGroup(difficultyTabGroup);
        }

        for (int i = 0; i < availableDifficulties.Length; i++)
        {
            var buttonObject = difficultyTabGroup.TabButtons[i].transform;
            buttonObject.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableDifficulties[i].Replace("Plus", "+");
        }

        for (int i = availableDifficulties.Length; i < difficultyTabGroup.transform.childCount; i++)
        {
            Destroy(difficultyTabGroup.TabButtons[i].gameObject);
        }

        difficultyTabGroup.OnTabSelected(difficultyTabGroup.TabButtons[0]);
    }

    public void PlayLevel()
    {
        CurrentSongDataManager.Instance.SetData(songData, difficultyTabGroup.SelectedTab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Replace("+", "Plus"));
        GameManager.Instance.PlayLevel();
    }
}
