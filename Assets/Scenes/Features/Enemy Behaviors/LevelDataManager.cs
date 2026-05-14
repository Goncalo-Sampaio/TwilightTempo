using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelDataManager : MonoBehaviour
{
    //PLAYER:
    [HideInInspector]public PlayerHealth playerRef = null;
    //ENEMIES:
    [HideInInspector]public List<EnemyHealth> enemyRefs = new();
    //CANVAS:
    [HideInInspector]public UIManager playerCanvas = null;
    
    public static LevelDataManager Instance;

    public delegate void OnPlayerAdd();
    public static event OnPlayerAdd onPlayerAdd;

    public delegate void OnEnemyAdd();
    public static event OnEnemyAdd onEnemyAdd;

    public delegate void OnEnemyRemove();
    public static event OnEnemyRemove onEnemyRemove;

    public delegate void OnCanvasRegister();
    public static event OnCanvasRegister onCanvasRegister;
    public void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void AddEnemy(EnemyHealth enemy)
    {
        enemyRefs.Add(enemy);
        //if player already registered:
        if (playerRef != null) enemy.transform.GetComponent<EnemyReferences>().playerRef = playerRef.transform;
        onEnemyAdd?.Invoke();
        Debug.Log("Enemy Added");
    }
    public void RemoveEnemy(EnemyHealth enemy)
    {
        enemyRefs.Remove(enemy);        
        onEnemyRemove?.Invoke();
        Debug.Log("Enemy Removed");
    }    
    public void AddPlayer(PlayerHealth player)
    {        
        playerRef = player;
        onPlayerAdd?.Invoke();
        Debug.Log("Player Added");
    }    
    public void RegisterCanvas(UIManager canvas)
    {        
        playerCanvas = canvas;        
        onCanvasRegister?.Invoke();
        Debug.Log("Canvas Registered");


    }
    

    

}
