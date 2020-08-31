using BeatGame.Data;
using BeatGame.Logic.Managers;
using BeatGame.UI.Components.Tabs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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
        Components.Tabs.TabGroup difficultyTabGroup;
        [SerializeField]
        GameObject difficultyTabPrefab;
        [SerializeField]
        AudioSource songPreviewAudioSource;

        [SerializeField, Header("Stats")]
        private TextMeshProUGUI timeValueText;

        AvailableSongData songData;

        public void DisplaySong(AvailableSongData songData, GameObject selectedSongObject)
        {
            this.songData = songData;

            songBackgroundImage.sprite = selectedSongObject.GetComponent<SongEntryController>().CoverImage.sprite;
            songNameText.text = songData.SongInfoFileData.SongName;


            SetupDifficulty(songData.SongInfoFileData.DifficultyBeatmapSets[0].DifficultyBeatmaps.Select(x => x.Difficulty).ToArray());
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

        void SetStats()
        {
            if (songData.AudioClip != null)
                timeValueText.text = $"{math.floor(songData.AudioClip.length / 60)}:{math.floor(songData.AudioClip.length % 60).ToString("00")}";
        }

        void SetupDifficulty(string[] availableDifficulties)
        {
            for (int i = difficultyTabGroup.transform.childCount; i < availableDifficulties.Length; i++)
            {
                var tabButtonObject = Instantiate(difficultyTabPrefab, difficultyTabGroup.transform);
                tabButtonObject.GetComponent<Components.Tabs.TabButton>().SetTabGroup(difficultyTabGroup);
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
                        songData.AudioClip = DownloadHandlerAudioClip.GetContent(www);
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
            }//end while
        }

        public void PlayLevel()
        {
            CurrentSongDataManager.Instance.SelectedSongData = songData;
            CurrentSongDataManager.Instance.Difficulity = difficultyTabGroup.SelectedTab.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Replace("+", "Plus");

            GameManager.Instance.PlayLevel();
        }

    }
}