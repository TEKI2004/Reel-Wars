using UnityEngine;

public interface IAttackable 
{
    int OwnerId { get; }
    bool TakeDamage(int amount);
}