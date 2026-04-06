using UnityEngine;
using System.Collections.Generic;

public class OutfitFrameSync : MonoBehaviour
{
    [Header("Body Reference")]
    public SpriteRenderer bodyRenderer;
    public Animator bodyAnimator;

    [Header("Outfit")]
    public SpriteRenderer outfitRenderer;
    public Animator outfitAnimator;

    [Header("Frame Data")]
    [Tooltip("Total frame dalam 1 animasi (contoh: 4 frame idle)")]
    public int totalFrames = 4;

    [Tooltip("Frame rate animasi (default: 12 fps)")]
    public float frameRate = 12f;

    [Header("Outfit Sprites")]
    public List<Sprite> outfitSprites;  // Semua 23 frame outfit

    // Private variables
    private AnimatorStateInfo currentState;
    private float normalizedTime;
    private int currentFrame;

    // Frame offsets untuk tiap animasi
    private Dictionary<string, int> animationFrameOffsets = new Dictionary<string, int>();

    void Awake()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponent<SpriteRenderer>();
        if (bodyAnimator == null)
            bodyAnimator = GetComponent<Animator>();

        // Setup offsets (sesuaikan dengan sprite sheet kamu)
        // Contoh: Idle = frame 0-3, Walk = frame 4-7, etc.
        animationFrameOffsets.Add("Idle", 0);
        animationFrameOffsets.Add("Walk", 4);
        animationFrameOffsets.Add("Run", 8);
        animationFrameOffsets.Add("Jump", 12);
        // Tambahin sesuai animasi kamu...
    }

    void Update()
    {
        if (bodyAnimator == null || outfitRenderer == null) return;

        // Get current animation state
        currentState = bodyAnimator.GetCurrentAnimatorStateInfo(0);
        normalizedTime = currentState.normalizedTime;

        // Calculate current frame
        int totalAnimFrames = GetFrameCountForState(currentState);
        currentFrame = Mathf.FloorToInt(normalizedTime * totalAnimFrames) % totalAnimFrames;

        // Get outfit sprite index
        int outfitIndex = GetOutfitFrameIndex(currentState, currentFrame);

        // Apply sprite
        if (outfitIndex >= 0 && outfitIndex < outfitSprites.Count)
        {
            outfitRenderer.sprite = outfitSprites[outfitIndex];
        }

        // Sync flip
        outfitRenderer.flipX = bodyRenderer.flipX;
        outfitRenderer.flipY = bodyRenderer.flipY;
    }

    int GetFrameCountForState(AnimatorStateInfo state)
    {
        // Default frame count, bisa diubah sesuai animasi
        return totalFrames;
    }

    int GetOutfitFrameIndex(AnimatorStateInfo state, int frame)
    {
        string stateName = GetStateName(state);

        int offset = 0;
        if (animationFrameOffsets.ContainsKey(stateName))
        {
            offset = animationFrameOffsets[stateName];
        }

        return offset + frame;
    }

    string GetStateName(AnimatorStateInfo state)
    {
        // Mendapatkan nama state dari hash
        // Ini simplified, bisa diperbaiki dengan AnimatorController
        if (state.IsName("Idle") || state.IsTag("Idle")) return "Idle";
        if (state.IsName("Walk") || state.IsTag("Walk")) return "Walk";
        if (state.IsName("Run") || state.IsTag("Run")) return "Run";
        if (state.IsName("Jump") || state.IsTag("Jump")) return "Jump";

        return "Idle"; // Default
    }

    // Testing: Ganti outfit
    public void ChangeOutfit(List<Sprite> newOutfitSprites)
    {
        outfitSprites = newOutfitSprites;
    }

    // Debug
    void OnGUI()
    {
        if (Application.isEditor)
        {
            GUILayout.Label($"State: {GetStateName(currentState)}");
            GUILayout.Label($"Frame: {currentFrame} / {totalFrames}");
            GUILayout.Label($"Normalized Time: {normalizedTime:F2}");
        }
    }
}
