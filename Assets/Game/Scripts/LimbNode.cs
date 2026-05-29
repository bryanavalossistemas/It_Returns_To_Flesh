using UnityEngine;

public class LimbNode: MonoBehaviour
{
    public FleshLimbs.LimbType limbType;
    public FleshLimbs parentLimbsManager;
    public void ReceiveClick()
    {
        if(parentLimbsManager !=null)
        {
            parentLimbsManager.DetonateLimb(limbType);
        }
    }
}
