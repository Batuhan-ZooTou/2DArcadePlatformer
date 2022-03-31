using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{   
    [SerializeField]
    private CinemachineVirtualCamera virtualCam;

    private CinemachineComponentBase componentBase;


    public StateMachina player;
    private void Awake()
    {
        componentBase = virtualCam.GetCinemachineComponent(CinemachineCore.Stage.Body);

    }
    private void Update()
    {
        if (componentBase is CinemachineFramingTransposer)
        {
            var framingTransposer = componentBase as CinemachineFramingTransposer;
            // Now we can change all its values easily.
            if (player.descend)
            {
                framingTransposer.m_LookaheadIgnoreY = false;
            }
            else
            {
                framingTransposer.m_LookaheadIgnoreY = true;

            }

        }

    }

}
