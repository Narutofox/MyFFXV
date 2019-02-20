using System;
using System.Collections;
using System.Collections.Generic;
using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Gamekit3D
{
    public class HealthUI : MonoBehaviour
    {
        public Damageable representedDamageable;
        public GameObject healthIconPrefab;
        public GameObject magicIconPrefab;

        protected IList<Animator> m_HealthIconAnimators;

        protected readonly int m_HashActivePara = Animator.StringToHash("Active");
        protected readonly int m_HashInactiveState = Animator.StringToHash("Inactive");
        protected const float k_HeartIconAnchorWidth = 0.041f;
        protected const float magicIconPrefabWidth = 0.071f;

    
        IEnumerator Start()
        {
            if (representedDamageable == null)
                yield break;

            yield return null;

            SetHealth(representedDamageable);

            GameObject magicIcon = Instantiate(magicIconPrefab);
            magicIcon.transform.SetParent(transform);
            RectTransform magicIconRect = magicIcon.transform as RectTransform;
            magicIconRect.anchoredPosition = Vector2.zero;
            magicIconRect.sizeDelta = Vector2.zero;
            magicIconRect.anchorMin = new Vector2(magicIconPrefabWidth, 0.05f);
            magicIconRect.anchorMax = new Vector2(magicIconPrefabWidth, 0.05f);

            foreach (Image image in magicIcon.GetComponentsInChildren<Image>())
            {
                image.fillAmount = 0.1f;
            }
        }

        public void ChangeHitPointUI(Damageable damageable)
        {
            if (m_HealthIconAnimators == null)
                return;
            AddHealth(damageable);

            for (int i = 0; i < m_HealthIconAnimators.Count; i++)
            {
                m_HealthIconAnimators[i].SetBool(m_HashActivePara, damageable.currentHitPoints >= i + 1);
            }
        }

        private void SetHealth(Damageable damageable)
        {

            m_HealthIconAnimators = new List<Animator>();

            for (int i = 0; i < damageable.maxHitPoints; i++)
            {
                GameObject healthIcon = Instantiate(healthIconPrefab);
                healthIcon.transform.SetParent(transform);
                RectTransform healthIconRect = healthIcon.transform as RectTransform;
                healthIconRect.anchoredPosition = Vector2.zero;
                healthIconRect.sizeDelta = Vector2.zero;
                healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                m_HealthIconAnimators.Add(healthIcon.GetComponent<Animator>());

                if (damageable.currentHitPoints < i + 1)
                {
                    m_HealthIconAnimators[i].Play(m_HashInactiveState);
                    m_HealthIconAnimators[i].SetBool(m_HashActivePara, false);
                }
            }
        }

        private void AddHealth(Damageable damageable)
        {
            int currentNumberOfHearts = m_HealthIconAnimators.Count;

            if (currentNumberOfHearts < damageable.maxHitPoints)
            {
                for (int i = currentNumberOfHearts; i < damageable.maxHitPoints; i++)
                {
                    GameObject healthIcon = Instantiate(healthIconPrefab);
                    healthIcon.transform.SetParent(transform);
                    RectTransform healthIconRect = healthIcon.transform as RectTransform;
                    healthIconRect.anchoredPosition = Vector2.zero;
                    healthIconRect.sizeDelta = Vector2.zero;
                    healthIconRect.anchorMin += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                    healthIconRect.anchorMax += new Vector2(k_HeartIconAnchorWidth, 0f) * i;
                    m_HealthIconAnimators.Add(healthIcon.GetComponent<Animator>());
                }
            }


        }
    } 
}
