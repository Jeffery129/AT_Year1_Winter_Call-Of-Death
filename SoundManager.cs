using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    [Header("M1911")]
    public AudioSource Sound_1911;
    public AudioClip shootingClip1911;
    public AudioClip reloadClip1911;

    [Header("M4")]
    public AudioSource Sound_M4;
    public AudioClip shootingClipM4;
    public AudioClip reloadClipM4;

    [Header("R870")]
    public AudioSource Sound_R870;
    public AudioClip shootingClipR870;
    public AudioClip reloadClipR870;

    [Header("Empty Mag")]
    public AudioSource emptyMagazineSound;
    public AudioClip emptyMagazineClip;

    [Header("Throwables")]
    public AudioSource throwablesChannel;
    public AudioClip grenadeSoundClip;
    public AudioClip smokeSoundClip;

    [Header("ZombieSounds")]
    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayShootingSound(WeaponModel weaponType)
    {
        switch (weaponType)
        {
            case WeaponModel.M1911:
                Sound_1911.PlayOneShot(shootingClip1911);
                break;
            case WeaponModel.M4:
                Sound_M4.PlayOneShot(shootingClipM4);
                break;
            case WeaponModel.R870:
                Sound_R870.PlayOneShot(shootingClipR870);
                break;
                
        }
    }

    public void PlayReloadSound(WeaponModel weaponType)
    {
        switch (weaponType)
        {
            case WeaponModel.M1911:
                Sound_1911.PlayOneShot(reloadClip1911);
                break;
            case WeaponModel.M4:
                Sound_M4.PlayOneShot(reloadClipM4);
                break;
            case WeaponModel.R870:
                Sound_R870.PlayOneShot(reloadClipR870);
                break;
        }
    }

    public void PlayEmptyMagazineSound()
    {
        emptyMagazineSound.PlayOneShot(emptyMagazineClip);
    }
}
