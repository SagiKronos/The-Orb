using TheOrb.Core;
using TheOrb.Stats;
using UnityEngine;

namespace TheOrb.Combat.Skills
{
    public abstract class ActiveSkill : MonoBehaviour
    {

        [SerializeField] protected float[] damagePerecentageModifierByLevel = new float[] { 100 };
        [SerializeField] protected float[] damageAdditionModifierByLevel = new float[] { 0 };
        [SerializeField] protected SkillIds id;

        [SerializeField] float cooldown = 5;
        private PlayerSkillsManager _launcher;
        protected LazyValue<PlayerSkillsManager> launcher = new LazyValue<PlayerSkillsManager>(SetupLauncher);

        private static PlayerSkillsManager SetupLauncher()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkillsManager>();
        }

        public void Execute()
        {
            launcher.value.GetComponentInChildren<Animator>().ResetTrigger("stopAttack");
            launcher.value.GetComponentInChildren<Animator>().SetTrigger("useSkill");
        }

        public void Cancel(GameObject launcher)
        {
            launcher.GetComponentInChildren<Animator>().ResetTrigger("useSkill");
            launcher.GetComponentInChildren<Animator>().SetTrigger("stopAttack");
        }

        public abstract void LaunchSkill(Vector3 launchedPosition, int skillLevel);
        public abstract object[] GetParams();

        public float GetCooldown()
        {
            return cooldown;
        }

        public SkillIds GetId()
        {
            return id;
        }

        protected float GetDamage()
        {
            int skillLvl = GetSkillLevel(launcher.value);
            var damagePercentageModifier = GetModifierValueByLevel(damagePerecentageModifierByLevel, skillLvl);
            var damageAdditionModifier = GetModifierValueByLevel(damageAdditionModifierByLevel, skillLvl);
            var damage = (launcher.value.GetComponent<BaseStats>().GetStat(Stat.Damage) + damageAdditionModifier) * damagePercentageModifier / 100;
            return damage;
        }

        protected int GetSkillLevel()
        {
            return GetSkillLevel(launcher.value);
        }

        private int GetSkillLevel(PlayerSkillsManager player)
        {
            return player.GetComponent<PlayerSkillsManager>().GetSkillLevel(id);
        }

        protected T GetModifierValueByLevel<T>(T[] modifiers, int level)
        {
            if (level == 0)
                return modifiers[0];

            if (level >= modifiers.Length)
                return modifiers[modifiers.Length - 1];

            return modifiers[level - 1];
        }
    }
}
