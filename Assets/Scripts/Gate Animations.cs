using UnityEngine;

public class GateTrigger : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "Open";
    public string targetTag = "Car"; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            animator.SetTrigger(triggerName);
        }
    }
}