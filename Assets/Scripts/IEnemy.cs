using UnityEngine;

public interface IEnemy
{
    float CurrentHealth { get; }
    void TakeDamage(float amount);
    float Progress { get; }
    Transform Transform { get; } // For towers to get position easily
}
