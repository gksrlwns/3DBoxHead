using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using BackEnd;

public class MultiGameManager : MonoBehaviour
{
    public Transform[] playerSpots;
    public Transform[] enemySpawnSpots;
    public Transform[] itemSpawnSpots;
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
    public PhotonView photonView;

    public bool isGame;

    PhotonPlayer photonPlayer;
    Boss boss;
    GameObject playerClone;
    float timer = 0;
    int spotPoint;
    bool isEscape;
    private void Start()
    {
        spotPoint = PhotonNetwork.IsMasterClient ? 0 : 1;
        var player = PhotonNetwork.Instantiate
           ("PhotonPlayer", playerSpots[spotPoint].position,
           playerSpots[spotPoint].rotation);
        photonPlayer = player.GetComponent<PhotonPlayer>();
        photonPlayer.SetPlayer();
        StartCoroutine(ShowTimer());
    }
    private void Update()
    {
        //플레이어 스코어 동기화
        photonPlayer.score = playerScore;
        isEscape = Input.GetKey(KeyCode.Escape);
        if (isEscape)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

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
        //if (enemyCnt == 0 && timer > 10 && !boss)
        //{
        //    StartCoroutine(SpawnBoss());
        //}
        if (boss)
        {
            bossHp.localScale = new Vector3(boss.curHp / boss.maxHp, 1, 1);
            if (boss.isDead)
                GameVictory();
        }
        if (photonPlayer.isDead)
            GameDefeat();
    }
    //게임이 끝나면 쌓인 플레이어의 스코어가 적립된다.
    void GameVictory()
    {
        isGame = false;
        victoryPanel.SetActive(true);
        Where _where = new Where();
        Param _param = new Param();
        _where.Equal("id", BackendManager.instance.id);
        _param.Add("highscore", playerScore);
        BackendManager.instance.BackendUpdateInfo("user", _where, _param);

    }
    void GameDefeat()
    {
        isGame = false;
        DefeatPanel.SetActive(true);
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
        //StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnItem());
        if (PhotonNetwork.IsMasterClient)
            photonView.RPC("SpawnMonsterRPC", RpcTarget.All);
    }
    [PunRPC]
    public IEnumerator TestSpawn()
    {
        yield return null;
        var enemyClone = Instantiate(enemyPrefabs[0], enemySpawnSpots[0].position, enemySpawnSpots[0].rotation, enemys);
        Enemy enemy = enemyClone.GetComponent<Enemy>();
        //enemy.gameManager = this;
        enemyCnt++;
    }
    IEnumerator SpawnEnemy()
    {
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            var enemyClone = Instantiate(enemyPrefabs[0], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            //enemy.gameManager = this;
            enemyCnt++;
        }
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < enemySpawnSpots.Length; i = i + 2)
        {
            var enemyClone = Instantiate(enemyPrefabs[1], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            //enemy.gameManager = this;
            enemyCnt++;
        }
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < enemySpawnSpots.Length; i = i + 4)
        {
            var enemyClone = Instantiate(enemyPrefabs[2], enemySpawnSpots[i].position, enemySpawnSpots[i].rotation, enemys);
            Enemy enemy = enemyClone.GetComponent<Enemy>();
            //enemy.gameManager = this;
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
        //boss.gameManager = this;
        isGame = true;
    }

    IEnumerator SpawnItem()
    {
        while (isGame)
        {
            int itemIndex = Random.Range(0, itemPrefabs.Length);
            int spawnIndex = Random.Range(0, itemSpawnSpots.Length);
            var itemClone = Instantiate(itemPrefabs[itemIndex], itemSpawnSpots[spawnIndex].position, Quaternion.identity);
            yield return new WaitForSeconds(10f);
        }

    }

}
