using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayFPS : MonoBehaviour
{
    [SerializeField] 
    [Tooltip ("How often does the FPS counter update")]
    private float updateTimer;
    private float realFpsCount;
    private int displayFpsCount;
    private Text fpsText;
    private bool counterUpdated = true;

    private void Start() 
    {
        fpsText = GetComponent<Text>();
    }

    void Update()
    {
        realFpsCount = 1/Time.smoothDeltaTime;
        
        
        if(counterUpdated)
        {
            counterUpdated = false;
            StartCoroutine("UpdateCounter");
        }
    }

    private IEnumerator UpdateCounter()
    {
        yield return new WaitForSeconds(updateTimer);
        displayFpsCount = (int) realFpsCount;
        fpsText.text = displayFpsCount.ToString();
        counterUpdated = true;
    }
}
