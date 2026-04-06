using UnityEngine;

public class OutfitSyncFixed : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer bodyRenderer;  // Reference ke body player
    public SpriteRenderer outfitRenderer; // Outfit sendiri

    [Header("Position Offset")]
    public Vector3 offset = Vector3.zero;  // Offset dari body (biasanya 0)

    private Transform bodyTransform;

    void Awake()
    {
        // Auto-find references
        if (bodyRenderer == null)
        {
            // Cari di parent (asumsi outfit adalah child dari player)
            bodyRenderer = GetComponentInParent<SpriteRenderer>();
        }

        if (outfitRenderer == null)
        {
            outfitRenderer = GetComponent<SpriteRenderer>();
        }

        if (bodyRenderer != null)
        {
            bodyTransform = bodyRenderer.transform;
        }
    }

    void LateUpdate()
    {
        // Sync di LateUpdate untuk menghindari jitter
        SyncPosition();
        SyncFlip();
        SyncSorting();
    }

    void SyncPosition()
    {
        if (bodyTransform == null) return;

        // Ikut posisi body + offset
        transform.position = bodyTransform.position + offset;
    }

    void SyncFlip()
    {
        if (bodyRenderer == null || outfitRenderer == null) return;

        // Sync flipX
        outfitRenderer.flipX = bodyRenderer.flipX;
        outfitRenderer.flipY = bodyRenderer.flipY;
    }

    void SyncSorting()
    {
        if (bodyRenderer == null || outfitRenderer == null) return;

        // Outfit selalu di depan body
        outfitRenderer.sortingLayerName = bodyRenderer.sortingLayerName;
        if (outfitRenderer.sortingOrder <= bodyRenderer.sortingOrder)
        {
            outfitRenderer.sortingOrder = bodyRenderer.sortingOrder + 1;
        }
    }
}
