public interface IAttackable 
{
    string OwnerId { get; }
    void TakeDamage(int amount);
}