using System;

namespace Assets._3DGamekitLite.Scripts.Game.MagicSystem
{
    public class Magic
    {
        [Flags]
        public enum MagicType
        {
            Fire = 0x01,
            Blizzard = 0x02,
            Lightning = 0x04
        }

        public enum MagicEffect
        {
            Burn = 0x05,
            Slow = 0x06,
            Paralyze = 0x04
        }



    }
}
