using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  데이터베이스 매니저에 저장된 실제 대사 스크립트를 꺼내오는 클래스
/// </summary>
public class InteractionEvent : MonoBehaviour
{
    [SerializeField] DialogueEvent dialogueEvent;

    public Dialogue[] GetDialogue()
    {
        DialogueEvent t_DialogueEvent = new DialogueEvent(); // 임시 DialogueEvent 생성
        t_DialogueEvent.dialogues = DatabaseManager.instance.GetDialogue((int)dialogueEvent.line.x, (int)dialogueEvent.line.y); // 대화 정보를 임시 DialogueEvent에 저장
        for(int i = 0; i < dialogueEvent.dialogues.Length; i++) // 유니티 인스펙터에서 정의해주는 dialogueEvent의 길이만큼 반복
        {
            t_DialogueEvent.dialogues[i].tf_Target = dialogueEvent.dialogues[i].tf_Target; // 임시 DialogueEvent의 타겟을 유니티 인스펙터에서 정의해주는 dialogueEvent의 타겟으로 설정
        }

        dialogueEvent.dialogues = t_DialogueEvent.dialogues; // 임시 정보를 실제 dialogueEvent에 저장
        return dialogueEvent.dialogues; // 실제 dialogueEvent를 return  
    }
}
