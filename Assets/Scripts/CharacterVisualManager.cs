using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Script All-in-One untuk mengatur tampilan karakter multi-sprite.
/// Menangani Layering (Sorting Order) dan Sinkronisasi Animasi (Blend Tree).
/// </summary>
public class CharacterVisualManager : MonoBehaviour
{
    [Header("Animator Pusat")]
    public Animator rootAnimator;

    [Header("Bagian Visual (Tarik Objek ke Sini)")]
    public SpriteRenderer body;
    public SpriteRenderer clothes;
    public SpriteRenderer hair;
    public SpriteRenderer shadow;

    [Header("Konfigurasi")]
    public bool autoSyncAnimations = true;
    public bool autoSetSortingOrder = true;

    private Animator[] childAnimators;

    void Awake()
    {
        // Jika rootAnimator kosong, cari di object ini sendiri
        if (rootAnimator == null) 
            rootAnimator = GetComponent<Animator>();
        
        // Secara otomatis mencari animator pada bagian yang dimasukkan
        RefreshVisualParts();
        
        // Atur urutan layer agar tidak tumpang tindih
        if (autoSetSortingOrder) 
            SetupSortingOrders();
    }

    /// <summary>
    /// Panggil fungsi ini jika kamu mengganti baju/rambut di tengah game lewat script.
    /// </summary>
    public void RefreshVisualParts()
    {
        List<Animator> anims = new List<Animator>();
        
        if (body != null && body.GetComponent<Animator>()) anims.Add(body.GetComponent<Animator>());
        if (clothes != null && clothes.GetComponent<Animator>()) anims.Add(clothes.GetComponent<Animator>());
        if (hair != null && hair.GetComponent<Animator>()) anims.Add(hair.GetComponent<Animator>());
        
        childAnimators = anims.ToArray();
    }

    void SetupSortingOrders()
    {
        // Standar urutan: Shadow paling belakang, Hair paling depan
        if (shadow != null) shadow.sortingOrder = -1;
        if (body != null) body.sortingOrder = 0;
        if (clothes != null) clothes.sortingOrder = 1;
        if (hair != null) hair.sortingOrder = 2;
        
        // Pastikan shadow tidak ikut ter-flip secara aneh jika tidak perlu (opsional)
    }

    void LateUpdate()
    {
        // Sinkronkan semua parameter (MoveX, MoveY, Speed, dll) setiap frame
        if (autoSyncAnimations && rootAnimator != null && childAnimators != null)
        {
            SyncParameters();
        }
    }

    void SyncParameters()
    {
        foreach (var child in childAnimators)
        {
            if (child == null || !child.gameObject.activeInHierarchy) continue;

            foreach (var param in rootAnimator.parameters)
            {
                switch (param.type)
                {
                    case AnimatorControllerParameterType.Float:
                        child.SetFloat(param.nameHash, rootAnimator.GetFloat(param.nameHash));
                        break;
                    case AnimatorControllerParameterType.Int:
                        child.SetInteger(param.nameHash, rootAnimator.GetInteger(param.nameHash));
                        break;
                    case AnimatorControllerParameterType.Bool:
                        child.SetBool(param.nameHash, rootAnimator.GetBool(param.nameHash));
                        break;
                }
            }
        }
    }

    // Membantu pengecekan di Editor
    void OnValidate()
    {
        if (autoSetSortingOrder) SetupSortingOrders();
    }
}
