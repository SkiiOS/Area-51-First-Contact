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
    public float typingSpeed = 0.05f; // Kecepatan mengetik per huruf
    public float delayPerLine = 2.0f; // Jeda setelah satu baris selesai diketik

    [TextArea(3, 10)]
    public string[] dialogLines;

    [Header("Alien Animation (NPC)")]
    // Tarik Animator si Alien (GameObject ini atau anaknya) ke sini
    public Animator alienAnimator;
    public string alienIdleAnimation = "Alien_Idle"; // Nama state animasi Idle Alien
    public string alienTalkAnimation = "Alien_Talk"; // Nama state animasi Bicara Alien

    // --- VARIABEL UNTUK SCRIPT LAIN ---
    [HideInInspector]
    public bool isDialogueFinished = false;

    private bool hasTriggered = false;

    private void Awake()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        isDialogueFinished = false;

        // Pastikan alien mulai dengan animasi Idle
        if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            MonoBehaviour movementScript = other.GetComponent<MonoBehaviour>();
            Animator playerAnim = other.GetComponent<Animator>();
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();

            StartCoroutine(PlayDialogue(movementScript, playerAnim, rb));
        }
    }

    IEnumerator PlayDialogue(MonoBehaviour movement, Animator anim, Rigidbody2D rb)
    {
        hasTriggered = true;

        // 1. FREEZE PLAYER
        if (movement != null) movement.enabled = false;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        // Opsional: Matikan animator player atau set ke state Idle
        if (anim != null) anim.enabled = false;

        // 2. AKTIFKAN PANEL DIALOG
        if (dialoguePanel != null) dialoguePanel.SetActive(true);

        // 3. LOOPING DIALOG DENGAN EFEK MENGETIK & ANIMASI ALIEN
        foreach (string line in dialogLines)
        {
            // ALIEN MULAI BICARA
            if (alienAnimator != null) alienAnimator.Play(alienTalkAnimation);

            yield return StartCoroutine(TypeText(line));

            // ALIEN KEMBALI IDLE SAAT JEDA
            if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);

            yield return new WaitForSeconds(delayPerLine);
        }

        // 4. SELESAI
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        // Pastikan Alien kembali Idle permanen setelah dialog usai
        if (alienAnimator != null) alienAnimator.Play(alienIdleAnimation);

        // Aktifkan kembali kontrol player
        if (movement != null) movement.enabled = true;
        if (anim != null) anim.enabled = true;

        // --- TRIGGER UNTUK SCRIPT LAIN ---
        isDialogueFinished = true;
        Debug.Log("Dialog Selesai! Script pindah scene sekarang diizinkan.");
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