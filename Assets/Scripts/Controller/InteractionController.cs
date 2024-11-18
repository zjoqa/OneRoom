using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


    public class InteractionController : MonoBehaviour
    {
    [SerializeField] Camera cam;

    RaycastHit  hitInfo;

    [SerializeField] GameObject go_NormalCrosshair;
    [SerializeField] GameObject go_InteractiveCrosshair;
    [SerializeField] GameObject go_Crosshair;
    [SerializeField] GameObject go_Cursor;
    [SerializeField] GameObject go_TargetNameBar;
    [SerializeField] Text txt_TargetName;

    bool isContact = false; // 상호작용 가능 여부
    public static  bool isInteract = false; // 상호작용 중인지 아닌지

    [SerializeField] ParticleSystem ps_QuestionEffect;

    DialogueManager theDM;

    public void HideUI() // 대화창 활성화 중일 때 UI 숨김
    {
        go_Crosshair.SetActive(false);
        go_Cursor.SetActive(false);
    }

    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>();
    }   

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
    private void Contact() // 상호작용 가능 여부 확인
    {
        if(hitInfo.transform.CompareTag("Interaction"))
        {
            go_TargetNameBar.SetActive(true);
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName();
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
            go_TargetNameBar.SetActive(false);
            isContact = false;
            go_InteractiveCrosshair.SetActive(false);
            go_NormalCrosshair.SetActive(true);
        }
    }

    private void ClickLeftBtn()
    {
        if(!isInteract) // 상호작용 중이 아니면
        {
            if(Input.GetMouseButtonDown(0))
                {
                    if(isContact) // 상호작용이 가능 할 때
                    {
                        Interact();
                    }
                }
        }

    }
    private void Interact() // 상호작용 실행
    {
        isInteract = true;
        Vector3 t_targetPos = hitInfo.transform.position;
        ps_QuestionEffect.gameObject.SetActive(true);
        ps_QuestionEffect.GetComponent<QuestionEffect>().SetTarget(t_targetPos);
        ps_QuestionEffect.transform.position = cam.transform.position;

        StartCoroutine(WaitCollision());
    }

    IEnumerator WaitCollision() // 충돌 대기 코루틴
    {
        // QuestionEffect의 isCollide가 true일 때까지 대기
        yield return new WaitUntil(()=> QuestionEffect.isCollide);
        QuestionEffect.isCollide = false; // 충돌 여부 초기화

        theDM.ShowDialogue();
    }
}

