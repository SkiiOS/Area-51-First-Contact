using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AffinityQuestSystem : MonoBehaviour
{
    [System.Serializable]
    public class QuestData
    {
        public string kodeAneh;
        public string jawabanBenar;
    }

    [Header("UI Elements")]
    public TMP_Text textDisplayAcak;
    public TMP_InputField inputJawaban;
    public TMP_Text textAffinity;
    public TMP_Text textStatus;

    [Header("Quest Data")]
    public List<QuestData> daftarQuest;
    public int currentAffinity = 0;
    public int maxAffinity = 100;
    public int pointsPerCorrect = 20;
    public float typingSpeed = 0.05f;

    private int correctCount = 0;
    private int totalNeed = 5;
    private int indexSekarang;
    private Coroutine typingCoroutine;

    void Start()
    {
        GenerateNewQuest();
        UpdateUI();
    }

    // --- FUNGSI BARU UNTUK INPUT FIELD ---
    // Hubungkan On End Edit di Inspector ke fungsi ini
    public void OnInputEndEdit()
    {
        // Hanya jalan jika yang ditekan adalah Enter
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitJawaban();
        }
    }

    // --- FUNGSI UTAMA (Bisa dipanggil Tombol atau Enter) ---
    public void SubmitJawaban()
    {
        if (string.IsNullOrEmpty(inputJawaban.text)) return;
        if (correctCount >= totalNeed) return;

        string input = inputJawaban.text.Trim().ToUpper();
        string kunci = daftarQuest[indexSekarang].jawabanBenar.ToUpper();

        if (input == kunci)
        {
            correctCount++;
            currentAffinity += pointsPerCorrect;
            textStatus.text = "<color=green>BENAR! +20 Affinity</color>";

            if (correctCount >= totalNeed)
            {
                FinishAllQuests();
            }
            else
            {
                inputJawaban.interactable = false;
                Invoke("GenerateNewQuest", 1.2f);
            }
        }
        else
        {
            textStatus.text = "<color=red>KODE SALAH!</color>";
            inputJawaban.interactable = false;
            Invoke("GenerateNewQuest", 1.2f);
        }

        UpdateUI();
    }

    void GenerateNewQuest()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        indexSekarang = Random.Range(0, daftarQuest.Count);

        inputJawaban.interactable = true;
        inputJawaban.text = "";
        textStatus.text = "";

        typingCoroutine = StartCoroutine(TypeText(daftarQuest[indexSekarang].kodeAneh));
        inputJawaban.ActivateInputField();
    }

    IEnumerator TypeText(string textToType)
    {
        textDisplayAcak.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            textDisplayAcak.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void UpdateUI()
    {
        textAffinity.text = $"Affinity Point: {currentAffinity}/{maxAffinity}";
    }

    void FinishAllQuests()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textDisplayAcak.text = "SISTEM STABIL";
        textStatus.text = "<color=green>DATA TERSINKRONISASI 100%</color>";
        inputJawaban.interactable = false;
    }
}