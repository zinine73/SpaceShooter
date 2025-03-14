using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

// 반드시 필요한 컴포넌트를 삭제 안되도록 하는 어트리뷰트
[RequireComponent(typeof(AudioSource))]
public class FireCtrl : MonoBehaviour
{
    public const float BULLET_DISTANCE = 50.0f;
    // 총알 프리펩
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;
    // 총소리 음원
    public AudioClip fireSfx;

    // AudioSource Component
    private new AudioSource audio;
    private MeshRenderer muzzleFlash;
    private bool isPlayerDie;
    private RaycastHit hit;

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie;    
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie;
    }

    void Start()
    {
        audio = GetComponent<AudioSource>();
        muzzleFlash = firePos.GetComponentInChildren<MeshRenderer>();
        // 처음 시작할 때 비활성화
        muzzleFlash.enabled = false;
        isPlayerDie = false;
    }

    public void OnPlayerDie()
    {
        isPlayerDie = true;
    }

    void Update()
    {
        if (isPlayerDie) return;

        Debug.DrawRay(firePos.position, firePos.forward * BULLET_DISTANCE, Color.green);

        // 마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
            //int mask = (1 << LayerMask.NameToLayer("MONSTER_PUNCH"))
            //            + (1 << LayerMask.NameToLayer("BARREL"));
            //mask = ~mask;
            if (Physics.Raycast(firePos.position,
                                firePos.forward,
                                out hit,
                                BULLET_DISTANCE,
                                //1 << 6))
                                1 << LayerMask.NameToLayer("MONSTER_BODY")))
            {
                Debug.Log($"Hit={hit.transform.name}");
                hit.transform.GetComponent<MonsterCtrl>()?.OnDamage(hit.point, hit.normal);
            }
        }    
    }

    void Fire()
    {
        // bullet 프리펩을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);

        // 총소리 발생
        audio.PlayOneShot(fireSfx, 1.0f);

        // 총구 화염 효과 코루틴 함수 호출
        //StartCoroutine("ShowMuzzleFlash") : 가능하지만, GC발생
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator ShowMuzzleFlash()
    {
        // Offset좌표값을 랜덤 함수로 생성
        Vector2 offset = new Vector2(Random.Range(0,2), Random.Range(0,2)) * 0.5f;
        // Texture의 오프셋값 설정
        muzzleFlash.material.mainTextureOffset = offset;

        // MuzzleFlash 회전 반경
        float angle = Random.Range(0, 360);
        muzzleFlash.transform.localRotation = Quaternion.Euler(0,0,angle);

        // MuzzleFlash 크기 조절
        float scale = Random.Range(0.5f, 1.2f);
        muzzleFlash.transform.localScale = Vector3.one * scale;

        // MuzzleFlash 활성화
        muzzleFlash.enabled = true;

        // 0.2초동안 대기(제어권 양보)
        yield return new WaitForSeconds(0.2f);

        // MuzzelFlash 비활성화
        muzzleFlash.enabled = false;
    }
}
