namespace TanksOnAPlain.Unity.Components.Physics
{
    public class DamageInflictedEventArgs
    {
        public float Damage { get; }

        public DamageInflictedEventArgs(float damage)
        {
            Damage = damage;
        }
    }
}