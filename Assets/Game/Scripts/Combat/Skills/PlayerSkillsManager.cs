using TheOrb.Core;
using TheOrb.Saving;
using TheOrb.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace TheOrb.Combat.Skills
{
    public class PlayerSkillsManager : MonoBehaviour, IAction, ISaveable
    {
        private Dictionary<SkillIds, ActiveSkillConfig> lookup;
        [SerializeField] UnityEvent<ActiveSkillConfig> activeSkillChanged;
        ActiveSkillConfig currentActiveSkill = null;
        private bool isCanceled;
        private Vector3 launchedPosition;
        private Animator animator;
        private Dictionary<SkillIds, int> availableSkills = new Dictionary<SkillIds, int>();
        private int freeSkillPoints;

        private void Awake()
        {
            var skills = Resources.LoadAll<ActiveSkillConfig>("");
            lookup = skills.ToDictionary(x => x.skill.GetId(), x => x);

            foreach (var skill in lookup)
            {
                skill.Value.timeSinceLastActivated = int.MaxValue;
            }
        }

        public static PlayerSkillsManager GetPlayersSkillsManager()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerSkillsManager>();
        }


        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            foreach (var skill in lookup)
            {
                skill.Value.timeSinceLastActivated += Time.deltaTime;
            }
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += AddSkillPoint;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= AddSkillPoint;
        }

        public void ChangeActiveSkill(SkillIds id)
        {
            if (id == currentActiveSkill?.skill.GetId()) return;

            Cancel();
            currentActiveSkill = lookup[id];

            activeSkillChanged.Invoke(currentActiveSkill);
        }

        public bool LaunchActiveSkill(Vector3 position)
        {
            if (currentActiveSkill == null || currentActiveSkill.timeSinceLastActivated < currentActiveSkill.skill.GetCooldown())
                return false;

            GetComponent<ActionScheduler>().StartAction(this);
            isCanceled = false;
            launchedPosition = position;
            currentActiveSkill.skill.Execute();
            return true;
        }

        public void Shoot()
        {
            if (!isCanceled)
            {
                currentActiveSkill.skill.LaunchSkill(launchedPosition, GetSkillLevel(currentActiveSkill.skill.GetId()));
                currentActiveSkill.timeSinceLastActivated = 0;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }

        public float GetCooldownRatio(SkillIds skillId)
        {
            var skillConfig = lookup[skillId];
            return Mathf.Min(skillConfig.timeSinceLastActivated / skillConfig.skill.GetCooldown(), 1);
        }

        public ActiveSkillConfig GetActiveSkillConfig(SkillIds skillId)
        {
            return skillId != SkillIds.None && lookup.ContainsKey(skillId) ? lookup[skillId] : null;
        }

        public void Cancel()
        {
            isCanceled = true;
            currentActiveSkill?.skill.Cancel(gameObject);
        }

        public int GetSkillLevel(SkillIds skillId)
        {
            if (!availableSkills.ContainsKey(skillId))
                return 0;

            return availableSkills[skillId];
        }

        public bool CanBeUsed(SkillIds skillId)
        {
            return availableSkills.ContainsKey(skillId);
        }

        public bool CanBeLearned(SkillIds skillId)
        {
            if (freeSkillPoints == 0)
                return false;

            var skill = lookup[skillId];
            var areRequiremntsMet = IsLevelRequirementMet(skill) && !GetMissingSkillRequirements(skill).Any();
            return areRequiremntsMet;
        }

        public bool IsLevelRequirementMet(ActiveSkillConfig skill)
        {
            return skill.requiredLvl /*+ GetSkillLevel(skill.skill.GetId())*/ <= GetComponent<BaseStats>().GetLevel();
        }

        public SkillIds[] GetMissingSkillRequirements(ActiveSkillConfig skill)
        {
            if (skill.SkillRequirements == null)
                return new SkillIds[0];

            return skill.SkillRequirements.Where(x => !CanBeUsed(x)).ToArray();
        }

        public void AddSkillPoint(int levelDiff)
        {
            freeSkillPoints+= levelDiff;
        }

        public int GetFreeSkillPoints()
        {
            return freeSkillPoints;
        }

        public bool LearnSkill(SkillIds skillId)
        {
            if (!CanBeLearned(skillId))
                return false;

            if (!availableSkills.ContainsKey(skillId))
            {
                availableSkills.Add(skillId, 1);
            }
            else
            {
                availableSkills[skillId]++;
            }

            freeSkillPoints--;
            return true;
        }

        public object CaptureState()
        {
            return new SkillManagerSaveObject()
            {
                currentActiveSkill = currentActiveSkill == null ? SkillIds.None : currentActiveSkill.skill.GetId(),
                availableSkills = availableSkills.Keys.ToArray(),
                skillsLevels = availableSkills.Values.ToArray(),
                freeSkillPoints = freeSkillPoints
            };
        }

        public void RestoreState(object state)
        {
            var saveObj = (SkillManagerSaveObject)state;
            if (saveObj.currentActiveSkill != SkillIds.None)
                currentActiveSkill = lookup[saveObj.currentActiveSkill];

            availableSkills = new Dictionary<SkillIds, int>();
            for (int i = 0; i < saveObj.availableSkills.Length; i++)
            {
                availableSkills.Add(saveObj.availableSkills[i], saveObj.skillsLevels[i]);
            }
            freeSkillPoints = saveObj.freeSkillPoints;
        }

        [Serializable]
        public struct SkillManagerSaveObject
        {
            public SkillIds currentActiveSkill;
            public SkillIds[] availableSkills;
            public int[] skillsLevels;
            public int freeSkillPoints;
        }
    }
}
