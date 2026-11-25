using UnityEngine;

public interface IAttackable 
{
    string OwnerId { get; }
    int TakeDamage(DamageContext context);
    Vector3 FeedbackPosition { get; }
}

