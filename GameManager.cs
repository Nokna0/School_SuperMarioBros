using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public Timer timer;

    public static GameManager Instance { get; private set; }

    public int world { get; private set; }
    public int stage { get; private set; }
    public int lives { get; private set; }
    public int coins { get; private set; } 

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(gameObject);
        }
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }        
    }

    private void Start()
    {
        Application.targetFrameRate = 60; //프레임제한
        NewGame(); 
    }

    public void NewGame()
    {
        Debug.Log("버튼눌림");
        lives = 1;
        coins = 0;

        LoadLevel(1, 1);
    }

    public void LoadLevel(int world, int stage)
    {
        this.world = world;
        this.stage = stage;

        SceneManager.LoadScene($"{world}-{stage}");
    }

    public void NextLevel()
    {
        LoadLevel(world, stage + 1);
    }

    public void ResetLevel()
    {
        --lives;

        if (lives > 0)
        {
            LoadLevel(world, stage);
        }
        else
        {
            GameOver();
        }
    }

    public void ResetLevel(float delay)
    {
        Invoke(nameof(ResetLevel), delay);
    }

    private void GameOver()
    {
        NewGame();
    }

    public void AddCoin()
    {
        coins++;
        if(coins == 100)
        {
            AddLife();
            coins = 0;
        }

    }
    public void AddLife()
    {
        lives++;
    }
}
