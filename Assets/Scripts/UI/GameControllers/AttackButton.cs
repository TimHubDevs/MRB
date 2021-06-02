using Com.TimCorporation.Multiplayer;
using UnityEngine;

public class AttackButton : MonoBehaviour
{
    public void Fire()
    {
        PlayerManager.Instance.ProcessInputs();
    }
}
