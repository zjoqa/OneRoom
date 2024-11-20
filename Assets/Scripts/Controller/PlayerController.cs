using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private Transform tf_Crosshair; // 크로스헤어
        [SerializeField]
        private Transform tf_Cam; // Main Camera
        [SerializeField]
        private Vector2 camBoundary; // 캠의 가두기 영역
        [SerializeField]
        private float sightMoveSpeed; // 카메라 움직임 속도
        [SerializeField]
        private float sightSensitivity; // 카메라 회전 속도
        [SerializeField]
        private float lookLimitX; // 카메라 좌,우 방향 제한 영역
        [SerializeField]
        private float lookLimitY; // 카메라 상,하 방향 제한 영역

        private float currentAngleX;
        private float currentAngleY;

        // 카메라 제한 UI
        [SerializeField] GameObject go_NotCamDown;
        [SerializeField] GameObject go_NotCamUp;
        [SerializeField] GameObject go_NotCamLeft;
        [SerializeField] GameObject go_NotCamRight;

        // 카메라 원래 위치
        float originPosY;

        private void Start() {
            // 카메라 원래 위치 저장
            originPosY = tf_Cam.localPosition.y;
        }

        void Update()
        {
            // 커서 숨기기
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            if(!InteractionController.isInteract) // 상호작용 중이 아니면
            {
                CrosshairMoving(); // 크로스헤어 움직임 
                ViewMoving(); // 화면 움직임(마우스)  
                KeyViewMoving(); // 화면 움직임 (키보드)
                CameraLimit();// 카메라 고정
                NotCamUI(); // 카메라 제한 UI
            }
        }
        
        private void NotCamUI()
        {
            go_NotCamDown.SetActive(false);
            go_NotCamUp.SetActive(false);
            go_NotCamLeft.SetActive(false);
            go_NotCamRight.SetActive(false);
            
            if(currentAngleY >= lookLimitX) // 현재 카메라 Y축 방향(좌,우) 각도가 제한 영역 오른쪽 각도보다 크거나 같으면
                go_NotCamRight.SetActive(true);
            else if(currentAngleY <= -lookLimitX) // 현재 카메라 Y축 방향(좌,우) 각도가 제한 영역 왼쪽 각도보다 작거나 같으면
                go_NotCamLeft.SetActive(true);
            
            if(currentAngleX >= lookLimitY) // 현재 카메라 X축 방향(상,하) 각도가 제한 영역 아래 각도보다 크거나 같으면
                go_NotCamDown.SetActive(true);
            else if(currentAngleX <= -lookLimitY) // 현재 카메라 X축 방향(상,하) 각도가 제한 영역 위 각도보다 작거나 같으면   
                go_NotCamUp.SetActive(true);
        }

        private void CameraLimit()
        {
            if(tf_Cam.localPosition.x >= camBoundary.x) // 카메라 x축의 위치가 오른쪽 제한 범위보다 크거나 같으면
            {
                // 카메라의 x축을 오른쪽 제한 범위로 설정
                tf_Cam.localPosition = new Vector3(camBoundary.x , tf_Cam.localPosition.y , tf_Cam.localPosition.z);
            }
            else if(tf_Cam.localPosition.x <= -camBoundary.x) // 카메라의 x축의 위치가 왼쪽 제한범위보다 작거나 같으면
            {   // 카메라의 x축을 왼쪽 제한 범위로 설정
                tf_Cam.localPosition = new Vector3(-camBoundary.x , tf_Cam.localPosition.y , tf_Cam.localPosition.z);
            }

            if(tf_Cam.localPosition.y >= originPosY + camBoundary.y) // 카메라의 y축의 위치가 위쪽 제한 범위보다 크거나 같으면
            {   // 카메라의 y축을 위쪽 제한 범위로 설정
                tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x , originPosY + camBoundary.y , tf_Cam.localPosition.z);
            }
            else if(tf_Cam.localPosition.y <= originPosY - camBoundary.y) // 카메라의 y축의 위치가 아래쪽 제한 범위보다 작거나 같으면
            {   // 카메라의 y축을 아래쪽 제한 범위로 설정
                tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, originPosY - camBoundary.y , tf_Cam.localPosition.z);
            }
        }

        private void KeyViewMoving()
        {
            if(Input.GetAxisRaw("Horizontal") != 0) // 좌우 키 입력이 있으면
            {
                // 현재 카메라 좌,우 방향 각도에 좌우 키 입력 값 * 고개 움직임 속도를 더함
                currentAngleY += sightSensitivity * Input.GetAxisRaw("Horizontal");
                // 현재 카메라 좌,우 방향 각도를 제한 영역 왼쪽 각도와 오른쪽 각도 사이로 제한
                currentAngleY = Mathf.Clamp(currentAngleY , -lookLimitX , lookLimitX);
                // 카메라의 포지션에 좌우 키 입력 값 * 좌우 움직임 스피드를 더함
                tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x + sightMoveSpeed * Input.GetAxisRaw("Horizontal"), tf_Cam.localPosition.y, tf_Cam.localPosition.z);
            }
            if(Input.GetAxisRaw("Vertical") != 0) // 상,하 키 입력이 있으면
            {
                currentAngleX += sightSensitivity * -Input.GetAxisRaw("Vertical"); // 현재 카메라 상,하에 회전 민감도 * 상,하 키 입력 (상,하 반전)
                currentAngleX = Mathf.Clamp(currentAngleX , -lookLimitY, lookLimitY); // 현재 카메라 상,하 각도를 제한
                // 카메라 이동 구현 (카메라 포지션 Y축 + 상,하 키 입력 * 카메라 움직임 속도)
                tf_Cam.localPosition = new Vector3(tf_Cam.localPosition.x, tf_Cam.localPosition.y + sightMoveSpeed * Input.GetAxisRaw("Vertical"), tf_Cam.localPosition.z); 
            }
        }

        private void ViewMoving()
        {   // 크로스헤어의 x 포지션이 화면 오른쪽 끝 - 100 의 값보다 크거나 || 왼쪽 끝 + 100 보다 작으면
            if (tf_Crosshair.localPosition.x > (Screen.width / 2 - 100) || tf_Crosshair.localPosition.x < (-Screen.width / 2 + 100))
            {
                // currentAngleY에 크로스헤어 x 포지션이 0보다 크면 카메라 좌,우 회전 속도를 더하고 아니면 빼줌
                currentAngleY += (tf_Crosshair.localPosition.x > 0) ? sightSensitivity : -sightSensitivity;
                currentAngleY = Mathf.Clamp(currentAngleY , -lookLimitX , lookLimitX); // 현재 카메라 좌,우 각도를 제한
                // 왼쪽으로 움직이면 카메라 움직임 속도를 빼주고 오른쪽으로 움직이면 더해줌
                float t_applySpeed = (tf_Crosshair.localPosition.x > 0) ? sightMoveSpeed : -sightMoveSpeed; 
                // 카메라의 로컬포지션에 t_applySpeed의 값을 더함
                tf_Cam.localPosition =  new Vector3(tf_Cam.localPosition.x + t_applySpeed, tf_Cam.localPosition.y, tf_Cam.localPosition.z);
            }
            if (tf_Crosshair.localPosition.y > (Screen.height / 2 - 100) || tf_Crosshair.localPosition.y < (-Screen.height / 2 + 100))
            {
                currentAngleX += (tf_Crosshair.localPosition.y > 0) ? -sightSensitivity : sightSensitivity;
                currentAngleX = Mathf.Clamp(currentAngleX , -lookLimitY , lookLimitY);

                float t_applySpeed = (tf_Crosshair.localPosition.y > 0) ? sightMoveSpeed : -sightMoveSpeed; 
                tf_Cam.localPosition =  new Vector3(tf_Cam.localPosition.x , tf_Cam.localPosition.y + t_applySpeed, tf_Cam.localPosition.z); 
            }
            // 카메라의 로컬 회전 값을 currentAngleX, currentAngleY 로 설정
            tf_Cam.localEulerAngles = new Vector3(currentAngleX, currentAngleY, tf_Cam.localEulerAngles.z);
        }

        private void CrosshairMoving()
        {
            ///
            /// 마우스 화면의 원점은 왼쪽 하단이고 
            /// 크로스헤어 이미지의 로컬포지션 원점은 중앙이 원점이기에
            /// (Screen.width/ 2) 해줘야함
            ///
            tf_Crosshair.localPosition = new Vector2(Input.mousePosition.x - (Screen.width/ 2),
                                                    Input.mousePosition.y - (Screen.height / 2));
            
            float t_cursorPosX = tf_Crosshair.localPosition.x;
            float t_cursorPosY = tf_Crosshair.localPosition.y;

            t_cursorPosX =  Mathf.Clamp(t_cursorPosX , -Screen.width / 2 + 50 , Screen.width / 2 - 50);
            t_cursorPosY =  Mathf.Clamp(t_cursorPosY , -Screen.height / 2 + 50 , Screen.height / 2 - 50);

            tf_Crosshair.localPosition = new Vector2(t_cursorPosX ,t_cursorPosY);
        }
    }

