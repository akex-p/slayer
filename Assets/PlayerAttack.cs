using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    [Header("References")]
    [SerializeField] private Animator animator;

    void Update()
    {
        if (Input.GetKey(attackKey))
        {
            animator.SetTrigger("Attack");
            Debug.Log("Attacked.");
        }
    }
}
