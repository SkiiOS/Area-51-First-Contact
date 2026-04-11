using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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
        // Ambil referensi collider di awal
        triggerCollider = GetComponent<Collider2D>();

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isDialogueFinished = false;

        if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            // Ambil komponen player untuk di-freeze
            MonoBehaviour movementScript = other.GetComponent<MonoBehaviour>();
            Animator playerAnim = other.GetComponent<Animator>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            StartCoroutine(PlayDialogue(movementScript, playerAnim, rb));
        }
    }

    IEnumerator PlayDialogue(MonoBehaviour movement, Animator anim, Rigidbody2D rb)
    {
        hasTriggered = true;

        // 1. FREEZE PLAYER (Menggunakan linearVelocity untuk Unity 2023+)
        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (anim != null) anim.enabled = false;

        // 2. AKTIFKAN PANEL DIALOG
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // 3. LOOPING DIALOG DENGAN EFEK MENGETIK
        foreach (string line in dialogLines)
        {
            // Ambarukmo mulai bicara
            if (alienAnimator != null) alienAnimator.Play(alienTalkAnimation);

            yield return StartCoroutine(TypeText(line));

            // Ambarukmo kembali idle saat jeda baca
            if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);

            yield return new WaitForSeconds(delayPerLine);
        }

        // 4. SELESAI
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isDialogueFinished = true;

        // Kembalikan kontrol player
        if (movement != null) movement.enabled = true;
        if (anim != null) anim.enabled = true;

        // --- SOLUSI AGAR TIDAK TEMBUS ---
        if (matikanTriggerSetelahSelesai)
        {
            if (triggerCollider != null)
            {
                // MATIKAN 'Is Trigger' agar Collider jadi solid (seperti tembok)
                triggerCollider.isTrigger = false;

                // ATAU jika ingin benar-benar hilang/tidak bisa disentuh sama sekali:
                // triggerCollider.enabled = false; 
            }
            Debug.Log("Trigger dimatikan, sekarang objek menjadi padat/solid.");
        }
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