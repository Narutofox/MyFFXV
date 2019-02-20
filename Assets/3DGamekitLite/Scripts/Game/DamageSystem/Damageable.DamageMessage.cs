using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using UnityEngine;

namespace Gamekit3D
{
    public partial class Damageable : MonoBehaviour
    {
        public struct DamageMessage
        {
            public MonoBehaviour damager;
            public int amount;
            public Vector3 direction;
            public Vector3 damageSource;
            public bool throwing;

            public bool stopCamera;
            public Magic.MagicEffect? effect;
            public int magicEffectLasting;
        }
    } 
}
