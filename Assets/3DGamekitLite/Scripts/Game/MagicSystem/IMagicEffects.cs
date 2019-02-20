using System.Collections;

namespace Assets._3DGamekitLite.Scripts.Game.MagicSystem
{
    public interface IMagicEffects
    {
        IEnumerator OnBurn(float duration, int damageCount, int damageAmount);
        IEnumerator OnSlow(float duration, float slowRate);
        IEnumerator OnParalyze(float duration);
    }
}
