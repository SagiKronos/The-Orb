using System.Collections;
using UnityEngine;

namespace TheOrb.Combat.Skills.SkillTypes
{
    public class FallingMagicOrbs : ActiveSkill
    {
        [SerializeField] MagicBolt magicBoltPerfab;
        [SerializeField] int[] numberOfPartsEveryPeriodByLevel = new int[] { 2 };
        [SerializeField] float timePeriodBetweenLaunches;
        [SerializeField] float[] totalDurationByLevel = new float[] { 5 };
        [SerializeField] float[] radiusByLevel = new float[] { 2 };
        [SerializeField] float startingHeight = 4;
        [SerializeField] float fallingSpeed = 5;

        private void Start()
        {
            StartCoroutine(FallingMagicOrbsCoroutine());
        }

        private void Update()
        {
            foreach (var bolt in GetComponentsInChildren<MagicBolt>())
            {
                bolt.transform.position += new Vector3(0, -1, 0) * fallingSpeed * Time.deltaTime;
            }
        }

        public override void LaunchSkill(Vector3 position, int skillLevel)
        {
            var fallingMagicOrb = gameObject.GetComponent<FallingMagicOrbs>();
            var blizzard = Instantiate(fallingMagicOrb, position, Quaternion.identity);
            Destroy(blizzard.gameObject, GetModifierValueByLevel(totalDurationByLevel, skillLevel) + 1);
        }

        private IEnumerator FallingMagicOrbsCoroutine()
        {
            var skillLevel = GetSkillLevel();
            var timePassed = 0f;
            var duration = GetModifierValueByLevel(totalDurationByLevel, skillLevel);
            var numberOfPartsEveryPeriod = GetModifierValueByLevel(numberOfPartsEveryPeriodByLevel, skillLevel);
            var radius = GetModifierValueByLevel(radiusByLevel, skillLevel);

            while (timePassed < duration)
            {
                for (int i = 0; i < numberOfPartsEveryPeriod; i++)
                {
                    var magicBolt = Instantiate(magicBoltPerfab);
                    magicBolt.SetDamage(GetDamage());
                    magicBolt.transform.parent = transform;
                    magicBolt.transform.position = transform.position + new Vector3(0, startingHeight) + GetPointInRadius(radius);
                }
                yield return new WaitForSeconds(timePeriodBetweenLaunches);
                timePassed += timePeriodBetweenLaunches;
            }
        }

        private Vector3 GetPointInRadius(float radius)
        {
            var pointInCircle = Random.insideUnitCircle * radius;
            return new Vector3(pointInCircle.x, 0, pointInCircle.y);
        }

        public override object[] GetParams()
        {
            var skillLevel = GetSkillLevel();
            return new object[] { GetDamage(), GetModifierValueByLevel(totalDurationByLevel, skillLevel), GetModifierValueByLevel(radiusByLevel, skillLevel) };
        }
    }
}
