using UnityEngine;

namespace TheOrb.Combat.Skills.SkillTree
{
    [CreateAssetMenu(menuName = ("The Orb/Skills/Make new skill tree"))]
    public class SkillTree : ScriptableObject
    {
        [SerializeField] SkillTreeNode[] skillTreeNodes;

        public SkillTreeNode[] GetNodes()
        {
            return skillTreeNodes;
        }
    }
}
