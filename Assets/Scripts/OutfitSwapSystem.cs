using UnityEngine;
using System.Collections.Generic;

public class OutfitSwapSystem : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer outfitRenderer;  // SpriteRenderer baju
    public Animator outfitAnimator;        // Animator baju

    [Header("Outfit Sprites")]
    public List<Sprite> idleSprites;       // Outfit idle (0, 1, 2, 3...)
    public List<Sprite> walkSprites;       // Outfit walk (0, 1, 2, 3...)
    // Tambah list lain sesuai animasi (run, jump, dll)

    [Header("Current Outfit")]
    [SerializeField] private int currentOutfitIndex = 0;

    private string currentAnimationState = "Idle";

    void Awake()
    {
        if (outfitRenderer == null)
            outfitRenderer = GetComponent<SpriteRenderer>();
        if (outfitAnimator == null)
            outfitAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        ApplyOutfit(0);  // Default outfit pertama
    }

    // Ganti outfit by index
    public void ChangeOutfit(int outfitIndex)
    {
        if (outfitIndex < 0 || outfitIndex >= idleSprites.Count)
        {
            Debug.LogWarning($"Outfit index {outfitIndex} tidak valid!");
            return;
        }

        currentOutfitIndex = outfitIndex;
        ApplyOutfit(outfitIndex);

        Debug.Log($"Outfit diganti ke: {outfitIndex}");
    }

    // Ganti outfit next/previous
    public void NextOutfit()
    {
        int next = (currentOutfitIndex + 1) % idleSprites.Count;
        ChangeOutfit(next);
    }

    public void PreviousOutfit()
    {
        int prev = currentOutfitIndex - 1;
        if (prev < 0) prev = idleSprites.Count - 1;
        ChangeOutfit(prev);
    }

    // Update sprite sesuai animasi
    public void UpdateOutfitSprite(string animationState, int frameIndex)
    {
        currentAnimationState = animationState;

        Sprite newSprite = null;

        switch (animationState)
        {
            case "Idle":
                if (frameIndex < idleSprites.Count)
                    newSprite = idleSprites[currentOutfitIndex];
                break;

            case "Walk":
                if (frameIndex < walkSprites.Count)
                    newSprite = walkSprites[currentOutfitIndex];
                break;

            // Tambah case lain sesuai animasi
        }

        if (newSprite != null)
            outfitRenderer.sprite = newSprite;
    }

    void ApplyOutfit(int index)
    {
        // Ganti sprite sesuai animasi saat ini
        if (currentAnimationState == "Idle" && index < idleSprites.Count)
            outfitRenderer.sprite = idleSprites[index];
        else if (currentAnimationState == "Walk" && index < walkSprites.Count)
            outfitRenderer.sprite = walkSprites[index];
    }

    // Testing: Tekan O untuk next outfit, P untuk previous
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
            NextOutfit();

        if (Input.GetKeyDown(KeyCode.P))
            PreviousOutfit();

        // Angka 1-9 untuk outfit spesifik
        for (int i = 0; i < 9 && i < idleSprites.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                ChangeOutfit(i);
        }
    }
}
