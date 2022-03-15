using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    [Header("Reference Component")]
    [SerializeField] CameraClampAndFollow cam;

    [Header("Wave Controller")]
    [SerializeField] private WaveOrder wave;
    [SerializeField] private enum WaveOrder { WAVE1, WAVE2, WAVE3 };
    [SerializeField] private GameObject enemyA;
    [SerializeField] private GameObject enemyB;
    [SerializeField] private float spawnAmount_TypeA;
    [SerializeField] private float spawnAmount_TypeB;
    [SerializeField] List<GameObject> enemyInWave;
    [SerializeField] bool isCollided;
    [SerializeField] bool isWaveStart;
    [SerializeField] public bool isWaveEnd;
    [SerializeField] float allEnemyAmount;

    [Header("Spawn Point Variable")]
    [SerializeField] private float spawnPointOffsetX;
    [SerializeField] private float spawnPointOffsetY;

    //Function order
    //  - Fix Camera position
    //  - Active player move only in bounds
    //  - Spawn enemy
    //  - Unlock after clear them all

    private void CheckWave()
    {
        if(wave == WaveOrder.WAVE1)
        {
            ActiveWave1();
        }
    }

    private void ActiveWave1()
    {
        cam.isActive = false;
        SpawnEnemy(spawnAmount_TypeA + 1, spawnAmount_TypeB + 1);
    }    

    private void SpawnEnemy(float typeA_Amount, float typeB_Amount)
    {
        for(int i = 1; i < typeA_Amount;i++)
        {
            Vector2 tempPos = new Vector2(cam.left.position.x + (-spawnPointOffsetX), cam.left.position.y - (spawnPointOffsetY * i));
            GameObject spawned = Instantiate<GameObject>(enemyA, tempPos, Quaternion.identity);
            enemyInWave.Add(spawned);
            spawned.SetActive(true);
        }

        for (int i = 1; i < typeB_Amount; i++)
        {
            Vector2 tempPos = new Vector2(cam.right.position.x + (spawnPointOffsetX), cam.right.position.y - (spawnPointOffsetY * i));
            GameObject spawned = Instantiate<GameObject>(enemyB, tempPos, Quaternion.identity);
            enemyInWave.Add(spawned);
            spawned.SetActive(true);
        }
        allEnemyAmount = spawnAmount_TypeA + spawnAmount_TypeB;
        isWaveStart = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("PlayerBulletCollider") && !isCollided)
        {
            CheckWave();
            isCollided = true;
        }
    }

    private void CheckIsKillAll()
    {
        for(int i = 0; i < allEnemyAmount; i++)
        {
            if(enemyInWave[i].GetComponentInChildren<StateMachineBehav_EnemyA>() == true)
            {
                if(enemyInWave[i].GetComponentInChildren<StateMachineBehav_EnemyA>().currentHp <= 0)
                {
                    enemyInWave.RemoveAt(i);
                    allEnemyAmount--;
                }
            }
            if (enemyInWave[i].GetComponentInChildren<StateMachineBehav_EnemyB>() == true)
            {
                if (enemyInWave[i].GetComponentInChildren<StateMachineBehav_EnemyB>().currentHp <= 0)
                {
                    enemyInWave.RemoveAt(i);
                    allEnemyAmount--;
                }
            }
        }
        if(allEnemyAmount <=0)
        {
            isWaveEnd = true;
            StartCoroutine(DelayBeforeResetCam());
        }
    }

    private void Update()
    {
        if (isWaveStart)
            CheckIsKillAll();
        else
            return;
    }

    public IEnumerator DelayBeforeResetCam()
    {
        yield return new WaitForSeconds(3.0f);
        cam.isActive = true;
        Destroy(this.gameObject);
    }

}
