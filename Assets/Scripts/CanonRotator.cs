using UnityEngine;

public class CannonRotator : MonoBehaviour
{
    public Transform cannonPivot;
    public float rotationSpeed = 5f;
    public Vector3 forwardOffset = new Vector3(-90f, 0f, 0f); // keep your original model alignment

    private CannonTowerAttack towerAttack;

    void Start()
    {
        towerAttack = GetComponentInParent<CannonTowerAttack>();
        if (cannonPivot == null)
            cannonPivot = transform;
    }

    void Update()
    {
        if (towerAttack == null || towerAttack.currentTarget == null)
            return;

        // Direction to target
        Vector3 direction = towerAttack.currentTarget.transform.position - cannonPivot.position;

        // Only rotate horizontally
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f) return;

        // Compute rotation toward target
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Apply your forward offset
        targetRotation *= Quaternion.Euler(forwardOffset);

        // Prevent flipping by keeping original X and Z rotation
        Vector3 euler = targetRotation.eulerAngles;
        euler.x = forwardOffset.x;
        euler.z = forwardOffset.z;
        targetRotation = Quaternion.Euler(euler);

        // Smooth rotation
        cannonPivot.rotation = Quaternion.Slerp(
            cannonPivot.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
