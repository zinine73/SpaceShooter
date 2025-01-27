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

    // 반응속도
    public float damping = 10.0f;
    // camera offset
    public float targetOffset = 2.0f;
    // smoothdamp
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        camTr = GetComponent<Transform>();
    }

    void LateUpdate()
    {
        // 추적할 대상 뒤쪽으로 distance만큼, 높이는 height만큼 이동
        Vector3 pos = tragetTr.position
                        + (-tragetTr.forward * distance)
                        + (Vector3.up * height);
        // 구면선형보간 함수를 이용해서 부드럽게 위치 변경
        //camTr.position = Vector3.Slerp(camTr.position,          // 시작 위치
        //                            pos,                        // 목표 위치
        //                            Time.deltaTime * damping);  // 시간
        // smoothdamp
        camTr.position = Vector3.SmoothDamp(camTr.position, // 시작 위치
                                            pos,            // 목표 위치
                                            ref velocity,   // 현재 속도
                                            damping);       // 도달 시간
        // 피벗좌표를 향해 회전
        camTr.LookAt(tragetTr.position + (tragetTr.up * targetOffset));
    }
}
