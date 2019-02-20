using Gamekit3D;

namespace Assets._3DGamekitLite.Scripts.Game.Enemies
{
    interface IEnemyBehavior
    {
        void ApplyDamage(Damageable.DamageMessage msg);
    }
}
