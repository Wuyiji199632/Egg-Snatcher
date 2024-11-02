using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerDetection))]
public class PlayerFill : MonoBehaviour
{

    [Header("Elements")]
    [SerializeField] private Renderer[] renderers;
    private PlayerDetection playerDetection;

    [Header("Settings")]
    [SerializeField] private float fillAmount = 0.0f;
    const string fillAmountRef = "Fill_Amount";

    private void Awake()
    {
        playerDetection = GetComponent<PlayerDetection>();
    }

    private void Start()
    {
        fillAmount = 1;
        UpdateRenderers();
    }

    public bool UpdateFill(float fillStep)
    {
        Debug.Log("Fill amount: " + fillStep);

        if (playerDetection.IsHoldingEgg)
        {
            fillAmount += fillStep; 
        }
        else
        {
            fillAmount -= fillStep;
        }
        Mathf.Clamp01(fillAmount);
        UpdateRenderers();

        return fillAmount <= 0;
    }

    private void UpdateRenderers()
    {
        foreach(Renderer renderer in renderers)
        {
            renderer.material.SetFloat(fillAmountRef, fillAmount);
        }
    }
}
