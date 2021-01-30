using BeatGame.Data;
using BeatGame.Events;
using BeatGame.Logic.Saber;
using BeatGame.UI.Controllers;
using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Valve.VR;

namespace BeatGame.Logic.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public bool IsPlaying;

        public double CurrentBeat;

        public double LastBeat;

        [SerializeField]
        GameEvent levelSetupEvent;

        [SerializeField]
        SteamVR_Action_Boolean returnToMenuAction;

        public AudioSource audioSource;

        [SerializeField]
        GameObject leftModel;
        [SerializeField]
        GameObject rightModel;
        [SerializeField]
        GameObject leftSaber;
        [SerializeField]
        GameObject rightSaber;
        [SerializeField]
        GameObject doubleSaber;

        [SerializeField]
        GameObject UIPointer;
        [SerializeField]
        Camera pointerCamera;

        AudioClip songClip;

        RemovementSystem removementSystem;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            SceneManager.sceneLoaded += SceneLoaded;
        }

        private void Start()
        {
            removementSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<RemovementSystem>();
            HealthManager.Instance.OnDeath += GameOver;
            LoadMenu();
            SceneFader.Instance.FadeOut(1);
        }

        void GameOver()
        {
            IsPlaying = false;
            CurrentBeat = 0;
            audioSource.Stop();
            removementSystem.RemoveAllSpawnedObjects();

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

        public void ActivateSabers(bool deactivatePointer = true)
        {
            if (SettingsManager.Instance.Settings["Modifiers"]["DoubleSaber"].IntValue == 1)
            {
                doubleSaber.SetActive(true);
                leftSaber.SetActive(false);
                rightSaber.SetActive(false);
            }
            else
            {
                doubleSaber.SetActive(false);
                leftSaber.SetActive(true);
                rightSaber.SetActive(true);
            }

            leftModel.SetActive(false);
            rightModel.SetActive(false);

            if (deactivatePointer)
                UIPointer.SetActive(false);
        }

        public void ActivatePointer(bool deactivateSabers = true)
        {
            if (deactivateSabers)
            {
                doubleSaber.SetActive(false);
                leftSaber.SetActive(false);
                rightSaber.SetActive(false);
            }

            leftModel.SetActive(true);
            rightModel.SetActive(true);

            UIPointer.SetActive(true);
            UIPointer.transform.localPosition = Vector3.zero;
            UIPointer.transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            if (IsPlaying)
            {

                if (Input.GetKeyDown(KeyCode.X) || (returnToMenuAction != null && returnToMenuAction.GetStateDown(SteamVR_Input_Sources.RightHand)))
                {
                    IsPlaying = false;
                    CurrentBeat = 0;
                    audioSource.Stop();
                    ReturnToMenu();
                }

                LastBeat = CurrentBeat;
                CurrentBeat += 1 / CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * Time.deltaTime;
                if (CurrentBeat > ((CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat
                                    * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration) + audioSource.clip.length) && !audioSource.isPlaying)
                {
                    IsPlaying = false;
                    CurrentBeat = 0;
                    audioSource.Stop();
                    Invoke("DisplayEndScreen", 2);
                }
            }
        }

        public void Restart()
        {
            SceneFader.Instance.FadeIn(1, () =>
            {
                removementSystem.RemoveAllSpawnedObjects();
                SceneFader.Instance.FadeOut(.5f);

                CurrentBeat = 0;

                ActivateSabers();

                levelSetupEvent.Raise();
                IsPlaying = true;
                Invoke(nameof(PlaySong), CurrentSongDataManager.Instance.SongSpawningInfo.SecondEquivalentOfBeat * CurrentSongDataManager.Instance.SongSpawningInfo.HalfJumpDuration);
            });
        }

        public void DisplayEndScreen()
        {
            ActivatePointer();
            removementSystem.RemoveAllSpawnedObjects();

            SongCompletedUIController.Instance.canvas.worldCamera = pointerCamera;
            SongCompletedUIController.Instance.Display();
        }

        public void ReturnToMenu()
        {
            removementSystem.RemoveAllSpawnedObjects();
            SceneFader.Instance.FadeIn(1.5f, () =>
            {
                songClip.UnloadAudioData();
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
            while (true)
            {
                if (songClip != null && CurrentSongDataManager.Instance.HasLoadedData)
                    break;

                yield return null;
            }

            audioSource.clip = songClip;

            var mapLoad = SceneManager.LoadSceneAsync((int)SceneIndexes.Map, LoadSceneMode.Additive);

            mapLoad.completed += (AsyncOperation operation) =>
            {
                Restart();
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

                    if (www.result == UnityWebRequest.Result.ConnectionError)
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