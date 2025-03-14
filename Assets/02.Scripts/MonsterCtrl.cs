using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    private const int MAX_MONSTER_HP = 100;
    private const int HIT_MONSTER_HP = 10;

    #region Hash
    // 해시값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");
    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashDie = Animator.StringToHash("Die");
    #endregion

    #region Public
    public const float TIMER_CHECK = 0.3f;
    public const int SCORE_KILL = 50;
    public enum State
    {
        IDLE,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.IDLE;
    public float traceDist = 10.0f;
    public float attackDist = 2.0f;
    public bool isDie = false;
    #endregion

    #region Private
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject bloodEffect; // 혈흔 효과 프리팹
    private int hp = MAX_MONSTER_HP;
    #endregion

    // 스크립트가 활성화 될 때마다 호출되는 함수
    void OnEnable()
    {
        // 이벤트 발생 시 수행할 함수 연결 
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;

        // 몬스터의 상태를 체크하는 코루틴 함수 호출
        StartCoroutine(CheckMonsterState());
        // 상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
        StartCoroutine(MonsterAction());
    }

    // 스크립트가 비활성 될 때마다 호출되는 함수
    void OnDisable()
    {
        // 기존에 연결된 함수 해제
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            // 0.3초 대기하는 동안 제어권 넘김
            yield return new WaitForSeconds(TIMER_CHECK);
            // 몬스터의 상태가 IDE일 때 코루틴 종료
            if (state == State.DIE) yield break;

            float distance = Vector3.Distance(playerTr.position, monsterTr.position);
            if (distance <= attackDist)
            {
                state = State.ATTACK;
            }
            else if (distance <= traceDist)
            {
                state = State.TRACE;
            }
            else
            {
                state = State.IDLE;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist);
        }    
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist);
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie)
        {
            switch (state)
            {
                case State.IDLE:
                    agent.isStopped = true;
                    animator.SetBool(hashTrace, false);
                    break;
                case State.TRACE:
                    agent.SetDestination(playerTr.position);
                    agent.isStopped = false;
                    animator.SetBool(hashTrace, true);
                    animator.SetBool(hashAttack, false);
                    break;
                case State.ATTACK:
                    animator.SetBool(hashAttack, true);
                    break;
                case State.DIE:
                    isDie = true;
                    agent.isStopped = true;
                    animator.SetTrigger(hashDie);
                    // 몬스터의 Collider 비활성화
                    GetComponent<CapsuleCollider>().enabled = false;
                    // 몬스터의 손에 달려 있는 Collider 비활성화
                    SphereCollider[] sc = GetComponentsInChildren<SphereCollider>();
                    foreach (var item in sc)
                    {
                        item.enabled = false;
                    }

                    // 일정시간 대기 후 오브젝트 풀링으로 환원
                    yield return new WaitForSeconds(3.0f);
                    // 사망 후 다시 사용될 때를 위해 hp값 초기화
                    hp = MAX_MONSTER_HP;
                    isDie = false;
                    GetComponent<CapsuleCollider>().enabled = true;
                    foreach (var item in sc)
                    {
                        item.enabled = true;
                    }
                    // 상태도 평소상태로 변경
                    state = State.IDLE;
                    // 몬스터를 비활성화
                    this.gameObject.SetActive(false);
                    break;
            }
            yield return new WaitForSeconds(TIMER_CHECK);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            // 충돌한 총알 삭제
            Destroy(collision.gameObject);
        }
    }

    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        // 피격 애니메이션 실행
        animator.SetTrigger(hashHit);

        // normal vector
        Quaternion rot = Quaternion.LookRotation(normal);
        // 혈흔 효과를 생성하는 함수 호출
        ShowBloodEffect(pos, rot);

        // 몬스터의 HP 처리
        hp -= HIT_MONSTER_HP;
        if (hp <= 0)
        {
            state = State.DIE;
            GameManager.instance.DisplayScore(SCORE_KILL);
        }
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
    }

    private void OnPlayerDie()
    {
        // 몬스터의 상태를 체크하는 코루틴 정지
        StopAllCoroutines();

        // 죽지 않은 몬스터들만 춤을 추게 한다
        if (state == State.DIE) return;
        
        // 추적 정지, 애니메이션 실행
        agent.isStopped = true;
        animator.SetFloat(hashSpeed, Random.Range(0.8f, 1.2f));
        animator.SetTrigger(hashPlayerDie);
    }
}
