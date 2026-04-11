using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SimpleBranchingDialogue : MonoBehaviour
{
    [Header("UI Dialogue Elements")]
    public TMP_Text textDisplay;
    public GameObject choicePanel; // Panel berisi 2 tombol
    public Button buttonGood;
    public Button buttonBad;

    [Header("Dialogue Content")]
    [TextArea(2, 5)] public string[] introDialogue; // Ambarukmo tidak dikenal bicara duluan
    [TextArea(2, 5)] public string[] jalurGood;      // Dr. Berry percaya
    [TextArea(2, 5)] public string[] jalurBad;       // Dr. Berry menyerang

    [Header("Scene Transitions")]
    public float delaySebelumPindah = 2.0f;
    public string sceneGoodEnding = "GoodEndingScene";
    public string sceneApocalypse = "ApocalypseEndingScene";

    private Coroutine typingCoroutine;

    void Start()
    {
        // Setup Awal
        choicePanel.SetActive(false);

        // Setup Button Listeners
        buttonGood.onClick.AddListener(() => PilihJalur(true));
        buttonBad.onClick.AddListener(() => PilihJalur(false));

        // Mulai Intro
        StartCoroutine(StartIntro());
    }

    IEnumerator StartIntro()
    {
        foreach (string line in introDialogue)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(1.5f);
        }

        // Tampilkan pilihan setelah intro selesai
        textDisplay.text = "Dr. Berry: (Komputer mendeteksi pesan masuk... apa yang harus aku lakukan?)";
        choicePanel.SetActive(true);
    }

    void PilihJalur(bool isGood)
    {
        choicePanel.SetActive(false);
        if (isGood)
        {
            StartCoroutine(MainkanAlur(jalurGood, sceneGoodEnding));
        }
        else
        {
            StartCoroutine(MainkanAlur(jalurBad, sceneApocalypse));
        }
    }

    IEnumerator MainkanAlur(string[] lines, string sceneTujuan)
    {
        foreach (string line in lines)
        {
            yield return StartCoroutine(TypeText(line));
            yield return new WaitForSeconds(2.0f);
        }

        // Selesai dialog, langsung pindah scene
        yield return new WaitForSeconds(delaySebelumPindah);
        SceneManager.LoadScene(sceneTujuan);
    }

    IEnumerator TypeText(string textToType)
    {
        textDisplay.text = "";
        foreach (char letter in textToType.ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(0.05f);
        }
    }
}