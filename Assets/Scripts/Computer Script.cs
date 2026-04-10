using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportTrigger : MonoBehaviour
{
    public NPCAutoDialogue npcDialogue; // Tarik object NPC yang ada script dialognya
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // CEK APAKAH DIALOG SUDAH SELESAI?
            if (npcDialogue != null && npcDialogue.isDialogueFinished)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.Log("Selesaikan dialog dulu sebelum pergi!");
            }
        }
    }
}