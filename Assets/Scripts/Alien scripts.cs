using UnityEngine;
using TMPro;
using System.Collections;

public class NPCAutoDialogue : MonoBehaviour
{
    [Header("UI Reference")]
    // Seret GameObject 'Panel' atau 'Background Dialog' ke sini
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public string playerTag = "Player";
    public float delayPerLine = 3.0f;
    [TextArea(3, 10)]
    public string[] dialogLines;

    private bool hasTriggered = false;

    private void Awake()
    {
        // MEMBUAT TMP INAKTIF SAAT START
        // Ini memastikan panel dialog tidak muncul di awal scene
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && !hasTriggered)
        {
            // Mencari komponen movement, animator, dan rigidbody pada objek yang masuk (Player)
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

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.enabled = false;
        }

        // 2. BUAT TMP AKTIF SAAT INTERAKSI
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        // 3. LOOPING DIALOG
        foreach (string line in dialogLines)
        {
            dialogueText.text = line;
            yield return new WaitForSeconds(delayPerLine);
        }

        // 4. KEMBALIKAN TMP JADI INAKTIF SAAT SELESAI
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Aktifkan kembali kontrol player
        if (movement != null) movement.enabled = true;
        if (anim != null) anim.enabled = true;
    }
}