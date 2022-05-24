using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOrb.Stats
{
    [CreateAssetMenu(menuName = "The Orb/Stats/Make new progression")]
    public class Progression : ScriptableObject
    {
        [SerializeField] ProgresionCharClass[] progression;
        private Dictionary<CharClass, Dictionary<Stat, float[]>> lookupTable = null;
        Dictionary<CharClass, Dictionary<Stat, float[]>> LookupTable
        {
            get
            {
                if (lookupTable == null)
                    lookupTable = BuildLookup();
                return lookupTable;
            }
        }

        public float GetStat(Stat stat, CharClass charClass, int level)
        {
            BuildLookup();
            return LookupTable[charClass][stat][level - 1];
        }

        public int GetPenultimateLevel(Stat stat, CharClass charClass)
        {
            return LookupTable[charClass][stat].Length;
        }

        private Dictionary<CharClass, Dictionary<Stat, float[]>> BuildLookup()
        {
            return progression.ToDictionary(x => x.charClass, x => x.stats.ToDictionary(y => y.stat, y => y.levels));
        }

        [Serializable]
        class ProgresionCharClass
        {
            public CharClass charClass;
            public ProgressionStat[] stats;

        }

        [Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;

        }
    }
}