using System.Collections.Generic;
using System.Linq;

namespace TheOrb.Core
{
    public static class UIInteractionBlockers
    {
        private static readonly List<object> bag = new List<object>();

        public static void AddToBag(object blocker)
        {
            bag.Add(blocker);
        }

        public static void RemoveBlocker(object blocker)
        {
            bag.Remove(blocker);
        }

        public static bool IsBlocked()
        {
            return bag.Any();
        }
    }
}
