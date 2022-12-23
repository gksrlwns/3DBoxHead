using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform[] enemySpawnSpots;
    public Transform playerSpawnSpot;
    public Transform bossSpawnSpot;
    public GameObject[] enemyPrefabs;
    public GameObject playerPrefab;
    public Transform enemys;
    public int enemyCnt;

    //public Text stageText;
    public Text timerText;
    public Text showTimeText;
    public GameObject bossGameobj;
    public RectTransform bossHp;
    
    public bool isGame;

    Player player;
    Boss boss;
    GameObject playerClone;
    GameObject crossHair;
    float timer = 0;
    int curWeaponindex = -1;


    private void Awake()
    {
        Application.targetFrameRate = 60;
        //crossHair = gamePanel.transform.Find("Crosshair").gameObject;
    }

    private void Start()
    {
        playerClone = Instantiate(playerPrefab, playerSpawnSpot.position, playerSpawnSpot.rotation);
        player = playerClone.GetComponent<Player>();
        player.gameManager = this;
        StartCoroutine(ShowTimer());
    }
    private void Update()
    {
        if (isGame)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00");
        }
        if(enemyCnt == 0 && timer != 0)
        {
            //StartCoroutine(SpawnBoss());
        }
        if (!boss) return;
        bossHp.localScale = new Vector3(boss.curHp / boss.maxHp, 1, 1);

        //EquipWeapon(player.equipWeaponIndex);
        //PlayerState();
        //if (player.isDead)
        //    Destroy(playerClone, 2f);
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
        var enemyClone = Instantiate(enemyPrefabs[3], bossSpawnSpot.position, bossSpawnSpot.rotation);
        bossGameobj.SetActive(true);
        boss = enemyClone.GetComponent<Boss>();
        //StartCoroutine(SpawnEnemy());
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
        var enemyClone = Instantiate(enemyPrefabs[3], bossSpawnSpot.position, bossSpawnSpot.rotation);
        bossGameobj.SetActive(true);
        boss = enemyClone.GetComponent<Boss>();
        isGame = true;
    }
}
