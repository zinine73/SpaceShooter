using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    #region Private
    private Transform tr;
    private Animation anim;
    private readonly float initHp = 100.0f; // 초기 생명값
    private const float DAMAGE_HP = 10.0f;
    private Image hpBar; // HPBar에 연결할 변수
    #endregion

    #region Public
    public float currHp; // 현재 생명값
    public float moveSpeed = 10.0f; // 이동속도 변수
    public float turnSpeed = 80.0f; // 회전속도 변수

    public delegate void PlayerDieHandler(); // 델리게이트 선언
    public static event PlayerDieHandler OnPlayerDie; // 이벤트 선언
    #endregion

    IEnumerator Start()
    {
        // HPBar 연결
        /* ? 연산자
        GameObject go = GameObject.FindGameObjectWithTag("HP_BAR");
        if (go == null)
        {
            hpBar = null;
        }
        else
        {
            hpBar = go.GetComponent<Image>();
        }
        */
        hpBar = GameObject.FindGameObjectWithTag("HP_BAR")?.GetComponent<Image>();
        currHp = initHp;
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();

        // 애니메이션 실행
        anim.Play("Idle");

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f);
        turnSpeed = 80.0f;
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float r = Input.GetAxis("Mouse X");

        //Debug.Log("h=" + h);
        //Debug.Log("v=" + v);   

        // Transform / position 
        //transform.position += new Vector3(0, 0, 1);     

        //정규화vector
        //tr.position += Vector3.forward * 1;

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);

        tr.Translate(moveDir.normalized * Time.deltaTime * moveSpeed);

        tr.Rotate(Vector3.up * turnSpeed * Time.deltaTime * r);

        // 주인공 애니메이션 설정
        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f)
        {
            anim.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f)
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }

    private void OnTriggerEnter(Collider coll)
    {
        if (currHp >= 0.0f && coll.CompareTag("PUNCH"))
        {
            currHp -= DAMAGE_HP;
            DisplayHealth();

            Debug.Log($"Player HP = {currHp/initHp}");

            if (currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    private void DisplayHealth()
    {
        if (hpBar != null)
        {
            hpBar.fillAmount = currHp / initHp;
        }
    }

    private void PlayerDie()
    {
        Debug.Log("Player Die!!!");

        /* SendMessage 방식 말고 event 방식을 사용하자
        // MONSTER 라는 태그를 가진 모든 오브젝트를 찾아옴
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER");
        // 모든 몬스터의 OnPlayerDie 함수를 순차적으로 호출
        foreach (var item in monsters)
        {
            item.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        }
        */

        // todo: UI 에서 "Game Over"라고 보여주게 하자
        // todo: ui직접 연결 말고, 이벤트 호출을 통해서 해보자

        //GetComponent<FireCtrl>().OnPlayerDie();
        
        // 주인공 사망 이벤트 호출(발생)
        OnPlayerDie();
    }
}
