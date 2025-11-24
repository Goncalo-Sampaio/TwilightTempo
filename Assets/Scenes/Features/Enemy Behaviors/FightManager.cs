using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(0)]
public class FightManager : MonoBehaviour
{
    private static FightManager instance;
    public static FightManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    public Transform Target;
    public float RadiusAroundTarget = 0.5f;
    
    public IList<Enemy> Enemies = new List<Enemy>();
    [SerializeField] private int openSlots = 8;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    //private void OnGUI()
    //{
    //    if (GUI.Button(new Rect(20,20,200,50), "Move to Target"))
    //    {
    //        MakeAgentsCircleTarget();
    //    }
    //}
    [Button]
    public void MakeAgentsCircleTarget()
    {
        //Move all of them for now
        // Then later we lock this to open slots
        for (int i = 0; i < Enemies.Count; i++)
        {
            Enemies[i].MoveTo(new Vector3(
                Target.position.x + RadiusAroundTarget * Mathf.Cos(2 * Mathf.PI * i / Enemies.Count),
                Target.position.y,
                Target.position.z + RadiusAroundTarget * Mathf.Sin(2 * Mathf.PI * i / Enemies.Count)));
        }
    }
}
