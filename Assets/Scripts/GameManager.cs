using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform[] enemySpawnSpots;
    public Transform playerSpawnSpot;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject gamePanel;

    //public Text stageText;
    public Text timerText;
    public Text hpText;
    public Text ammoText;
    public Text coinText;
    public GameObject[] curWeaponImages;

    Player player;
    GameObject playerClone;
    GameObject crossHair;
    private void Awake()
    {
        Application.targetFrameRate = 60;
        //crossHair = gamePanel.transform.Find("Crosshair").gameObject;
    }

    private void Start()
    {
        gamePanel.SetActive(true);
        playerClone = Instantiate(playerPrefab, playerSpawnSpot.position, playerSpawnSpot.rotation);
        player = playerClone.GetComponent<Player>();
        //player.crossHair = crossHair;        
        for (int i = 0; i < enemySpawnSpots.Length; i++)
        {
            Instantiate(enemyPrefab, enemySpawnSpots[i].position, enemySpawnSpots[i].rotation);
        }
    }
    private void Update()
    {
        timerText.text = Time.deltaTime.ToString();
        PlayerState();
        //if (player.isDead)
        //    Destroy(playerClone, 2f);
    }

    void PlayerState()
    {
        hpText.text = $"{player.health} / {player.maxHealth}";
        ammoText.text = $"{player.ammo} / {player.maxAmmo}";
        coinText.text = $"{player.coin}";
    }

    
}
