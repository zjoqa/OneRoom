using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [SerializeField] string csv_FileName;
    
    /// <summary>
    /// 대화 정보를 저장하는 딕셔너리
    /// </summary>
    Dictionary<int, Dialogue> dialogueDic = new Dictionary<int, Dialogue>();

    public static bool isFinish = false;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DialogueParser theParser = GetComponent<DialogueParser>();
            Dialogue[] dialogues = theParser.Parse(csv_FileName); // Parse 함수를 통해 CSV 파일을 파싱하여 dialogue 배열로 반환
            for(int i = 0; i < dialogues.Length; i++)
            {
                dialogueDic.Add(i+1, dialogues[i]); // dialogueDic에 대화 정보를 추가
            }
            isFinish = true;
        }
    }
    // 대화 정보를 가져오는 함수
    public Dialogue[] GetDialogue(int _StartNum, int _EndNum)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 임시 대화 리스트 생성

        for(int i = 0 ; i <= _EndNum - _StartNum; i++)
        {
            dialogueList.Add(dialogueDic[_StartNum + i]); // 대화 정보를 임시 대화 리스트에 추가
        }
        return dialogueList.ToArray(); // 임시 대화 리스트를 배열로 변환 후 return
    }
}
