using Assets._3DGamekitLite.Scripts.Game.TargetSystem;
using Gamekit3D;
using UnityEngine;

namespace Assets._3DGamekitLite.Scripts.Game.MagicSystem
{
    class MagicAttack : MonoBehaviour
    {
        public GameObject Fire;
        public GameObject Lightning;
        public GameObject Blizzard;

        internal TargetController TargetController;
        internal MagicUI MagicUi;
        void Start()
        {
            TargetController = FindObjectOfType<TargetController>();
            MagicUi = FindObjectOfType<MagicUI>();
        }

        public void CastMagic()
        {
            if (MagicUi ==  null)
            {
                MagicUi = FindObjectOfType<MagicUI>();
            }

            EnemyController target = TargetController._target;
            if (target != null)
            {
                Vector3 position = new Vector3(target.transform.position.x, target.transform.position.y + 10, target.transform.position.z);
                
                switch (MagicUi.GetCurrentMagic())
                {
                    case Magic.MagicType.Fire:
                    {
                        Instantiate(Fire, position, Fire.transform.rotation);
                        break;
                    }
                       
                    case Magic.MagicType.Blizzard:
                    {
                        Instantiate(Blizzard, position, Blizzard.transform.rotation);
                        break;
                    }
                    case Magic.MagicType.Lightning:
                    {
                        Instantiate(Lightning, position, Lightning.transform.rotation);
                        break;
                    }                   
                }
                MagicUi.DecreaseMagic();
            }
        }

        internal bool HasMagic()
        {
            if (MagicUi == null)
            {
                MagicUi = FindObjectOfType<MagicUI>();
            }

            return MagicUi.HasMagic();
        }
    }
}
