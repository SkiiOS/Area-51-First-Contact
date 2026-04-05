using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Trust Point")]
    public int trustPoint = 0;

    [Header("UI")]
    public Text trustText;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        UpdateUI();
    }

    
    public void AddTrustFromInput()
    {
        trustPoint += 5; // kecil
        UpdateUI();
    }

   
    public void AddTrustFromMission()
    {
        trustPoint += 50; // besar
        UpdateUI();
    }

    void UpdateUI()
    {
        trustText.text = "Trust Point: " + trustPoint;
    }
}