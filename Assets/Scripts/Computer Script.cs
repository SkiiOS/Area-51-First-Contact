using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTrigger : MonoBehaviour
{
    public NPCAutoDialogue npcDialogue;
    public string nextSceneName;

    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();

        // Di awal, pastikan ini jadi tembok solid (bukan trigger) 
        // agar Player tidak bisa menembus area teleportasi
        if (myCollider != null)
        {
            myCollider.isTrigger = false;
        }
    }

    private void Update()
    {
        // Terus cek status dari script NPCAutoDialogue
        if (npcDialogue != null && npcDialogue.isDialogueFinished)
        {
            // Jika dialog SELESAI, ubah jadi Trigger agar bisa dimasuki
            if (myCollider != null && !myCollider.isTrigger)
            {
                myCollider.isTrigger = true;
                Debug.Log("Pintu teleportasi sekarang terbuka!");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Karena sudah diatur di Update, saat ini terpanggil pasti isDialogueFinished sudah true
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    // Opsional: Jika player menabrak tembok saat belum selesai dialog
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !npcDialogue.isDialogueFinished)
        {
            Debug.Log("Selesaikan dialog dengan Ambarukmo dulu untuk membuka jalan!");
        }
    }
}