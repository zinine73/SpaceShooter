using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Public
    //public Transform[] points;
    public List<Transform> points = new List<Transform>();
    public GameObject monster;
    public float createTime = 3.0f;
    #endregion

    #region Private
    private bool isGameOver;
    #endregion

    #region Property
    public bool IsGameOver
    {
        get { return isGameOver; }
        set {
            isGameOver = value;
            if (isGameOver)
            {
                CancelInvoke("CreateMonster");
            }
        }
    }
    #endregion

    #region Singleton
    public static GameManager instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
    #endregion

    void Start()
    {
        //GameObject spawnPointGroupObject = GameObject.Find("SpawnPointGroup");
        //if (spawnPointGroupObject != null)
        //{
        //    Transform spawnPointGroup = spawnPointGroupObject.GetComponent<Transform>();
        //    points = spawnPointGroup.GetComponentsInChildren<Transform>();
        //}
        Transform spawnPointGroup = GameObject.Find("SpawnPointGroup")?.transform;

        //points = spawnPointGroup?.GetComponentsInChildren<Transform>();
        //spawnPointGroup?.GetComponentsInChildren<Transform>(points);
        foreach (Transform point in spawnPointGroup)
        {
            points.Add(point);
        }

        // 일정한 시간 간격으로 함수를 호출
        InvokeRepeating("CreateMonster", 2.0f, createTime);
    }

    private void CreateMonster()
    {
        // 몬스터의 불규칙한 생성 위치 산출
        int idx = Random.Range(0, points.Count);
        // 몬스터 프리펩 생성
        Instantiate(monster, points[idx].position, points[idx].rotation);
    }
}
