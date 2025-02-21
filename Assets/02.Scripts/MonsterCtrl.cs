using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    // 해시값 추출
    private readonly int hashTrace = Animator.StringToHash("IsTrace");
    private readonly int hashAttack = Animator.StringToHash("IsAttack");
    private readonly int hashHit = Animator.StringToHash("Hit");

    public const float TIMER_CHECK = 0.3f;
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

    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent agent;
    private Animator animator;
    private GameObject bloodEffect; // 혈흔 효과 프리팹

    void Start()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("PLAYER").GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        bloodEffect = Resources.Load<GameObject>("BloodSprayEffect");

        //agent.destination = playerTr.position;
        // 몬스터의 상태를 체크하는 코루틴 함수 호출
        StartCoroutine(CheckMonsterState());
        // 상태에 따라 몬스터의 행동을 수행하는 코루틴 함수 호출
        StartCoroutine(MonsterAction());
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie)
        {
            // 0.3초 대기하는 동안 제어권 넘김
            yield return new WaitForSeconds(TIMER_CHECK);
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
            // 피격 애니메이션 실행
            animator.SetTrigger(hashHit);

            // 총알의 충돌 지점
            Vector3 pos = collision.GetContact(0).point;
            // normal vector
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            // 혈흔 효과를 생성하는 함수 호출
            ShowBloodEffect(pos, rot);
        }
    }

    private void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot, monsterTr);
        Destroy(blood, 1.0f);
    }
}
