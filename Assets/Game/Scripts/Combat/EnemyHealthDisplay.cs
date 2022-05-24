using TheOrb.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;
        Text text;

        // Start is called before the first frame update
        void Awake()
        {
            fighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
            text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            var target = fighter.GetTarget();

            text.text = target != null ? string.Format("{0:0.0}/{1:0.0}", target.GetHealth(), target.GetMaxHealthPoint()) : "N/A";
        }
    }
}