using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] List<WaveController> waveController;

    private void IsLevelClear()
    {
        float temp = 0;
        for(int i = 0;i < waveController.Count; i++)
        {
            if(waveController[i].isWaveEnd)
            {
                temp++;
            }
        }
        if(temp >= waveController.Count)
        {
            StartCoroutine(DelayForChangeScene());
        }
    }

    private void Update()
    {
        IsLevelClear();
    }

    private IEnumerator DelayForChangeScene()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("MainMenu");
    }
}
