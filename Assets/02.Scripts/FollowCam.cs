using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    // 따라갈 대상
    public Transform tragetTr;
    // MainCamera자신의 Transform
    private Transform camTr;
    // 대상으로부터 거리
    [Range(2.0f, 20.0f)]
    public float distance = 10.0f;
    // y축 높이
    [Range(0.0f, 10.0f)]
    public float height = 2.0f;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // 추적할 대상 뒤쪽으로 distance만큼, 높이는 height만큼 이동
        camTr.position = tragetTr.position
                        + (-tragetTr.forward * distance)
                        + (Vector3.up * height);
        // 피벗좌표를 향해 회전
        camTr.LookAt(tragetTr.position);
    }
}
