using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    // 충돌 시작 시 발생하는 이벤트
    void OnCollisionEnter(Collision coll)
    {
        // tag값 비교
        //if (coll.collider.tag == "BULLET")
        if (coll.collider.CompareTag("BULLET"))
        {
            // 충돌한 gameObject 삭제
            Destroy(coll.gameObject);
        }
    }
}
