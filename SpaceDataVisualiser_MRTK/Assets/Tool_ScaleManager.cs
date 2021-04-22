using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_ScaleManager : MonoBehaviour
{

    [SerializeField] private GameObject m_dragCursor;

    [SerializeField] private GameObject m_dragAnchor;

    [SerializeField] private GameObject m_dragAnchorVisual;

    [SerializeField] private GameObject m_visualisationManager;


    float m_storedScale = 1;
    float m_currentScale = 1;

    // Start is called before the first frame update
    void Start()
    {
        if (m_visualisationManager != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();
            m_storedScale = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float m_rawScale = 1 + (m_dragCursor.transform.position.y - m_dragAnchor.transform.position.y);

        Debug.Log("Raw Distance: " + m_rawScale.ToString());

        m_currentScale = m_storedScale * m_rawScale;

        m_dragAnchorVisual.transform.localScale = new Vector3(m_rawScale * 0.1f, m_rawScale * 0.1f, m_rawScale * 0.1f);

        if (m_visualisationManager != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();
            VM.SetUIScale(m_currentScale);
            Debug.Log("Current Scale to Set: " + m_currentScale.ToString());
        }
    }

    public void FinishDrag()
    {
        // Save offset
        m_storedScale = m_currentScale;

        // Reset Cursor Position to 0
        m_dragAnchorVisual.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        m_dragCursor.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

        print(m_storedScale);

    }
}
