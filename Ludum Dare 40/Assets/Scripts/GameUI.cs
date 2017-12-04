using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    GameObject[] pauseObjects;
    GameObject[] playObjects;
    GameObject[] gameOverObjects;

    public float score = 0;
    public Text scoreText;

    // Use this for initialization
    void Start () {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        gameOverObjects = GameObject.FindGameObjectsWithTag("ShowOnGameOver");
        playObjects = GameObject.FindGameObjectsWithTag("ShowDuringPlay");
        hidePaused();
        hideGameOver();
        showPlay();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                showPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                hidePaused();
            }
        }
        if (Input.GetButtonUp("Fire3"))
        {
            UpdateScore(score+1);
        }
    }

    public void UpdateScore(float newScore)
    {
        score = newScore;
        scoreText.text = score.ToString();
    }


    //controls the pausing of the scene
    public void pauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            showPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            hidePaused();
        }
    }

    public void mainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    void hidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    void showPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
        hidePlay();
        hideGameOver();
    }

    void hidePlay()
    {
        foreach (GameObject g in playObjects)
        {
            g.SetActive(false);
        }
    }
    
    void showPlay()
    {
        foreach (GameObject g in playObjects)
        {
            g.SetActive(true);
        }
        hidePaused();
        hideGameOver();
    }

    void hideGameOver()
    {
        foreach (GameObject g in gameOverObjects)
        {
            g.SetActive(false);
        }
    }

    void showGameOver()
    {
        foreach (GameObject g in gameOverObjects)
        {
            g.SetActive(true);
        }
        hidePaused();
        hideGameOver();
    }
}
