using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackingBarValues : MonoBehaviour
{
    [Header("References")]
    PlayerHackingScript hackingScript;
    [SerializeField] ProgressBar hackingBar;
    [SerializeField] GameObject barVisuals;
    [SerializeField] Text barText;

    [Header("Colours")]
    [SerializeField] Color hackingColor;
    [SerializeField] Color interruptedColor;
    [SerializeField] Color breakingColor;

    [Header("Hacking Text")]
    [SerializeField] List<string> hackingText;

    [Header("Interrupted Text")]
    [SerializeField] List<string> interruptedText;

    bool shouldSwitchText = true;

    int whatHackingText;
    int whatInterruptedText;

    private void Start()
    {
        hackingScript = GameObject.Find("Player").GetComponent<PlayerHackingScript>();
    }

    private void Update()
    {
        Check();
        if (hackingScript.hacking)
        {
            barVisuals.SetActive(true);
            UpdateBar();
        }

    }

    void UpdateBar()
    {
        if (shouldSwitchText)
        {
            whatHackingText = Random.Range(0, hackingText.Count);
            whatInterruptedText = Random.Range(0, interruptedText.Count);
            shouldSwitchText = false;
        }

        if (!hackingScript.hackInterrupted)
        {
            barText.text = hackingText[whatHackingText];
            hackingBar.color = hackingColor;
            hackingBar.current += Time.deltaTime;
        }
        if (hackingScript.hackInterrupted)
        {
            barText.text = interruptedText[whatInterruptedText];
            hackingBar.color = Color.Lerp(interruptedColor, breakingColor, Mathf.PingPong(Time.time, 0.65f));
        }
    }
    void Check()
    {
        if (!hackingScript.hacking)
        {
            hackingBar.maximum = hackingScript.tempHackingDurration;
            hackingBar.current = 0;
            shouldSwitchText = true;
            barVisuals.SetActive(false);
        }
    }
}
