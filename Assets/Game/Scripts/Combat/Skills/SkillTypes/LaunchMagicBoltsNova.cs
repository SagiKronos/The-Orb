using UnityEngine;

namespace TheOrb.Combat.Skills.SkillTypes
{
    public class LaunchMagicBoltsNova : ActiveSkill
    {
        [SerializeField] int[] amountOfBoltsByLevel = new int[] { 1 };
        [SerializeField] MagicBolt magicBolt;
        [SerializeField] float distance;
        [SerializeField] float timeToLive;
        [SerializeField] Vector3 positionModifier;
        [SerializeField] float angle = 0;

        void Update()
        {
            foreach (var bolt in transform.GetComponentsInChildren<MagicBolt>())
            {
                bolt.transform.position += bolt.transform.forward * distance / timeToLive * Time.deltaTime;
            }
        }

        public override void LaunchSkill(Vector3 position, int skillLevel)
        {
            var nova = Instantiate(gameObject);
            CreateNovaBolts(nova, skillLevel);
            Destroy(nova, timeToLive);
        }

        private void CreateNovaBolts(GameObject nova, int skillLevel)
        {
            var amountOfBolts = GetModifierValueByLevel(amountOfBoltsByLevel, skillLevel);
            for (int i = 0; i < amountOfBolts; i++)
            {
                var bolt = Instantiate(magicBolt, launcher.value.gameObject.transform.position + positionModifier, launcher.value.gameObject.transform.rotation, nova.transform);
                bolt.SetDamage(GetDamage());
                bolt.gameObject.transform.Rotate(0, angle / amountOfBolts * i -angle/ amountOfBolts, 0);
            }
        }

        public override object[] GetParams()
        {
            return new object[] { GetDamage(), GetModifierValueByLevel(amountOfBoltsByLevel, GetSkillLevel()) };
        }
    }
}