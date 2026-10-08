using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount_Pistol = 200;
    public int ammoAmount_Rifle = 200;
    public int ammoAmount_ShotGun = 200;

    //Ammo type
    public enum Ammotype
    {
        AllType,
        RifleAmmo,
        PistolAmmo,
        ShotGunAmmo
    }
    public Ammotype ammoType;
}
