using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SongInfoController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI songNameText;
    [SerializeField]
    TabGroup difficultyTabGroup;
    [SerializeField]
    GameObject difficultyTabPrefab;

    public void DisplaySong(AvailableSongData songData)
    {
        songNameText.text = songData.SongInfoFileData.SongName;
        SetupDifficulty(songData.SongInfoFileData.DifficultyBeatmapSets[0].DifficultyBeatmaps.Select(x => x.Difficulty).ToArray());
    }

    void SetupDifficulty(string[] availableDifficulties)
    {
        for (int i = 0; i < availableDifficulties.Length - difficultyTabGroup.transform.childCount; i++)
        {
            var tabButtonObject = Instantiate(difficultyTabPrefab, difficultyTabGroup.transform);
            tabButtonObject.GetComponent<TabButton>().SetTabGroup(difficultyTabGroup);
        }

        for (int i = 0; i < availableDifficulties.Length; i++)
        {
            var buttonObject = difficultyTabGroup.tabButtons[i].transform;
            buttonObject.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableDifficulties[i].Replace("Plus", "+");
        }

        for (int i = availableDifficulties.Length; i < difficultyTabGroup.transform.childCount; i++)
        {
            Destroy(difficultyTabGroup.tabButtons[i].gameObject);
        }
    }
}
