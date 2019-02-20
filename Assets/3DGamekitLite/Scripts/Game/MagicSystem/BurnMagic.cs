using Gamekit3D;
using UnityEngine;

namespace Assets._3DGamekitLite.Scripts.Game.MagicSystem
{
    class BurnMagic : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            Damageable d = collision.collider.GetComponentInChildren<Damageable>();

            if (d != null && !d.isInvulnerable)
            {
                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damageSource = transform.position,
                    damager = this,
                    amount = 1,
                    direction = (collision.collider.transform.position - transform.position).normalized,
                    effect = Magic.MagicEffect.Burn,
                    magicEffectLasting = 5
                };

                d.ApplyDamage(message);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && !d.isInvulnerable)
            {
                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damageSource = transform.position,
                    damager = this,
                    amount = 1,
                    direction = (other.transform.position - transform.position).normalized,
                    throwing = false,
                    effect = Magic.MagicEffect.Burn,
                    magicEffectLasting = 5
                };

                d.ApplyDamage(message);
            }
        }

        void OnParticleCollision(GameObject other)
        {
            Damageable d = other.GetComponentInChildren<Damageable>();

            if (d != null && !d.isInvulnerable)
            {
                Damageable.DamageMessage message = new Damageable.DamageMessage
                {
                    damageSource = transform.position,
                    damager = this,
                    amount = 1,
                    direction = (other.transform.position - transform.position).normalized,
                    throwing = false,
                    effect = Magic.MagicEffect.Burn,
                    magicEffectLasting = 5
                };

                d.ApplyDamage(message);
            }
        }
    }
}
