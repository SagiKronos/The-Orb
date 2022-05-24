using System;
using UnityEngine;

namespace TheOrb.Combat.Skills.SkillTree
{
    [Serializable]
    public class SkillTreeNode
    {
        [SerializeField] public SkillIds SkillId;
        [SerializeField] public int Line;
        [SerializeField] public int Column;
    }
}
