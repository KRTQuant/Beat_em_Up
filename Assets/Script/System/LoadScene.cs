using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private Button Start;
    [SerializeField] private Button Option;
    [SerializeField] private Button Exit;
    [SerializeField] private Button LevelOne;
    [SerializeField] private Button LevelTwo;
    [SerializeField] private Button Back;

    public void LoadLevelOne()
    {
        SceneManager.LoadScene("LevelOne");
    }

    public void LoadLevelTwo()
    {
        SceneManager.LoadScene("LevelTwo");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToLevelSelect()
    {
        Start.gameObject.SetActive(false);
        Option.gameObject.SetActive(false);
        Exit.gameObject.SetActive(false);
        LevelOne.gameObject.SetActive(true);
        LevelTwo.gameObject.SetActive(true);
        Back.gameObject.SetActive(true);
    }

    public void ToMainMenu()
    {
        Start.gameObject.SetActive(true);
        Option.gameObject.SetActive(true);
        Exit.gameObject.SetActive(true);
        LevelOne.gameObject.SetActive(false);
        LevelTwo.gameObject.SetActive(false);
        Back.gameObject.SetActive(false);
    }
}
