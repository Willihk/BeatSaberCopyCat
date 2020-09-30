using BeatGame.Data;
using BeatGame.Data.Score;
using BeatGame.Logic.Managers;
using BeatGame.UI.Components.Tabs;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace BeatGame.UI.Controllers
{
    public class SongInfoController : MonoBehaviour
    {
        [SerializeField]
        private Image songBackgroundImage;
        [SerializeField]
        private TextMeshProUGUI songNameText;
        [SerializeField]
        private TextMeshProUGUI ScoreText;
        [SerializeField]
        TabGroup difficultySetTabGroup;
        [SerializeField]
        TabGroup difficultyTabGroup;
        [SerializeField]
        GameObject difficultyTabPrefab;
        [SerializeField]
        AudioSource songPreviewAudioSource;

        [SerializeField, Header("Stats")]
        private TextMeshProUGUI timeValueText;
        [SerializeField]
        private TextMeshProUGUI BPMValueText;
        [SerializeField]
        private TextMeshProUGUI NoteCountValueText;

        AvailableSongData songData;

        int difficultySetIndex;
        int difficultyIndex;

        HighScoreData HighScoreData;

        public void DisplaySong(AvailableSongData songData, GameObject selectedSongObject)
        {
            this.songData = songData;

            songBackgroundImage.sprite = selectedSongObject.GetComponent<SongEntryController>().CoverImage.sprite;
            songNameText.text = songData.SongInfoFileData.SongName;

            SetupDifficultySets(songData.SongInfoFileData.DifficultyBeatmapSets.Select(x => x.BeatmapCharacteristicName).ToArray());
            SetupDifficulties();

            StartCoroutine(PreviewSong());
            SetStats();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                if (songData.SongInfoFileData.SongFilename != string.Empty)
                    PlayLevel();
            }
        }

        public void DifficultySetChanged(int index)
        {
            difficultySetIndex = index;
            SetupDifficulties();
        }

        public void DifficultyChanged(int index)
        {
            difficultyIndex = index;
            SetStats();
        }

        string[] GetDifficultyLabels()
        {
            string[] labels = new string[songData.SongInfoFileData.DifficultyBeatmapSets[difficultySetIndex].DifficultyBeatmaps.Length];

            for (int i = 0; i < songData.SongInfoFileData.DifficultyBeatmapSets[difficultySetIndex].DifficultyBeatmaps.Length; i++)
            {
                var beatMap = songData.SongInfoFileData.DifficultyBeatmapSets[difficultySetIndex].DifficultyBeatmaps[i];

                if (string.IsNullOrEmpty(beatMap.CustomData.DifficultyLabel))
                {
                    labels[i] = beatMap.Difficulty.Replace("Plus", "+");
                }
                else
                {
                    labels[i] = beatMap.CustomData.DifficultyLabel;
                }
            }
            return labels;
        }

        void SetStats()
        {
            if (difficultyTabGroup.SelectedTab != null)
            {
                HighScoreData = HighScoreManager.Instance.GetHighScoreForSong(
                    songData.SongInfoFileData.SongName,
                    songData.SongInfoFileData.LevelAuthorName,
                    songData.SongInfoFileData.DifficultyBeatmapSets[difficultySetIndex].BeatmapCharacteristicName,
                    songData.SongInfoFileData.DifficultyBeatmapSets[difficultySetIndex].DifficultyBeatmaps[difficultyIndex].Difficulty);

                if (HighScoreData.Score == 0)
                    ScoreText.text = "-";
                else
                    ScoreText.text = HighScoreData.Score.ToString();
            }

            BPMValueText.text = songData.SongInfoFileData.BeatsPerMinute.ToString();

            if (songData.AudioClip != null)
                timeValueText.text = $"{math.floor(songData.AudioClip.length / 60)}:{math.floor(songData.AudioClip.length % 60).ToString("00")}";
        }

        void SetupDifficulties()
        {
            string[] availableDifficulties = GetDifficultyLabels();

            for (int i = difficultyTabGroup.transform.childCount; i < availableDifficulties.Length; i++)
            {
                var tabButtonObject = Instantiate(difficultyTabPrefab, difficultyTabGroup.transform);
                tabButtonObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(difficultyTabGroup);
            }

            for (int i = 0; i < availableDifficulties.Length; i++)
            {
                var buttonObject = difficultyTabGroup.TabButtons[i].transform;
                buttonObject.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableDifficulties[i];
            }

            for (int i = availableDifficulties.Length; i < difficultyTabGroup.transform.childCount; i++)
            {
                Destroy(difficultyTabGroup.TabButtons[i].gameObject);
            }

            difficultyTabGroup.OnTabSelected(difficultyTabGroup.TabButtons[0]);
        }

        void SetupDifficultySets(string[] availableDifficultySets)
        {
            for (int i = difficultySetTabGroup.transform.childCount; i < availableDifficultySets.Length; i++)
            {
                var tabButtonObject = Instantiate(difficultyTabPrefab, difficultySetTabGroup.transform);
                tabButtonObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(difficultySetTabGroup);
            }

            for (int i = 0; i < availableDifficultySets.Length; i++)
            {
                var buttonObject = difficultySetTabGroup.TabButtons[i].transform;
                buttonObject.GetChild(0).GetComponent<TextMeshProUGUI>().text = availableDifficultySets[i].Replace("Plus", "+");
            }

            for (int i = availableDifficultySets.Length; i < difficultySetTabGroup.transform.childCount; i++)
            {
                Destroy(difficultySetTabGroup.TabButtons[i].gameObject);
            }

            difficultySetTabGroup.OnTabSelected(difficultySetTabGroup.TabButtons[0]);
        }

        public IEnumerator PreviewSong()
        {
            songPreviewAudioSource.Stop();
            if (songData.AudioClip == null)
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                    $"file://{songData.DirectoryPath}/{songData.SongInfoFileData.SongFilename}",
                    AudioType.OGGVORBIS))
                {

                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        songData.AudioClip = ((DownloadHandlerAudioClip)www.downloadHandler).audioClip;
                    }
                }
            }
            songPreviewAudioSource.clip = songData.AudioClip;
            songPreviewAudioSource.time = (float)songData.SongInfoFileData.PreviewStartTime;
            StopCoroutine("fadeSource");
            songPreviewAudioSource.Play();
            StartCoroutine(fadeSource(songPreviewAudioSource, 0, 1, .4f));

            SetStats();
        }

        IEnumerator fadeSource(AudioSource sourceToFade, float startVolume, float endVolume, float duration)
        {
            float startTime = Time.time;

            while (true)
            {
                float elapsed = Time.time - startTime;

                sourceToFade.volume = Mathf.Clamp01(Mathf.Lerp(startVolume, endVolume, elapsed / duration));

                if (sourceToFade.volume == endVolume)
                {
                    break;
                }

                yield return null;
            }
        }

        public void PlayLevel()
        {
            CurrentSongDataManager.Instance.SelectedSongData = songData;
            CurrentSongDataManager.Instance.SelectMap(songData, difficultySetIndex, difficultyIndex);

            GameManager.Instance.PlayLevel();
        }
    }
}