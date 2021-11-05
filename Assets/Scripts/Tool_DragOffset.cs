using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tool_DragOffset : MonoBehaviour
{
    [SerializeField] private GameObject m_dragCursor;

    [SerializeField] private GameObject m_dragAnchor;

    [SerializeField] private GameObject m_visualisationManager;

    private Vector3 m_storedOffset = new Vector3(0, 0, 0);
    private Vector3 m_currentOffset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        if (m_visualisationManager != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();
            m_storedOffset = VM.GetOffsetVector();
         }
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 m_rawOffset = m_dragCursor.transform.position - m_dragAnchor.transform.position;

        m_currentOffset = m_storedOffset + m_rawOffset;

        if (m_visualisationManager != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();
            VM.SetOffsetVector(m_currentOffset);
        }
    }

    public void FinishDrag()
    {
        // Save offset
        m_storedOffset = m_currentOffset;

        // Reset Cursor Position to 0
        m_dragCursor.transform.localPosition = new Vector3(0, 0, 0);

        print(m_storedOffset);

    }
}
