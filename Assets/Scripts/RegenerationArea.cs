using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets._3DGamekitLite.Scripts.Game.MagicSystem;
using Gamekit3D;
using UnityEngine;

public class RegenerationArea : MonoBehaviour
{
    private MagicUI _magicUi;
    private WaitForSeconds _regenerationWait;
    private float _cooldown;
    public bool CanRegenerate { get; private set; }
    void Start()
    {
        CanRegenerate = true;
        _cooldown = 900f;// 15 min
        _regenerationWait = new WaitForSeconds(_cooldown);
    }

    public void Regenerate(Damageable damageable)
    {       
        if (CanRegenerate)
        {
            if (_magicUi == null)
            {
                _magicUi = FindObjectOfType<MagicUI>();
            }

            if (damageable.currentHitPoints < damageable.maxHitPoints)
            {
                damageable.Heal(0,1);
                damageable.invulnerabiltyTime = _cooldown;
            }

            if (Random.Range(0, 3) == 1)
            {
                damageable.Heal(1, 0);
            }

            IList<Magic.MagicType> magicList = _magicUi.GetMagicsThatCanBeIncreased();
            if (magicList.Any())
            {
                int index = Random.Range(0, magicList.Count);
                _magicUi.IncreaseMagic(magicList[index]);
            }

            StartCoroutine(RegenerationWaitCorutine());
        }
    }
    IEnumerator RegenerationWaitCorutine()
    {
        CanRegenerate = false;

        yield return _regenerationWait;

        CanRegenerate = true;
    }
}
