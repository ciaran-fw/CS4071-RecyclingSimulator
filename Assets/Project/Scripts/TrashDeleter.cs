using UnityEngine;

public class TrashDeleter : MonoBehaviour
{
    [SerializeField] private string[] tagsToDelete;

    private void OnTriggerEnter(Collider other)
    {
        foreach (string tag in tagsToDelete)
        {
            if (other.CompareTag(tag))
            {
                Destroy(other.gameObject);
            }
        }
    }
}
