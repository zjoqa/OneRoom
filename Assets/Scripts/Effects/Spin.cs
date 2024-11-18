using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] float spinSpeed; // 속도
    [SerializeField] Vector3 spinDir; // spin 방향

    private void Update() {
        transform.Rotate(spinDir * spinSpeed * Time.deltaTime);
    }
}
