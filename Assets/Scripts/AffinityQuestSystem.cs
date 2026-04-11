using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    public int currentAffinity = 20; // Mulai dari 20 agar tidak langsung kalah jika salah sekali
    public int maxAffinity = 100;
    public int pointsPerCorrect = 20;
    public int pointsPerWrong = 20; // Poin yang dikurangi jika salah

    [Header("Scene Transitions")]
    public float delaySebelumPindah = 2.0f;
    public string sceneMenang = "EndingSelamat"; // Jika Affinity >= 100
    public string sceneKalah = "EndingGagal";    // Jika Affinity < 0

    private int indexSekarang;
    private Coroutine typingCoroutine;

    void Start()
    {
        GenerateNewQuest();
        UpdateUI();
    }

    public void OnInputEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SubmitJawaban();
        }
    }

    public void SubmitJawaban()
    {
        if (string.IsNullOrEmpty(inputJawaban.text)) return;

        // Cek apakah game sudah berakhir sebelumnya
        if (currentAffinity >= maxAffinity || currentAffinity < 0) return;

        string input = inputJawaban.text.Trim().ToUpper();
        string kunci = daftarQuest[indexSekarang].jawabanBenar.ToUpper();

        if (input == kunci)
        {
            currentAffinity += pointsPerCorrect;
            textStatus.text = "<color=green>BENAR! +" + pointsPerCorrect + " Affinity</color>";
        }
        else
        {
            currentAffinity -= pointsPerWrong;
            textStatus.text = "<color=red>SALAH! -" + pointsPerWrong + " Affinity</color>";
        }

        UpdateUI();
        inputJawaban.interactable = false;

        // CEK PERCABANGAN ENDING
        if (currentAffinity >= maxAffinity)
        {
            FinishGame(true);
        }
        else if (currentAffinity < 0)
        {
            FinishGame(false);
        }
        else
        {
            Invoke("GenerateNewQuest", 1.2f);
        }
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
            yield return new WaitForSeconds(0.05f);
        }
    }

    void UpdateUI()
    {
        textAffinity.text = $"Affinity Point: {currentAffinity}/{maxAffinity}";
    }

    void FinishGame(bool isWin)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        inputJawaban.interactable = false;

        if (isWin)
        {
            textDisplayAcak.text = "SISTEM STABIL";
            textStatus.text = "<color=green>DATA TERSINKRONISASI 100%</color>";
            StartCoroutine(TransitionRoutine(sceneMenang));
        }
        else
        {
            textDisplayAcak.text = "SISTEM CRITICAL";
            textStatus.text = "<color=red>KONEKSI TERPUTUS...</color>";
            StartCoroutine(TransitionRoutine(sceneKalah));
        }
    }

    IEnumerator TransitionRoutine(string namaScene)
    {
        yield return new WaitForSeconds(delaySebelumPindah);

        // Jika ada SceneFader di scene, gunakan fader. Jika tidak, pakai SceneManager biasa
        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();
        if (fader != null)
        {
            fader.FadeToScene(namaScene);
        }
        else
        {
            SceneManager.LoadScene(namaScene);
        }
    }
}