using UnityEngine;

namespace TheOrb.Sound
{
    public class CharacterSoundsPlayer : MonoBehaviour
    {
        [SerializeField] AudioClip[] deathSounds;
        [SerializeField] AudioClip[] damageTakenSounds;
        [SerializeField] AudioSource source;

        public void PlayDeathSound()
        {
            PlaySoundForType(deathSounds);
        }

        public void PlayTakeDamageSound()
        {
            PlaySoundForType(damageTakenSounds);
        }

        public void PlaySoundForType(AudioClip[] clips)
        {
            var clipNum = Random.Range(0, clips.Length);
            source.clip = clips[clipNum];
            source.Play();
        }
    }
}
