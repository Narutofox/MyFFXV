using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = System.Random;

namespace Assets._3DGamekitLite.Scripts.Game.Graphics
{
    class DragonDissolve : MonoBehaviour
    {
        public float minStartTime = 2f;
        public float maxStartTime = 6f;
        public float dissolveTime = 3f;
        ParticleSystem m_ParticleSystem;
        float m_StartTime;
        float m_EndTime;

        void Awake()
        {
            m_ParticleSystem = GetComponentInChildren<ParticleSystem>();
            m_StartTime = Time.time + UnityEngine.Random.Range(minStartTime, maxStartTime);
            m_EndTime = m_StartTime + dissolveTime + m_ParticleSystem.main.startLifetime.constant;
        }

        void Update()
        {
            if (Time.time >= m_EndTime)
            {
                Destroy(gameObject);
            }
        }
    }
}
