using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueParser : MonoBehaviour
{



    public Dialogue[] Parse(string _CSVFileName)
    {
        List<Dialogue> dialogueList = new List<Dialogue>(); // 대화 리스트 생성
        TextAsset csvData = Resources.Load<TextAsset>(_CSVFileName); // CSV 파일 가져옴

        string[] data = csvData.text.Split(new char[] {'\n'}); // 엔터 기준으로 쪼갬

        for (int i = 1; i < data.Length;)
        {
            string[] row = data[i].Split(new char[] { ',' }); // 쉼표 기준으로 쪼갬

            Dialogue dialogue = new Dialogue(); // 대사 리스트 생성

            dialogue.name = row[1]; // 대사 치는 캐릭터 이름
            List<string> contextList = new List<string>();

            do
            {
                contextList.Add(row[2]);
                if(++i < data.Length)
                {
                    row = data[i].Split(new char[] { ',' });
                }
                else
                {
                    break;
                }
            } while (row[0].ToString() == "");

            dialogue.contexts = contextList.ToArray();
            dialogueList.Add(dialogue);
        }
        return dialogueList.ToArray();
    }
}
