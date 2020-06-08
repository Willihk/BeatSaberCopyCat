using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlaying;

    public float CurrentSongTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        SceneManager.LoadScene((int)SceneIndexes.MainMenu, LoadSceneMode.Additive);
    }

    private void Update()
    {
        if (IsPlaying)
            CurrentSongTime += Time.deltaTime;
    }
}
