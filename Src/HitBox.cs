using UnityEngine;


public class HitBox : MonoBehaviour
{
    public enum HitPart
    {
        Head,
        Body
    }

    public HitPart hitPart;
    public Enemy enemy;
}
