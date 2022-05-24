using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TheOrb.Core;
using UnityEngine;

namespace TheOrb.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] int startLevel = 1;

        [SerializeField] CharClass charClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject levelUpEffect;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action<int> onLevelUp;
        LazyValue<int> currentLvl;
        private Experience exp;

        private void Awake()
        {
            exp = GetComponent<Experience>();
            currentLvl = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLvl.ForceInit();
        }

        private void OnEnable()
        {
            if (exp != null)
            {
                exp.onExpGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (exp != null)
            {
                exp.onExpGained -= UpdateLevel;
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, charClass, GetLevel());
        }

        private float GetAdditiveModifier(Stat stat)
        {
            return shouldUseModifiers ? GetComponents<IModifierProvider>().SelectMany(x => x.GetAdditiveModifiers(stat)).Sum() : 0;
        }
        private float GetPercentageModifier(Stat stat)
        {
            return shouldUseModifiers ? GetComponents<IModifierProvider>().SelectMany(x => x.GetPercentageModifiers(stat)).Sum() : 0;
        }


        public int GetLevel()
        {
            return currentLvl.value;
        }

        public float GetExperienceFraction()
        {
            var formerLevelReq = currentLvl.value == 1 ? 0 : progression.GetStat(Stat.ExpRequiredForLvlUp, charClass, currentLvl.value-1);
            var currentLevelReq = progression.GetStat(Stat.ExpRequiredForLvlUp, charClass, currentLvl.value);
            return (exp == null ? 0 : exp.ExpPoints - formerLevelReq) / (currentLevelReq - formerLevelReq);
        }

        private int CalculateLevel()
        {
            if (exp == null)
                return startLevel;

            var currExp = exp.ExpPoints;
            var penultimateLevel = progression.GetPenultimateLevel(Stat.ExpRequiredForLvlUp, charClass);

            for (int level = 1; level <= penultimateLevel; level++)
            {
                var xpForNextLvl = progression.GetStat(Stat.ExpRequiredForLvlUp, charClass, level);
                if (xpForNextLvl > currExp)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }
        private void UpdateLevel()
        {
            var newLvl = CalculateLevel();
            if (newLvl > currentLvl.value)
            {
                var lvlDiff = newLvl - currentLvl.value;
                currentLvl.value = newLvl;
                LevelUpEffect();
                onLevelUp(lvlDiff);
            }
        }

        private void LevelUpEffect()
        {
            if (levelUpEffect)
                Instantiate(levelUpEffect, transform);
        }
    }
}