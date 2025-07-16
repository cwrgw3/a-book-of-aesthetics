using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SequenceTypewriter : MonoBehaviour
{
    public TextMeshProUGUI targetText;

    [TextArea]
    public List<string> sentences = new List<string>(8)
    {
        "고대의 대륙 칼큘라에는",
        "전설적인 서적",
        "미적의 서가 존재한다.",
        "이 책을 통해 사람들은 마법을 사용할 수 있었고",
        "수많은 학자와 마법사들이 이를 발전시켜왔다.",
        "하지만 어느 날, 미적의 서가 혼돈의 군주 인테그랄에 의해 찢겨졌다.",
        "미적의 서가 사라진 이후, 대륙은 혼돈에 빠지게 되었으니",
        "이 세계를 구하기 위해서는 미적의 서를 되찾아야 한다."
    };

    [Header("딜레이 설정 (초)")]
    [Tooltip("타이핑 시 글자 사이 간격")]
    public float typeDelay = 0.05f;
    [Tooltip("삭제 시 글자 사이 간격")]
    public float deleteDelay = 0.02f;

    private int currentIndex = -1;
    private Coroutine playCoroutine;

    private void Awake()
    {
        if (targetText == null)
            targetText = GetComponent<TextMeshProUGUI>();
        // 시작 시 빈 텍스트
        targetText.text = "";
    }

    /// <summary>
    /// Animation Event에서 이 메서드를 호출하세요.
    /// (예: CutScene1 끝에 PlayNext, CutScene2 끝에 PlayNext, … 총 9번)
    /// </summary>
    public void PlayNext()
    {
        // 이미 코루틴이 돌고 있으면 중단
        if (playCoroutine != null)
            StopCoroutine(playCoroutine);

        // 다음 인덱스로 넘어가되, 리스트 범위 안에서 순환
        currentIndex = (currentIndex + 1) % sentences.Count;
        playCoroutine = StartCoroutine(DeleteAndType(sentences[currentIndex]));
    }

    private IEnumerator DeleteAndType(string newSentence)
    {
        // 1) 이전 텍스트 한 글자씩 삭제
        while (targetText.text.Length > 0)
        {
            targetText.text = targetText.text.Substring(0, targetText.text.Length - 1);
            yield return new WaitForSeconds(deleteDelay);
        }

        // 2) 잠깐 대기 후 새 문장 타이핑
        yield return null;

        targetText.text = "";
        for (int i = 0; i < newSentence.Length; i++)
        {
            targetText.text += newSentence[i];
            yield return new WaitForSeconds(typeDelay);
        }

        playCoroutine = null;
    }
}
