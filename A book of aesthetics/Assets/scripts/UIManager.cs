using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Collectible UI")]
    [Tooltip("우측 상단에 표시할 수집 개수 텍스트 (TextMeshProUGUI)")]
    public TextMeshProUGUI collectibleText;

    [Header("Quiz UI")]
    [Tooltip("미적분 문제 패널")]
    public GameObject questionPanel;
    [Tooltip("미적분 문제 내용 표시용 텍스트 (TextMeshProUGUI)")]
    public TextMeshProUGUI questionText;
    [Tooltip("정답 입력용 필드 (TMP_InputField)")]
    public TMP_InputField answerInput;
    [Tooltip("정답 제출 버튼")]
    public Button submitButton;

    // 수집 개수 카운트
    int collectibleCount = 0;
    bool isQuizActive = false;

    // 예시 문제와 정답
    const string questionString = "∫2x dx = ?";
    const string correctAnswer = "x^2";

    void Awake()
    {
        // 싱글톤 초기화
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Quiz 패널 초기 비활성화
        if (questionPanel != null)
            questionPanel.SetActive(false);

        // 제출 버튼 콜백 연결
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    /// <summary>
    /// 플레이어가 종이를 먹을 때마다 호출
    /// </summary>
    public void AddCollectible(int num)
    {
        if (isQuizActive)
            return;

        collectibleCount += num;

        // 텍스트 갱신
        if (collectibleText != null)
            collectibleText.text = collectibleCount.ToString();

        // 2개 이상 모이면 퀴즈 시작
        if (collectibleCount >= 2)
            StartQuiz();
    }

    /// <summary>
    /// 퀴즈 시작: 게임 일시정지, 패널 활성화
    /// </summary>
    void StartQuiz()
    {
        isQuizActive = true;

        // 게임 정지
        Time.timeScale = 0f;

        // 문제 표시
        if (questionText != null)
            questionText.text = questionString;

        // 입력 초기화
        if (answerInput != null)
            answerInput.text = "";

        // 패널 활성화
        if (questionPanel != null)
            questionPanel.SetActive(true);
    }

    /// <summary>
    /// 제출 버튼 클릭 시 호출
    /// </summary>
    void OnSubmitAnswer()
    {
        if (answerInput == null) return;

        // 사용자가 입력한 값을 소문자로 비교
        string userAnswer = answerInput.text.Trim().ToLower();

        if (userAnswer == correctAnswer)
        {
            // 정답 처리
            EndQuiz();
        }
        else
        {
            // 오답 시 간단 피드백 (원한다면 UI로)
            Debug.LogWarning("오답입니다. 다시 시도하세요.");
        }
    }

    /// <summary>
    /// 퀴즈 종료: 게임 재개, 카운트 초기화
    /// </summary>
    void EndQuiz()
    {
        // 패널 숨기기
        if (questionPanel != null)
            questionPanel.SetActive(false);

        // 게임 재개
        Time.timeScale = 1f;

        // 수집 개수 초기화
        collectibleCount = 0;
        if (collectibleText != null)
            collectibleText.text = collectibleCount.ToString();

        isQuizActive = false;
    }
}
