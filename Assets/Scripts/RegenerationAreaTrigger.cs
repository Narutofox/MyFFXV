using Gamekit3D;
using UnityEngine;

public class RegenerationAreaTrigger : MonoBehaviour
{
    private MagicUI _magicUi;
    private float _originalInvulnerabiltyTime;


    void OnTriggerEnter(Collider other)
    {
        RegenerationArea regenerationArea = other.gameObject.GetComponent<RegenerationArea>();
        if (regenerationArea != null)
        {
            PlayerController player = gameObject.GetComponent<PlayerController>();
            Damageable damageable = gameObject.GetComponent<Damageable>();
            if (player != null && damageable != null)
            {
                if (_magicUi == null)
                {
                    _magicUi = FindObjectOfType<MagicUI>();
                }

                if (regenerationArea.CanRegenerate)
                {
                    _originalInvulnerabiltyTime = damageable.invulnerabiltyTime;
                    regenerationArea.Regenerate(damageable);
                }

                _magicUi.canUseMagic = false;
                player.canAttack = false;
                damageable.isInvulnerable = true;
            }
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "RegenerationArea")
        {
            PlayerController player = gameObject.GetComponent<PlayerController>();
            Damageable damageable = gameObject.GetComponent<Damageable>();
            if (player != null && damageable != null)
            {
                if (_magicUi == null)
                {
                    _magicUi = FindObjectOfType<MagicUI>();
                }

                damageable.invulnerabiltyTime = _originalInvulnerabiltyTime;
                _magicUi.canUseMagic = true;
                player.canAttack = true;
                damageable.isInvulnerable = false;
            }
        }
    }
}
