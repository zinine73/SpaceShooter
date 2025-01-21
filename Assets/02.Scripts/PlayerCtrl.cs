using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] private Transform tr;
    // 이동속도 변수
    public float moveSpeed = 10.0f;

    void Start()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Debug.Log("h=" + h);
        Debug.Log("v=" + v);   

        // Transform / position 
        //transform.position += new Vector3(0, 0, 1);     

        //정규화vector
        //tr.position += Vector3.forward * 1;

        tr.Translate(Vector3.forward * Time.deltaTime * v * moveSpeed);
    }
}
