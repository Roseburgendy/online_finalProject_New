using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header(" SO_GameplayAudio Prefab")]
    [SerializeField] private AudioClipRefsSO audioClips;

    private AudioSource sfxSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
    }

    public void PlayChop()
    {
        PlayRandom(audioClips.chop);
    }
    public void PlayDeliveryFail()
    {
        PlayRandom(audioClips.deliveryFail);
    }
    public void PlayDeliverySuccess()
    {
        PlayRandom(audioClips.deliverySuccess);
    }
    public void PlayFootstep()
    {
        PlayRandom(audioClips.footstep);
    }
    public void PlayObjectDrop()
    {
        PlayRandom(audioClips.objectDrop);
    }
    public void PlayObjectPickup()
    {
        PlayRandom(audioClips.objectPickup);
    }
    public void PlayStoveSizzle()
    {
        if (audioClips.stoveSizzle != null)
            sfxSource.PlayOneShot(audioClips.stoveSizzle);
    }
    public void PlayTrash()
    {
        PlayRandom(audioClips.trash);
    }
    public void PlayWarning()
    {
        PlayRandom(audioClips.warning);
    }

    public void PlayUIClick()
    {
        PlayRandom(audioClips.uiClick);
    }

    private void PlayRandom(AudioClip[] arr)
    {
        if (arr == null || arr.Length == 0) return;
        var clip = arr[Random.Range(0, arr.Length)];
        sfxSource.PlayOneShot(clip);
    }
}
