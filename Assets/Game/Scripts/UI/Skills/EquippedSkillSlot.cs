using TheOrb.Combat.Skills;
using TheOrb.Saving;
using TheOrb.UI.Dragging;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.Skills
{
    public class EquippedSkillSlot : MonoBehaviour, IDragContainer<ActiveSkillConfig>, ISaveable
    {
        [SerializeField] Image imageSlot;
        [SerializeField] Image loadingImage;
        [SerializeField] KeyCode launchKey;
        [SerializeField] SkillIds skillId;
        
        Sprite skillSprite;

        private float loadingImageHeight;
        private PlayerSkillsManager skillsManager;

        void Awake()
        {
            loadingImageHeight = loadingImage.rectTransform.rect.height;
        }

        void Start()
        {
            skillsManager = PlayerSkillsManager.GetPlayersSkillsManager();
            
            if (skillId != SkillIds.None)
            {
                skillSprite = skillsManager.GetActiveSkillConfig(skillId)?.skillSprite;
            }
        }

        void Update()
        {
            if (skillId == SkillIds.None || skillSprite == null)
            {
                imageSlot.enabled = false;
                return;
            }

            if (Input.GetKeyDown(launchKey))
            {
                SetAsActive();
            }

            imageSlot.enabled = true;
            imageSlot.sprite = skillSprite;
            var ratio = skillsManager.GetCooldownRatio(skillId);

            if (ratio < 1)
            {
                loadingImage.enabled = true;
                var loadingHeight = loadingImageHeight * (1 - ratio);
                loadingImage.rectTransform.sizeDelta = new Vector2 ( loadingImage.rectTransform.rect.width, loadingHeight);

                return;
            }

            loadingImage.enabled = false;
            
        }

        public void SetAsActive()
        {
            skillsManager.ChangeActiveSkill(skillId);
        }

        public void SetSkill(SkillIds skillId, Sprite skillSprite)
        {
            this.skillId = skillId;
            this.skillSprite = skillSprite;
        }

        public SkillIds GetSkillId()
        {
            return skillId;
        }

        object ISaveable.CaptureState()
        {
            return skillId;
        }

        void ISaveable.RestoreState(object state)
        {
            var skillConfig = PlayerSkillsManager.GetPlayersSkillsManager().GetActiveSkillConfig((SkillIds)state);
            if (skillConfig != null)
            {
                skillId = skillConfig.skill.GetId();
                skillSprite = skillConfig.skillSprite;
            }
            else
            {
                skillId = SkillIds.None;
                skillSprite = null;
            }
        }

        public int MaxAcceptable(ActiveSkillConfig item)
        {
            return int.MaxValue;
        }

        public void AddItems(ActiveSkillConfig item, int amount)
        {
            if (item == null || GetComponent<ActiveSkillSlot>()) return;

            SetSkill(item.skill.GetId(), item.skillSprite);
        }

        ActiveSkillConfig IDragSource<ActiveSkillConfig>.GetItem()
        {
            return skillsManager.GetActiveSkillConfig(skillId);
        }

        int IDragSource<ActiveSkillConfig>.GetAmount()
        {
            return 1;
        }

        void IDragSource<ActiveSkillConfig>.RemoveItems(int number)
        {
            skillId = SkillIds.None;
            skillSprite = null;
        }
    }
}
