using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Transform enemySpawnSpot;
    public Transform playerSpawnSpot;
    public GameObject enemyPrefab;
    public GameObject playerPrefab;
    public GameObject gamePanel;
    Player player;
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        var playerClone = Instantiate(playerPrefab, playerSpawnSpot.position, playerSpawnSpot.rotation);
        player = GetComponent<Player>();
        gamePanel.SetActive(true);
        Instantiate(enemyPrefab, enemySpawnSpot.position, enemySpawnSpot.rotation);
    }
    private void Update()
    {
        
    }

    
}
