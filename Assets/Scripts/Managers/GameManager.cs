using BeatGame.Data;
using BeatGame.UI.Controllers;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace BeatGame.Logic.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public event Action OnSongStart;

        public bool IsPlaying;

        public double CurrentBeat;
        public double LastBeat;

        [SerializeField]
        SteamVR_Action_Boolean returnToMenuAction;

        public AudioSource audioSource;

        [SerializeField]
        GameObject leftSaber;
        [SerializeField]
        GameObject rightSaber;

        [SerializeField]
        GameObject UIPointer;
        [SerializeField]
        Camera pointerCamera;

        AudioClip songClip;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Start()
        {
            HealthManager.Instance.OnDeath += GameOver;
            LoadMenu();
            SceneFader.Instance.FadeOut(1);
        }

        void GameOver()
        {
            IsPlaying = false;
            CurrentBeat = 0;
            audioSource.Stop();

            SceneFader.Instance.FadeIn(.3f, () =>
            {
                ActivatePointer();

                SongCompletedUIController.Instance.canvas.worldCamera = pointerCamera;
                SongCompletedUIController.Instance.Display(true);
                SceneFader.Instance.FadeOut(.3f);
            });
        }

        void LoadMenu()
        {
            SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
        }

        private void SceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            ActivateSabers();

            if (scene.name == "Menu")
            {
                Canvas[] canvas = FindObjectsOfType<Canvas>();
                foreach (var item in canvas)
                {
                    item.worldCamera = pointerCamera;
                }

                ActivatePointer();

            }
        }

        void ActivateSabers()
        {
            leftSaber.SetActive(true);

            rightSaber.SetActive(true);

            UIPointer.SetActive(false);
        }

        void ActivatePointer()
        {
            leftSaber.SetActive(false);

            rightSaber.SetActive(false);

            UIPointer.SetActive(true);
            UIPointer.transform.localPosition = Vector3.zero;
            UIPointer.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            if (IsPlaying && (Input.GetKeyDown(KeyCode.X) || returnToMenuAction.GetStateDown(SteamVR_Input_Sources.RightHand)))
            {
                IsPlaying = false;
                CurrentBeat = 0;
                audioSource.Stop();
                ReturnToMenu();
            }

            if (IsPlaying)
            {
                LastBeat = CurrentBeat;
                CurrentBeat += 1 / CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * Time.deltaTime;
                if (CurrentBeat >= (float)(CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration + 5) && !audioSource.isPlaying)
                {
                    IsPlaying = false;
                    CurrentBeat = 0;
                    audioSource.Stop();
                    Invoke("DisplayEndScreen", 5);
                }
            }
        }

        public void Restart()
        {
            SceneFader.Instance.FadeIn(1, () =>
            {
                SceneFader.Instance.FadeOut(.5f);

                CurrentBeat = 0;

                leftSaber.SetActive(true);
                rightSaber.SetActive(true);

                UIPointer.SetActive(false);

                OnSongStart?.Invoke();
                IsPlaying = true;
                Invoke("PlaySong", (float)(CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration));
            });
        }

        public void DisplayEndScreen()
        {
            ActivatePointer();

            SongCompletedUIController.Instance.canvas.worldCamera = pointerCamera;
            SongCompletedUIController.Instance.Display();
        }

        public void ReturnToMenu()
        {
            SceneFader.Instance.FadeIn(1.5f, () =>
            {
                SceneManager.UnloadSceneAsync((int)SceneIndexes.Map);
                SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
                SceneFader.Instance.FadeOut(.5f);
            });
        }

        public void PlayLevel()
        {
            SceneFader.Instance.FadeIn(1, StartLoading);
        }

        void PlaySong()
        {
            audioSource.Play();
        }

        public void StartLoading()
        {
            SceneManager.UnloadSceneAsync((int)SceneIndexes.MainMenu);


            StartCoroutine(GetAudioClip());

            CurrentSongDataManager.Instance.LoadLevelData();

            StartCoroutine(Loading());
        }

        IEnumerator Loading()
        {
            bool isLoaded = false;

            while (!isLoaded)
            {
                if (songClip != null && CurrentSongDataManager.Instance.HasLoadedData)
                    isLoaded = true;

                if (!isLoaded)
                    yield return null;
            }
            audioSource.clip = songClip;

            var mapLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.Map, LoadSceneMode.Additive);
            mapLoad.completed += (AsyncOperation operation) =>
            {
                OnSongStart?.Invoke();
                SceneFader.Instance.FadeOut(.5f);
                IsPlaying = true;
                Invoke("PlaySong", (float)(CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration));
            };
        }

        IEnumerator GetAudioClip()
        {
            if (CurrentSongDataManager.Instance.SelectedSongData.AudioClip != null)
            {
                songClip = CurrentSongDataManager.Instance.SelectedSongData.AudioClip;
            }
            else
            {
                using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(
                    $"file://{CurrentSongDataManager.Instance.SelectedSongData.DirectoryPath}/{CurrentSongDataManager.Instance.SelectedSongData.SongInfoFileData.SongFilename}",
                    AudioType.OGGVORBIS))
                {
                    songClip = null;

                    yield return www.SendWebRequest();

                    if (www.isNetworkError)
                    {
                        Debug.Log(www.error);
                    }
                    else
                    {
                        songClip = DownloadHandlerAudioClip.GetContent(www);
                    }
                }
            }
        }
    }
}