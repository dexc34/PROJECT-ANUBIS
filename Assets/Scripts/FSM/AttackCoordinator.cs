using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackCoordinator : MonoBehaviour
{
    [Header("# of Tokens")]
    [Tooltip("How many ranged enemies can be attacking at once")]
    [SerializeField] float rangedTokens = 2;

    //a list of all attack priority scripts current in play
    [HideInInspector] List<AttackPriority> backlog = new List<AttackPriority>();
    //a list of what enemies have a token
    [HideInInspector] List<AttackPriority> listOfAttackers = new List<AttackPriority>();
    [HideInInspector] List<float> attackerNumber = new List<float>();

    private void Update()
    {
        CheckToken();
    }

    public void AddToBackLog(AttackPriority ap)
    {
        backlog.Add(ap);
    }

    //what currently owns the token while checking
    int whatHasToken = 0;

    //checks to see what AP has the lowest score, thus taking priority 
    public void CheckToken()
    {
        float lowestValue = 10000000;
        float currentValue;
        for (int i = 0; i < rangedTokens; i++)
        {
            if (listOfAttackers.Count > rangedTokens - 1)
            {
                RemoveToken();
            }
            for (int x = 0; x < backlog.Count; x++)
            {
                currentValue = backlog[x].attackPriority;
                if (currentValue < lowestValue)
                {
                    lowestValue = backlog[x].attackPriority;

                    //sets the previous AP to false 
                    //listOfAttackers[i]
                    //backlog[whatHasToken].hasToken = false;
                    backlog[whatHasToken].hasBeenUsed = false;

                    whatHasToken = x;
                    listOfAttackers.Insert(i, backlog[x]);
                    attackerNumber.Insert(i, x);

                    //sets the current AP to "be used" so its not reused for the next token 
                    backlog[x].hasBeenUsed = true;
                }
            }
            backlog[whatHasToken].hasToken = true;
        }
        //resets the has been used variable 
        for (int x = 0; x < backlog.Count; x++)
        {
            backlog[x].hasBeenUsed = false;
        }

    }

    public void RemoveToken()
    {
        float maxValue = 0;
        float currentValue;
        int max = 0;
        for (int i = 0; i < listOfAttackers.Count; i++)
        {
            currentValue = listOfAttackers[i].attackPriority;
            if (currentValue > maxValue)
            {
                maxValue = listOfAttackers[i].attackPriority;
                max = i;
            }
        }
        float temp = attackerNumber[max];
        backlog[(int)temp].hasToken = false;
        listOfAttackers.RemoveAt(max);
        attackerNumber.RemoveAt(max);


        //gets the highest number in range of attackers and removes it 
        //backlog[attackerNumber.IndexOf(listOfAttackers.IndexOf(listOfAttackers.Max()))].hasToken = false;
        //Removes the highest number currently in the list of attackers to be replaced if the number of attackers exceeds the number of tokens available
        //listOfAttackers.RemoveAt(listOfAttackers.IndexOf(listOfAttackers.Max()));
    }

    public void RequestToAttack(bool canAttack)
    {

    }
}
