using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsUser : MonoBehaviour {
    public bool UseYAxis = false;
	void Update () {
        if (Camera.main == null)
        {
            return;
        }
        else
        {
            var lookPos = transform.position - Camera.main.transform.position;
            if (UseYAxis == false)
            {
                lookPos.y = 0;
            }
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
        
    }
}
