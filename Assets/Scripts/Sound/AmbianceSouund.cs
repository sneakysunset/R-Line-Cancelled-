using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbianceSouund : MonoBehaviour
{
    public Transform endPos;
    public Transform playerPos;
    public float limitLoop2;
    public float limitLoop1;
    public float transition;

    private FMOD.Studio.EventInstance event_fmod;

    private void Start()
    {
        event_fmod = FMODUnity.RuntimeManager.CreateInstance("event:/Ambiance");
        event_fmod.start();
    }
    // Update is called once per frame
    void Update()
    {
        //Loop0
        if (GetDistPlayerToEnd(playerPos.position, endPos.position) > limitLoop1)
        {
            transition = 0;
            event_fmod.setParameterByName("Transition", transition);
        }
        else if(limitLoop1 > GetDistPlayerToEnd(playerPos.position, endPos.position) && limitLoop2 < GetDistPlayerToEnd(playerPos.position, endPos.position))
        {
            transition = 1;
            event_fmod.setParameterByName("Transition", transition);
        }
        else if(GetDistPlayerToEnd(playerPos.position, endPos.position) < limitLoop2)
        {
            transition = 2;
            event_fmod.setParameterByName("Transition", transition);
        }
    }
    public float GetDistPlayerToEnd(Vector3 player, Vector3 end)
    {
        return Vector3.Distance(player, end);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(endPos.position, limitLoop2);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(endPos.position, limitLoop1);


    }
}
