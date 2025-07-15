using System.Collections;
using System.Collections.Generic;
using Framework;
using UnityEngine;

public class InkveilProjectGame : MonoBehaviour
{
    public Transform m_EnemyPoint;
    public Transform m_GodPoint;

    public List<Transform> m_EnemyPointList;

    private void Awake()
    {
        LevelManager.instance.SetEnemyPoint(m_EnemyPoint);
        LevelManager.instance.SetEnemyPointList(m_EnemyPointList);
        LevelManager.instance.StartGame();

        GodManager.instance.OnInitPoint(m_GodPoint);
    }
}
