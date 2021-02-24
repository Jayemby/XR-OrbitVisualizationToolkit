using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMenuManager : MonoBehaviour
{

    public GameObject[] menuTools;

    public int currentTool = 0;
    public void IncrementTool()
    {
        Debug.Log("Increment Tool!");
        if (menuTools.Length > 0)
        {
            menuTools[currentTool].gameObject.SetActive(false);

            if (currentTool == (menuTools.Length - 1))
            {
                currentTool = 0;
            }    
            else
            {
                currentTool++;
            }
            menuTools[currentTool].gameObject.SetActive(true);

        }
    }

    public void DecrementTool()
    {
        Debug.Log("Decrement Tool!");
        if (menuTools.Length > 0)
        {
            menuTools[currentTool].gameObject.SetActive(false);

            if (currentTool == 0)
            {
                currentTool = (menuTools.Length - 1);
            }
            else
            {
                currentTool--;
            }
            menuTools[currentTool].gameObject.SetActive(true);
        }

    }

    public void HideTool()
    {
        Debug.Log("Hide Tool!");
        menuTools[currentTool].gameObject.SetActive(false);
    }

    public void ShowTool()
    {
        Debug.Log("Show Tool!");
        menuTools[currentTool].gameObject.SetActive(true);
    }
}
