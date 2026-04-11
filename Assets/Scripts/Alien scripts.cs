using UnityEngine;
using TMPro;
using System.Collections;

public class NPCAutoDialogue : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public string playerTag = "Player";
    public float typingSpeed = 0.05f;
    public float delayPerLine = 2.0f;

    [TextArea(3, 10)]
    public string[] dialogLines;

    [Header("Alien Animation (NPC)")]
    public Animator alienAnimator;
    public string alienIdleAnimation = "Alien_Idle";
    public string alienTalkAnimation = "Alien_Talk";

    [Header("Post-Dialogue Action")]
    public bool matikanTriggerSetelahSelesai = true;
    public bool hancurkanObjectSetelahSelesai = false;

    [HideInInspector]
    public bool isDialogueFinished = false;

    private bool hasTriggered = false;
    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            MonoBehaviour movementScript = other.GetComponent<MonoBehaviour>();
            Animator playerAnim = other.GetComponent<Animator>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            // AMBIL AUDIO SOURCE PLAYER
            AudioSource playerAudio = other.GetComponent<AudioSource>();

            StartCoroutine(PlayDialogue(movementScript, playerAnim, rb, playerAudio));
        }
    }

    IEnumerator PlayDialogue(MonoBehaviour movement, Animator anim, Rigidbody2D rb, AudioSource audio)
    {
        hasTriggered = true;

        // 1. FREEZE PLAYER & MATIKAN SUARA
        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.enabled = false;

        // --- INI KUNCINYA: Matikan suara jalan ---
        if (audio != null && audio.isPlaying) audio.Stop();

        // 2. AKTIFKAN PANEL DIALOG
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // 3. LOOPING DIALOG
        foreach (string line in dialogLines)
        {
            if (alienAnimator != null) alienAnimator.Play(alienTalkAnimation);
            yield return StartCoroutine(TypeText(line));
            if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);
            yield return new WaitForSeconds(delayPerLine);
        }

        // 4. SELESAI
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isDialogueFinished = true;

        // Kembalikan kontrol player
        if (movement != null) movement.enabled = true;
        if (anim != null) anim.enabled = true;

        if (matikanTriggerSetelahSelesai && triggerCollider != null)
        {
            triggerCollider.isTrigger = false;
        }

        if (hancurkanObjectSetelahSelesai) Destroy(gameObject);
    }

    IEnumerator TypeText(string line)
    {
        dialogueText.text = "";
        foreach (char letter in line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}