using UnityEngine;

public interface IAttackable 
{
    string OwnerId { get; }
    bool TakeDamage(int amount);
}