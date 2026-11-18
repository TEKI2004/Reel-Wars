using UnityEngine;

public interface IAttackable 
{
    string OwnerId { get; }
    int TakeDamage(int amount);
    Vector3 FeedbackPosition { get; }
}