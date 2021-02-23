using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragOffset : MonoBehaviour
{
    public Vector3 DragOffsetRaw = new Vector3(0,0,0);

    public Vector3 StoredOffset = new Vector3(0, 0, 0);

    public Vector3 CurrentDragOffset = new Vector3(0, 0, 0);

    public GameObject DragCursor;

    public GameObject JsonManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DragOffsetRaw = this.transform.position - DragCursor.transform.position;
        CurrentDragOffset = StoredOffset + DragOffsetRaw;

        if (JsonManager != null)
        {
            JsonManager.transform.localPosition = new Vector3(0,1,0) - CurrentDragOffset;
        }
    }

    public void FinishDrag()
    {
        // Save offset
        StoredOffset = CurrentDragOffset;

        // Reset Cursor Position to 0
        DragCursor.transform.localPosition = new Vector3(0, 0, 0);

        print(StoredOffset);

    }
}
