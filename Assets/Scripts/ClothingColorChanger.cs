using UnityEngine;

public class ClothingColorChanger : MonoBehaviour
{
    [Header("Target Renderer")]
    [Tooltip("SpriteRenderer baju yang mau diubah warnanya")]
    public SpriteRenderer clothingRenderer;

    [Header("Color Presets")]
    public Color[] colorPresets = new Color[]
    {
        Color.white,           // Putih (default)
        new Color(0.8f, 0.2f, 0.2f),  // Merah
        new Color(0.2f, 0.5f, 0.8f),  // Biru
        new Color(0.2f, 0.7f, 0.3f),  // Hijau
        new Color(0.9f, 0.8f, 0.2f),  // Kuning
        new Color(0.5f, 0.2f, 0.6f),  // Ungu
        new Color(0.3f, 0.3f, 0.3f),  // Abu gelap
        new Color(0.9f, 0.6f, 0.4f),  // Orange/Peach
    };

    [Header("Current Color")]
    [SerializeField] private Color currentColor = Color.white;

    private MaterialPropertyBlock propertyBlock;
    private static readonly int ColorProperty = Shader.PropertyToID("_Color");

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();

        if (clothingRenderer == null)
            clothingRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ApplyColor(currentColor);
    }

    // Ubah warna via index preset
    public void SetColorByIndex(int index)
    {
        if (index < 0 || index >= colorPresets.Length)
        {
            Debug.LogWarning($"Color index {index} out of range!");
            return;
        }

        ApplyColor(colorPresets[index]);
    }

    // Ubah warna via Color langsung
    public void SetColor(Color color)
    {
        ApplyColor(color);
    }

    // Ubah warna via RGB (0-255)
    public void SetColorRGB(int r, int g, int b)
    {
        ApplyColor(new Color(r / 255f, g / 255f, b / 255f));
    }

    // Random color
    public void SetRandomColor()
    {
        ApplyColor(Random.ColorHSV());
    }

    void ApplyColor(Color color)
    {
        if (clothingRenderer == null)
        {
            Debug.LogWarning("Clothing Renderer belum di-assign!");
            return;
        }

        currentColor = color;

        // Gunakan MaterialPropertyBlock (performa lebih baik, tidak buat material baru)
        clothingRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor(ColorProperty, color);
        clothingRenderer.SetPropertyBlock(propertyBlock);

        Debug.Log($"Baju diubah ke warna: {color}");
    }

    // Reset ke warna putih/default
    public void ResetColor()
    {
        ApplyColor(Color.white);
    }

    // Dapatkan warna saat ini
    public Color GetCurrentColor()
    {
        return currentColor;
    }

    // Testing via input keyboard (1-8 untuk preset)
    void Update()
    {
        // Testing: Tekan angka 1-8 untuk ganti warna
        for (int i = 0; i < colorPresets.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SetColorByIndex(i);
            }
        }

        // R untuk random
        if (Input.GetKeyDown(KeyCode.R))
        {
            SetRandomColor();
        }

        // Backspace untuk reset
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ResetColor();
        }
    }
}
