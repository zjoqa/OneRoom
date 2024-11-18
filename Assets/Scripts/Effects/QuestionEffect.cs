using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class QuestionEffect : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        Vector3 targetPos = new Vector3();

        [SerializeField] ParticleSystem ps_Effect;

        public static bool isCollide = false; // 공유 자원 충돌 여부

        public void SetTarget(Vector3 _target){
            targetPos = _target;
        }

        void Update(){
            // 목표 위치가 있을 때
            if(targetPos != Vector3.zero){
                // transfrom의 위치와 목표 위치의 거리가 0.1 이상일 때
                if((transform.position - targetPos).sqrMagnitude >= 0.1f){
                    // 위치를 목표 위치로 이동
                    transform.position = Vector3.Lerp(transform.position , targetPos, moveSpeed);
                }
                else{ // 목표 위치에 도달했을 때
                    ps_Effect.gameObject.SetActive(true);
                    ps_Effect.transform.position = transform.position;
                    ps_Effect.Play();
                    isCollide = true; 
                    targetPos = Vector3.zero;
                    gameObject.SetActive(false);
                }
            }
        }
    }


