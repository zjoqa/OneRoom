using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public void CameraTargetting(Transform p_Target, float p_CamSpeed = 0.05f)
    {
        if(p_Target != null)
        {
            StopAllCoroutines();
            StartCoroutine(CameraTargettingCoroutine(p_Target, p_CamSpeed));
        }
    }


    // 카메라가 타겟팅할 대상의 정면 위치로 이동하는 코루틴
    IEnumerator CameraTargettingCoroutine(Transform p_Target, float p_CamSpeed = 0.05f)
    {
        Vector3 t_TargetPos = p_Target.position;
        Vector3 t_TargetFrontPos = t_TargetPos + p_Target.forward; // 카메라가 타겟팅할 대상의 앞쪽 위치    
        Vector3 t_Direction = (t_TargetPos - t_TargetFrontPos).normalized; // 카메라가 타겟팅할 대상의 앞쪽 방향
        // 카메라가 타겟의 정면에 도달하거나 Angle(A,B)의 각도 차가 0.5f 이하일 때 까지 반복    
        while(transform.position != t_TargetFrontPos || Quaternion.Angle(transform.rotation, Quaternion.LookRotation(t_Direction)) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, t_TargetFrontPos, p_CamSpeed); // 카메라가 타겟 정면 위치로 이동
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(t_Direction), p_CamSpeed); // 카메라가 타겟 정면 방향을 바라보도록 회전    
            yield return null;
        }
    }
}
