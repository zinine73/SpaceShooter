using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCtrl : MonoBehaviour
{
    // 총알 프리펩
    public GameObject bullet;
    // 총알 발사 좌표
    public Transform firePos;

    void Update()
    {
        // 마우스 왼쪽 버튼을 클릭했을 때 Fire 함수 호출
        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }    
    }

    void Fire()
    {
        // bullet 프리펩을 동적으로 생성
        Instantiate(bullet, firePos.position, firePos.rotation);
    }
}
