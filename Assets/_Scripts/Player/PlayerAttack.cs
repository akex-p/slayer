using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float offsetFromEnemy = 1f;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private LayerMask enemyMask;

    public BeatManager beatManager;

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PerformAttack();
        }
    }

    private void PerformAttack()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out RaycastHit hit, attackRange, enemyMask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log(beatManager.CheckInputTiming());

                animator.SetTrigger("Attack");

                // calculate target-position
                Vector3 enemyPos = hit.collider.transform.position;
                Vector3 directionToEnemy = (enemyPos - transform.position).normalized;
                Vector3 moveTarget = enemyPos - directionToEnemy * offsetFromEnemy;

                // move player
                playerMovement.MoveToPosition(moveTarget);

                Debug.Log("Hit");
            }
        }
    }
}
