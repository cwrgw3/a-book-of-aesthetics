using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public TMP_Text collectibleText; // Inspector�� �巡���� UI Text
    int count = 0;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void AddCollectible(int num)
    {
        count += num;
        collectibleText.text = count.ToString();
    }
}
