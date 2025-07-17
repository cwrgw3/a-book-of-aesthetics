using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Collectible UI")]
    [Tooltip("���� ��ܿ� ǥ���� ���� ���� �ؽ�Ʈ (TextMeshProUGUI)")]
    public TextMeshProUGUI collectibleText;

    [Header("Quiz UI")]
    [Tooltip("������ ���� �г�")]
    public GameObject questionPanel;
    [Tooltip("������ ���� ���� ǥ�ÿ� �ؽ�Ʈ (TextMeshProUGUI)")]
    public TextMeshProUGUI questionText;
    [Tooltip("���� �Է¿� �ʵ� (TMP_InputField)")]
    public TMP_InputField answerInput;
    [Tooltip("���� ���� ��ư")]
    public Button submitButton;

    // ���� ���� ī��Ʈ
    int collectibleCount = 0;
    bool isQuizActive = false;

    // ���� ������ ����
    const string questionString = "∫ 2x dx = ?";
    const string correctAnswer = "x^2";

    void Awake()
    {
        // �̱��� �ʱ�ȭ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Quiz �г� �ʱ� ��Ȱ��ȭ
        if (questionPanel != null)
            questionPanel.SetActive(false);

        // ���� ��ư �ݹ� ����
        if (submitButton != null)
            submitButton.onClick.AddListener(OnSubmitAnswer);
    }

    /// <summary>
    /// �÷��̾ ���̸� ���� ������ ȣ��
    /// </summary>
    public void AddCollectible(int num)
    {
        if (isQuizActive)
            return;

        collectibleCount += num;
        // Debug.Log(collectibleText.text);
        // �ؽ�Ʈ ����
        if (collectibleText != null)
            collectibleText.text = collectibleCount.ToString();

        // 2�� �̻� ���̸� ���� ����
        if (collectibleCount >= 2)
            StartQuiz();
    }

    /// <summary>
    /// ���� ����: ���� �Ͻ�����, �г� Ȱ��ȭ
    /// </summary>
    void StartQuiz()
    {
        isQuizActive = true;

        // ���� ����
        Time.timeScale = 0f;

        // ���� ǥ��
        if (questionText != null)
            questionText.text = questionString;

        // �Է� �ʱ�ȭ
        if (answerInput != null)
            answerInput.text = "";

        // �г� Ȱ��ȭ
        if (questionPanel != null)
            questionPanel.SetActive(true);
    }

    /// <summary>
    /// ���� ��ư Ŭ�� �� ȣ��
    /// </summary>
    void OnSubmitAnswer()
    {
        if (answerInput == null) return;

        // ����ڰ� �Է��� ���� �ҹ��ڷ� ��
        string userAnswer = answerInput.text.Trim().ToLower();

        if (userAnswer == correctAnswer)
        {
            // ���� ó��
            EndQuiz();
        }
        else
        {
            // ���� �� ���� �ǵ�� (���Ѵٸ� UI��)
            Debug.LogWarning("�����Դϴ�. �ٽ� �õ��ϼ���.");
        }
    }

    /// <summary>
    /// ���� ����: ���� �簳, ī��Ʈ �ʱ�ȭ
    /// </summary>
    void EndQuiz()
    {
        // �г� �����
        if (questionPanel != null)
            questionPanel.SetActive(false);

        // ���� �簳
        Time.timeScale = 1f;

        // ���� ���� �ʱ�ȭ
        collectibleCount = 0;
        if (collectibleText != null)
            collectibleText.text = collectibleCount.ToString();

        isQuizActive = false;
    }
}
