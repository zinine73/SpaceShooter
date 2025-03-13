using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Public
    public const int MAX_SCORE = 99999;
    public const string KEY_SCORE = "TOT_SCORE";
    public List<Transform> points = new List<Transform>();
    // 몬스터를 미리 생성하고 저장할 리스트
    public List<GameObject> monsterPool = new List<GameObject>();
    // 오브젝트풀에 생성할 몬스터의 최대 개수
    public int maxMonsters = 10;

    public GameObject monster;
    public float createTime = 3.0f;
    public TMP_Text scoreText;
    public GameObject panelGameOver;

    #endregion

    #region Private
    private bool isGameOver;
    private int totScore = 0;
    #endregion

    #region Property
    public bool IsGameOver
    {
        get { return isGameOver; }
        set {
            isGameOver = value;
            if (isGameOver)
            {
                panelGameOver.SetActive(true);
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
        panelGameOver.SetActive(false);

        // 몬스터 오브젝트 풀 생성
        CreateMonsterPool();

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

        // 스코어 출력
        totScore = PlayerPrefs.GetInt(KEY_SCORE, 0);
        DisplayScore(0);
    }

    private void CreateMonster()
    {
        // 몬스터의 불규칙한 생성 위치 산출
        int idx = Random.Range(0, points.Count);
        // 몬스터 프리펩 생성
        //Instantiate(monster, points[idx].position, points[idx].rotation);
        // 오브젝트 풀에서 몬스터 추출
        GameObject _monster = GetMonsterInPool();
        // 추출한 몬스터의 위치와 회전을 설정
        _monster?.transform.SetPositionAndRotation(points[idx].position,
                                                    points[idx].rotation);
        // 추출한 몬스터를 활성화
        _monster?.SetActive(true);
    }

    private void CreateMonsterPool()
    {
        for (int i = 0; i < maxMonsters; i++)
        {
            var _monster = Instantiate<GameObject>(monster);
            _monster.name = $"Monster_{i:00}";
            _monster.SetActive(false);
            monsterPool.Add(_monster);
        }
    }

    public GameObject GetMonsterInPool()
    {
        // 오브젝트 풀의 처음부터 끝까지 순회
        foreach (var _monster in monsterPool)
        {
            // 비활성화 여부로 사용 가능한 몬스터인지 판단
            if (_monster.activeSelf == false)
            {
                return _monster;
            }
        }
        return null;
    }

    public void DisplayScore(int score)
    {
        totScore += score;
        if (totScore > MAX_SCORE)
        {
            totScore = MAX_SCORE;
        }
        scoreText.text = $"<color=#00ff00>SCORE: </color><color=#ff0000>{totScore:#,##0}</color>";
        PlayerPrefs.SetInt(KEY_SCORE, totScore);
    }
}
