using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera cam;

    RaycastHit  hitInfo;

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;

    bool isContact = false; // 상호작용 가능 여부
    bool isInteract = false; // 상호작용 중인지 아닌지

    [SerializeField] ParticleSystem ps_QuestionEffect;

    void Update()
    {
        CheckObject();
        ClickLeftBtn();
    }

    private void CheckObject()
    {
        Vector3 t_MousePos = new Vector3(Input.mousePosition.x , Input.mousePosition.y , 0);

        if(Physics.Raycast(cam.ScreenPointToRay(t_MousePos), out hitInfo , 100)) // 마우스 포지션에서 Ray를 쏴서 충돌하면
        {
            Contact();
        }
        else // 충돌 안 하면
        {
            NotContact();
        }
    }
    // Interaction Tag를 가진 객체에 hit가 될 때 실행
    private void Contact()
    {
        if(hitInfo.transform.CompareTag("Interaction"))
        {
            if(!isContact) // 상호작용이 불가능 할 때 (중복 실행 방지)
            {
                // Interaction의 객체에 크로스헤어가 머물러 있으면 isContact가 ture이기 때문에 조건문이 맞지 않아 중복 실행 방지
                isContact = true; 
                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
            }
        }
        else{// 충돌 한 객체가 Interaction Tag가 아니면
            NotContact();
        }
    }

    private void NotContact()
    {
        if(isContact) // 상호작용이 가능 할 때 (중복 실행 방지)
        {
            isContact = false;
            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);
        }
    }

    private void ClickLeftBtn()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(isContact) // 상호작용이 가능 할 때
            {
                Interact();
            }
        }
    }
    private void Interact()
    {
        isInteract = true;
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.gameObject.SetActive(true);
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos);
        ps_QuestionEffect.transform.position = cam.transform.position;
    }
}