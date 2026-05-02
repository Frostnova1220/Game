using TMPro;  // 如果用旧版Text，请替换为 using UnityEngine.UI;
using UnityEngine;


public class DialogueController : MonoBehaviour
{
    public GameObject Player;
    public GameObject DialogUI;
    public TextMeshProUGUI dialogueText;
    public TextAsset dialogueFile;

    private string[] lines;
    private int currentIndex = 0;


    void Start()
    {
        lines = dialogueFile.text.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        UpdateDialogue();
    }

    void Update()
    {
        if (gameObject.activeSelf)
            Player.GetComponent<Player_X>().currentState = Player_X.State.Idle;
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void NextLine()
    {
        currentIndex++;
        UpdateDialogue();
    }

    void UpdateDialogue()
    {
        if (currentIndex + 1 > lines.Length)
            DialogUI.SetActive(false);
        dialogueText.text = lines[currentIndex];

    }
    private void OnDisable()
    {
        currentIndex = 0;
    }
}