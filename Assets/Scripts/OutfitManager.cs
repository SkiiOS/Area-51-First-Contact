using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class OutfitManager : MonoBehaviour
{
    [Header("Body Reference")]
    public SpriteRenderer bodyRenderer;      // Karakter telanjang/badan
    public Animator bodyAnimator;             // Animator badan

    [Header("Outfit Setup")]
    public GameObject currentOutfitObject;    // Object baju aktif sekarang
    private SpriteRenderer outfitRenderer;
    private Animator outfitAnimator;

    [Header("Outfit Prefabs (Optional)")]
    public List<GameObject> outfitPrefabs;    // Prefab baju siap pakai

    [Header("Sync Settings")]
    public bool syncAnimation = true;         // Sinkronkan animasi dengan body
    public bool syncFlip = true;              // Sinkronkan flip/arah

    void Awake()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyAnimator == null)
            bodyAnimator = GetComponent<Animator>();

        // Setup outfit yang sudah ada
        if (currentOutfitObject != null)
        {
            SetupOutfit(currentOutfitObject);
        }
    }

    void Update()
    {
        if (syncAnimation && outfitAnimator != null && bodyAnimator != null)
        {
            SyncAnimation();
        }

        if (syncFlip && outfitRenderer != null && bodyRenderer != null)
        {
            outfitRenderer.flipX = bodyRenderer.flipX;
            outfitRenderer.flipY = bodyRenderer.flipY;
        }
    }

    // Ganti outfit dengan prefab baru
    public void ChangeOutfit(GameObject newOutfitPrefab)
    {
        // Hapus outfit lama
        if (currentOutfitObject != null)
        {
            Destroy(currentOutfitObject);
        }

        // Instantiate outfit baru
        currentOutfitObject = Instantiate(newOutfitPrefab, transform);
        currentOutfitObject.transform.localPosition = Vector3.zero;
        currentOutfitObject.name = "Outfit_" + newOutfitPrefab.name;

        SetupOutfit(currentOutfitObject);
    }

    // Setup outfit yang sudah attach
    void SetupOutfit(GameObject outfit)
    {
        outfitRenderer = outfit.GetComponent<SpriteRenderer>();
        outfitAnimator = outfit.GetComponent<Animator>();

        if (outfitAnimator != null && bodyAnimator != null)
        {
            // Copy runtime controller dari body
            outfitAnimator.runtimeAnimatorController = bodyAnimator.runtimeAnimatorController;
        }

        // Set sorting order (baju di depan body)
        if (outfitRenderer != null)
        {
            outfitRenderer.sortingLayerName = bodyRenderer.sortingLayerName;
            outfitRenderer.sortingOrder = bodyRenderer.sortingOrder + 1;
        }
    }

    // Sinkronkan parameter animator
    void SyncAnimation()
    {
        // Copy semua parameter dari body ke outfit
        AnimatorControllerParameter[] parameters = bodyAnimator.parameters;

        foreach (var param in parameters)
        {
            switch (param.type)
            {
                case AnimatorControllerParameterType.Float:
                    float floatValue = bodyAnimator.GetFloat(param.name);
                    outfitAnimator.SetFloat(param.name, floatValue);
                    break;

                case AnimatorControllerParameterType.Int:
                    int intValue = bodyAnimator.GetInteger(param.name);
                    outfitAnimator.SetInteger(param.name, intValue);
                    break;

                case AnimatorControllerParameterType.Bool:
                    bool boolValue = bodyAnimator.GetBool(param.name);
                    outfitAnimator.SetBool(param.name, boolValue);
                    break;

                case AnimatorControllerParameterType.Trigger:
                    if (bodyAnimator.GetBool(param.name + "Triggered"))
                    {
                        outfitAnimator.SetTrigger(param.name);
                    }
                    break;
            }
        }
    }

    // Ganti outfit by index dari list prefab
    public void ChangeOutfitByIndex(int index)
    {
        if (index < 0 || index >= outfitPrefabs.Count)
        {
            Debug.LogWarning($"Outfit index {index} tidak valid!");
            return;
        }

        ChangeOutfit(outfitPrefabs[index]);
    }

    // Next outfit
    public void NextOutfit()
    {
        int currentIndex = outfitPrefabs.IndexOf(currentOutfitObject);
        int nextIndex = (currentIndex + 1) % outfitPrefabs.Count;
        ChangeOutfitByIndex(nextIndex);
    }

    // Testing
    void LateUpdate()
    {
        // Tombol N untuk next outfit
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextOutfit();
        }

        // Angka 1-9 untuk outfit spesifik
        for (int i = 0; i < 9 && i < outfitPrefabs.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ChangeOutfitByIndex(i);
            }
        }
    }
}
