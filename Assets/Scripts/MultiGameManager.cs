using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using BackEnd;

public class MultiGameManager : MonoBehaviourPunCallbacks ,IPunObservable
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
    public int playerDieCount;

    //public Text stageText;
    public Text timerText;
    public Text showTimeText;
    public Text dieText;
    public GameObject bossGameobj;
    public RectTransform bossHp;
    public GameObject victoryPanel;
    public GameObject DefeatPanel;
    //public PhotonView photonView;

    public bool isGame;

    PhotonPlayer photonPlayer;
    Boss boss;
    float timer = 0;
    bool isMaster;
    int spotPoint;
    
    private void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        isMaster = PhotonNetwork.IsMasterClient ? true : false;
        spotPoint = isMaster ? 1 : 0;
        var player = PhotonNetwork.Instantiate
           ("PhotonPlayer", playerSpots[spotPoint].position,
           playerSpots[spotPoint].rotation);
        photonPlayer = player.GetComponent<PhotonPlayer>();
        photonPlayer.SetPlayer();
        photonPlayer.multiGameManager = this;
        //Where _where = new Where();
        //_where.Equal("id", BackendManager.instance.id);
        //photonPlayer.score = int.Parse(BackendManager.instance.BackendGetInfo("user", _where, "highscore"));
        //playerScore = photonPlayer.score; 
        StartCoroutine(ShowTimer());
    }
    private void Update()
    {
        if (isGame)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            timer += Time.deltaTime;
            timerText.text = timer.ToString("00");
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        if (enemyCnt == 0 && timer > 10 && !boss)
        {
            GameVictory(isMaster);
            //StartCoroutine(SpawnBoss());
        }
        //if (boss)
        //{
        //    bossHp.localScale = new Vector3(boss.curHp / boss.maxHp, 1, 1);
        //    if (boss.isDead)
        //        GameVictory();
        //}
        if (photonPlayer.isDead && photonPlayer)
            PlayerDie();
            
    }
    
    //게임이 끝나면 쌓인 플레이어의 스코어가 적립된다.
    void GameVictory(bool isMaster)
    {
        isGame = false;
        //Where _where = new Where();
        //Param _param = new Param();
        //_where.Equal("id", BackendManager.instance.id);
        //_param.Add("highscore", playerScore);
        //BackendManager.instance.BackendUpdateInfo("user", _where, _param);
        if (isMaster)
        {
            dieText.text = "";
            victoryPanel.SetActive(true);
        }
        else
        {
            dieText.text = "승리";
        }
        

    }
    void GameDefeat(bool isMaster)
    {
        isGame = false;
        if (isMaster)
        {
            dieText.text = "";
            DefeatPanel.SetActive(true);
        }
        else
        {
            dieText.text = "패배";
        }
        
        
    }
    void PlayerDie()
    {
        isGame = false;
        dieText.text = "You Die";
        Destroy(photonPlayer, 1f);
        StartCoroutine(FindPlayer());
    }
    IEnumerator FindPlayer()
    {
        yield return new WaitForSeconds(1.1f);
        GameObject[] a = GameObject.FindGameObjectsWithTag("Player");
        if(a.Length == 0)
            GameDefeat(isMaster);
    }
    //public override void OnLeftRoom()
    //{
    //    SceneManager.LoadScene("MainScene");
    //}
    public void RoomLoadBtn()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        PhotonNetwork.LoadLevel("RoomScene");
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
        //StartCoroutine(SpawnItem());
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(TestSpawn());
        }
        //StartCoroutine(TestSpawn2());
    }
    IEnumerator TestSpawn2()
    {
        yield return null;
        var enemyClone = Instantiate(enemyPrefabs[0], enemySpawnSpots[0].position, enemySpawnSpots[0].rotation, enemys);
        Enemy enemy = enemyClone.GetComponent<Enemy>();
        //enemy.multiGameManager = this;
        enemyCnt++;
    }
    IEnumerator TestSpawn()
    {
        yield return null;
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            var enemyClone = PhotonNetwork.Instantiate("PhotonEnemyA", enemySpawnSpots[i].position, enemySpawnSpots[i].rotation);
            PhotonEnemy enemy = enemyClone.GetComponent<PhotonEnemy>();
            enemy.multiGameManager = this;
            enemyCnt++;
        }
        
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timer);
            stream.SendNext(enemyCnt);
            //stream.SendNext(isGame);

        }
        else
        {
            timer = (float)stream.ReceiveNext();
            enemyCnt = (int)stream.ReceiveNext();
            //isGame = (bool)stream.ReceiveNext();
        }
    }
}
