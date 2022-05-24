using TheOrb.Combat.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TheOrb.Combat.Skills.SkillTree;

namespace TheOrb.UI.Skills
{
    public class SkillTreeUIBuilder : MonoBehaviour
    {
        [SerializeField] GameObject skillSlotGameObjet;
        [SerializeField] GameObject connectionPrefab;
        [SerializeField] SkillTree skillTree;
        [SerializeField] Vector3 connectorsBasePose;
        [SerializeField] UnityEvent<ActiveSkillConfig> skillRightClicked;
        private SkillTreeNode[][] tree;
        private Dictionary<SkillIds, SkillTreeNode> nodesLookup;
        private IList<(SkillTreeNode from, SkillTreeNode to)> connections;
        private GridLayoutGroup grid;
        private Dictionary<SkillIds, ActiveSkillConfig> skillsLookup;

        private void Start()
        {
            grid = GetComponent<GridLayoutGroup>();
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            BuildTreeNodes();
            DrawNodes();
            BuildTreeConnections();
            DrawConnections();
        }

        private void DrawConnections()
        {
            foreach (var item in connections)
            {
                var from = GetRelativePositionOfNode(item.from);
                var to = GetRelativePositionOfNode(item.to);
                var distance = Vector3.Distance(from, to);
                var obj = Instantiate(connectionPrefab, transform.parent);
                obj.transform.localPosition = from + connectorsBasePose;
                
                var rectTransform = obj.transform.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, distance);
                float angle;

                if (to.x == from.x)
                    angle = 180;
                else if (to.y == from.y)
                    angle = 90;
                else if (from.x > to.x)
                    angle = 180 - Mathf.Acos((from.y-to.y)/distance) * 180 / Mathf.PI;
                else
                    angle = 180 +Mathf.Acos((from.y - to.y) / distance) * 180 / Mathf.PI;

                obj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                obj.transform.SetAsFirstSibling();
            }
        }

        private Vector3 GetRelativePositionOfNode(SkillTreeNode node)
        {
            var y = transform.parent.GetComponent<RectTransform>().rect.height / 2 + ((tree.Length - 1) / (float)2 - node.Line) * (grid.spacing.y + grid.cellSize.y);
            var x = transform.parent.GetComponent<RectTransform>().rect.width / 2 + (node.Column - (tree[0].Length-1)/(float) 2) *(grid.spacing.x+grid.cellSize.x) ;
            return new Vector3(x, y,0);
        }

        private void DrawNodes()
        {
            skillsLookup = new Dictionary<SkillIds, ActiveSkillConfig>();
            foreach (var line in tree)
            {
                foreach (var cell in line)
                {
                    var skillSlot = Instantiate(skillSlotGameObjet, transform);

                    if (cell == null)
                    {
                        foreach (Transform item in skillSlot.transform)
                        {
                            item.gameObject.SetActive(false);
                        }
                        skillSlot.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        var skill = skillSlot.GetComponentInChildren<SkillSlot>();
                        skill.SetSkill(cell.SkillId);
                        skill.GetComponent<SkillSlotRightClickHandler>().rightClicked = skillRightClicked;
                        skillsLookup.Add(cell.SkillId, skill.GetSkill());
                    }
                }
            }
        }

        private void BuildTreeConnections()
        {
            connections = new List<(SkillTreeNode from, SkillTreeNode to)>();
            foreach (var item in nodesLookup)
            {
                var requirements = skillsLookup[item.Key].SkillRequirements;
                
                if (requirements == null) continue;

                foreach (var skillReq in requirements)
                {
                    connections.Add((nodesLookup[skillReq], item.Value));
                }
            }
        }

        private void BuildTreeNodes()
        {
            tree = new SkillTreeNode[5][] { new SkillTreeNode[3], new SkillTreeNode[3], new SkillTreeNode[3], new SkillTreeNode[3], new SkillTreeNode[3] };
            nodesLookup = new Dictionary<SkillIds, SkillTreeNode>();

            foreach (var item in skillTree.GetNodes())
            {
                if (tree[item.Line][item.Column] != null)
                {
                    throw new Exception("Invalid tree skill");
                }

                tree[item.Line][item.Column] = item;
                nodesLookup.Add(item.SkillId, item);
            }
        }
    }
}
