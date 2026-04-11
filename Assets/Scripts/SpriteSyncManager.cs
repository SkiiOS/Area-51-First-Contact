using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script sakti versi 2 yang mendukung Offset Index (untuk rambut yang di urutan 72)
/// dan Fixed Index (untuk bayangan).
/// </summary>
public class SpriteSyncManager : MonoBehaviour
{
    [System.Serializable]
    public class SyncPart
    {
        public string partName;
        public SpriteRenderer renderer;
        public Sprite[] spritesheet; 
        
        [Tooltip("Gunakan jika index baju berbeda dengan badan. Contoh: Rambut mulai dari 72, maka isi 72.")]
        public int indexOffset = 0;

        [Tooltip("Centang jika objek ini gambarnya cuma satu dan nggak ganti-ganti (seperti bayangan).")]
        public bool isFixedIndex = false;
        public int fixedIndexValue = 0;
    }

    [Header("Referensi Utama")]
    public SpriteRenderer referenceBody;

    [Header("Bagian yang Ikut (Baju, Rambut, dll)")]
    public List<SyncPart> partsToSync = new List<SyncPart>();

    private Sprite lastSprite;
    private Dictionary<string, int> bodySpriteCache = new Dictionary<string, int>();

    void Start()
    {
        if (referenceBody == null)
            referenceBody = GetComponentInChildren<SpriteRenderer>();
    }

    void LateUpdate()
    {
        if (referenceBody == null || referenceBody.sprite == null) return;

        if (referenceBody.sprite != lastSprite)
        {
            lastSprite = referenceBody.sprite;
            SyncAllParts();
        }
    }

    void SyncAllParts()
    {
        int bodyIndex = GetSpriteIndex(referenceBody.sprite.name);
        if (bodyIndex == -1) return;

        foreach (var part in partsToSync)
        {
            if (part.renderer == null || part.spritesheet == null || part.spritesheet.Length == 0) continue;

            if (part.isFixedIndex)
            {
                // Kasus bayangan: selalu pakai urutan yang ditentukan
                if (part.fixedIndexValue < part.spritesheet.Length)
                    part.renderer.sprite = part.spritesheet[part.fixedIndexValue];
            }
            else
            {
                // Kasus Rambut/Baju: ambil index badan + offset
                int targetIndex = bodyIndex + part.indexOffset;

                if (targetIndex >= 0 && targetIndex < part.spritesheet.Length)
                {
                    part.renderer.sprite = part.spritesheet[targetIndex];
                }
            }
        }
    }

    int GetSpriteIndex(string spriteName)
    {
        // Pola Unity: "TextureName_Index"
        int lastUnderscore = spriteName.LastIndexOf('_');
        if (lastUnderscore != -1)
        {
            string indexStr = spriteName.Substring(lastUnderscore + 1);
            if (int.TryParse(indexStr, out int result))
            {
                return result;
            }
        }
        return -1; 
    }
}
