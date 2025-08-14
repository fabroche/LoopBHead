using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject UiAttackInfoControls;
    public GameObject mainPlayer;
    
    private ButtHeadController buttHeadController;
    void Start()
    {
        buttHeadController = mainPlayer.GetComponent<ButtHeadController>();
    }

    // Update is called once per frame
    void Update()
    {
        UiAttackInfoControls.SetActive(buttHeadController.haveWeapon);
    }
}