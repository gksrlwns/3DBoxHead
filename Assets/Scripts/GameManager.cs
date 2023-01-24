using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Transform[] enemySpawnSpots;
    public Transform[] itemSpawnSpots;
    public Transform playerSpawnSpot;
    public Transform bossSpawnSpot;
    public GameObject[] enemyPrefabs;
    public GameObject[] itemPrefabs;
    public GameObject playerPrefab;
    public Transform enemys;
    public int enemyCnt;
    public int playerScore;

    //public Text stageText;
    public Text timerText;
    public Text showTimeText;
    public GameObject bossGameobj;
    public RectTransform bossHp;
    public GameObject victoryPanel;
    public GameObject DefeatPanel;
    
    public bool isGame;

    Player player;
    Boss boss;
    GameObject playerClone;
    float timer = 0;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        //crossHair = gamePanel.transform.Find("Crosshair").gameObject;
    }

    private void Start()
    {
        playerClone = Instantiate(playerPrefab, playerSpawnSpot.position, playerSpawnSpot.rotation);
        player = playerClone.GetComponent<Player>();
        playerScore = player.score;
        player.gameManager = this;
        StartCoroutine(ShowTimer());
    }
    private void Update()
    {
        //플레이어 스코어 동기화
        player.score = playerScore;
        if (isGame)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00");
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if(enemyCnt == 0 && timer > 10 && !boss)
        {
            StartCoroutine(SpawnBoss());
        }
        if (boss)
        {
            bossHp.localScale = new Vector3(boss.curHp / boss.maxHp, 1, 1);
            if (boss.isDead)
                GameVictory();
        }
        if (player.isDead)
            GameDefeat();
    }
    //게임이 끝나면 쌓인 플레이어의 스코어가 적립된다.
    void GameVictory()
    {
        isGame = false;
        victoryPanel.SetActive(true);
        
    }
    void GameDefeat()
    {
        isGame = false;
        DefeatPanel.SetActive(true);
    }

    public void SceneBtn(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
    }
    IEnumerator ShowTimer()
    {
        showTimeText.text = "3";
        yield return new WaitForSeconds(1f);
        showTimeText.text = "2";
        yield return new WaitForSeconds(1f);
        showTimeText.text = "1";
        yield return new WaitForSeconds(1f);
        showTimeText.text = "게임시작";
        isGame = true;
        yield return new WaitForSeconds(1f);
        showTimeText.text = "";
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnItem());
        //StartCoroutine(TestSpawn());
    }
    IEnumerator TestSpawn()
    {
        yield return null;
        var enemyClone = Instantiate(enemyPrefabs[0], enemySpawnSpots[0].position, enemySpawnSpots[0].rotation, enemys);
        Enemy enemy = enemyClone.GetComponent<Enemy>();
        enemy.gameManager = this;
        enemyCnt++;
    }
    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            var enemyClone = Instantiate(enemyPrefabs[0], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            enemy.gameManager = this;
            enemyCnt++;
        }
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < enemySpawnSpots.Length; i = i + 2)
        {
            var enemyClone = Instantiate(enemyPrefabs[1], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            enemy.gameManager = this;
            enemyCnt++;
        }
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < enemySpawnSpots.Length; i = i + 4)
        {
            var enemyClone = Instantiate(enemyPrefabs[2], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            enemy.gameManager = this;
            enemyCnt++;
        }
    }

    IEnumerator SpawnBoss()
    {
        showTimeText.text = "보스 출현";
        isGame = false;
        timer = 0;
        yield return new WaitForSeconds(3f);
        showTimeText.text = "";
        var enemyClone = Instantiate(enemyPrefabs[3], bossSpawnSpot.position, bossSpawnSpot.rotation);
        bossGameobj.SetActive(true);
        boss = enemyClone.GetComponent<Boss>();
        boss.gameManager = this;
        isGame = true;
    }

    IEnumerator SpawnItem()
    {
        while(isGame)
        {
            int itemIndex = Random.Range(0, itemPrefabs.Length);
            int spawnIndex = Random.Range(0, itemSpawnSpots.Length);
            var itemClone = Instantiate(itemPrefabs[itemIndex], itemSpawnSpots[spawnIndex].position, Quaternion.identity);
            yield return new WaitForSeconds(10f);
        }
        
    }
}
