using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


    public class InteractionController : MonoBehaviour
    {
    [SerializeField] Camera cam;

    RaycastHit  hitInfo;

    [SerializeField] GameObject go_NormalCrosshair; // 일반 크로스헤어
    [SerializeField] GameObject go_InteractiveCrosshair; // 상호작용 크로스헤어
    [SerializeField] GameObject go_Crosshair; // 크로스헤어 (부모)
    [SerializeField] GameObject go_Cursor; // Arrow
    [SerializeField] GameObject go_TargetNameBar; // ToolTip
    [SerializeField] Text txt_TargetName; // ToolTip 텍스트

    bool isContact = false; // 상호작용 가능 여부
    public static  bool isInteract = false; // 상호작용 중인지 아닌지

    [SerializeField] ParticleSystem ps_QuestionEffect; // 물음표 투사체

    [SerializeField] Image img_Interaction; // 상호작용 마름모 이미지
    [SerializeField] Image img_InteractionEffect; // 상호작용 마름모 이펙트

    DialogueManager theDM; // 대화창 매니저

    public void SettingUI(bool p_flag)
    {
        go_Crosshair.SetActive(p_flag);
        go_Cursor.SetActive(p_flag);
        go_TargetNameBar.SetActive(p_flag);

        isInteract = !p_flag;
    }

    void Start()
    {
        theDM = FindObjectOfType<DialogueManager>(); // 대화창 매니저 찾아오기
    }   

    void Update()
    {
        if(!isInteract)
        {
            CheckObject(); 
            ClickLeftBtn();
        }
    }

    private void CheckObject()
    {
        // 마우스 위치를 3D 좌표로 변환
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
        if(hitInfo.transform.CompareTag("Interaction")) // 충돌한 객체가 Interaction Tag를 가지고 있으면
        {
            go_TargetNameBar.SetActive(true); // ToolTip 활성화
            txt_TargetName.text = hitInfo.transform.GetComponent<InteractionType>().GetName(); // ToolTip 텍스트 설정
            if(!isContact) // 상호작용이 불가능 할 때 (중복 실행 방지)
            {
                // Interaction의 객체에 크로스헤어가 머물러 있으면 isContact가 ture이기 때문에 조건문이 맞지 않아 중복 실행 방지
                isContact = true; 
                go_InteractiveCrosshair.SetActive(true);
                go_NormalCrosshair.SetActive(false);
                StopCoroutine("Interaction");
                StopCoroutine("InteractionEffect");
                StartCoroutine("Interaction", true);
                StartCoroutine("InteractionEffect");
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
            StopCoroutine("Interaction");
            StartCoroutine("Interaction", false);
        }
    }

    private IEnumerator Interaction(bool p_Appear)
    {
        Color color = img_Interaction.color;
        if(p_Appear)
        {
            color.a = 0;
            while(color.a < 1)
            {
                color.a += 0.1f;
                img_Interaction.color = color;
                yield return null;
            }
        }
        else
        {
            while(color.a > 0)
            {
                color.a -= 0.1f;
                img_Interaction.color = color;
                yield return null;
            }
        }
    }

    private IEnumerator InteractionEffect()
    {
        while(isContact && !isInteract)
        {
            Color color = img_InteractionEffect.color; // 투명도 변수 초기화
            color.a = 0.5f; // 투명도 설정

            img_InteractionEffect.transform.localScale = new Vector3(1,1,1); // 크기 초기화
            Vector3 t_scale = img_InteractionEffect.transform.localScale; // 크기 변수 초기화

            while(color.a > 0)
            {
                color.a -= 0.01f; // 투명도 감소
                img_InteractionEffect.color = color; // 투명도 변경
                t_scale.Set(t_scale.x + Time.deltaTime, t_scale.y + Time.deltaTime, t_scale.z + Time.deltaTime); // 크기 증가
                img_InteractionEffect.transform.localScale = t_scale; // 크기 변경
                yield return new WaitForSeconds(0.01f);
            }
            yield return null;
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

        StopCoroutine("Interaction"); // 상호작용 중일 때 상호작용 이미지 중복 실행 방지
        // InteractionEffect 은 isInteract가 상호작용시 true로 바뀌기 때문에 자동으로 중지됨
        Color color = img_Interaction.color;
        color.a = 0;
        img_Interaction.color = color;

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


        theDM.ShowDialogue(hitInfo.transform.GetComponent<InteractionEvent>().GetDialogue());
    }
}

