using Managers;
using TMPro;
using UnityEngine;

public class CharacterSelectUI : MonoBehaviour
{
    public TextMeshProUGUI p1Text;
    public TextMeshProUGUI p2Text;

    public TextMeshProUGUI p1ReadyText;
    public TextMeshProUGUI p2ReadyText;

    void Update()
    {
        if (GameManager.Instance is not null) return;

        p1Text.text = "P1: " + GameManager.Instance.player1Character;
        p2Text.text = "P2: " + GameManager.Instance.player2Character;

        p1ReadyText.text = "P1 Ready: " + (GameManager.Instance.player1Ready ? "YES" : "NO");
        p2ReadyText.text = "P2 Ready: " + (GameManager.Instance.player2Ready ? "YES" : "NO");
    }
}