using UnityEngine;

[CreateAssetMenu(fileName = "GunSound", menuName = "GunSO/Sound")]
public class GunSoundSO : ScriptableObject
{
    [SerializeField] private SoundController _shotAudioPrefab;

    public void PlayShotSound(Vector3 position, float volume = 1)
    {
        var sound = ObjectPooler.ProvideObject(_shotAudioPrefab, position, Quaternion.identity);
        
        if(sound is SoundController controller)
        {
            controller.Play();
        }
    }
}
