using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] public float minimum = 0;
    [SerializeField] public float maximum;
    [SerializeField] public float current;
    [SerializeField] Image mask;

    private void Update()
    {
        //GetCurrentFill();
    }

    
    void GetCurrentFill()
    {
        float currentOffset = current - minimum;
        float maximumOffset = maximum - minimum;
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;
    }
}
