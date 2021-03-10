using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tool_MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject VisualisationManager;

    [Header("OLD Interactible UI Elements")]
    [SerializeField] private Toggle m_globalLineToggle;
    [SerializeField] private Toggle m_globalTagToggle;
    [SerializeField] private Slider m_globalLineWidthSlider;
    [SerializeField] private Slider m_globalShapeToleranceSlider;
    [SerializeField] private Slider m_globalScaleSlider;
    [SerializeField] private Toggle m_globalRealTimeToggle;
    
    [SerializeField] private Slider m_timeStepSlider;
    [SerializeField] private Slider m_heightSlider;
    [SerializeField] private Toggle m_useRotationToggle;


    [Header("OLD UI text Elements")]    
    [SerializeField] private TMP_Text m_globalLinewidthtext;
    [SerializeField] private TMP_Text m_globalShapeTolerancetext;
    [SerializeField] private TMP_Text m_globalScaletext;
    
    [SerializeField] private TMP_Text m_unitsText;
    [SerializeField] private TMP_Text m_timeStepText;
    [SerializeField] private TMP_Text m_heightSliderValueText;


    [Header("Interactible UI Elements")]

    [SerializeField] private Toggle m_realTimeToggle;
    [SerializeField] private Slider m_julianDateSlider;
    [SerializeField] private Toggle m_useTimeStepToggle;

    [Header("UI text Elements")]
    [SerializeField] private TMP_Text m_coordinatesField;
    [SerializeField] private TMP_Text m_rawJulianDatetext;
    [SerializeField] private TMP_Text m_julianDateText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMenuUI();
    }

    public void ExitAppButtonPressed()
    {
        Application.Quit();
    }

    public void UpdateMenuUI()
    {

        VisualisationManager VM = VisualisationManager.GetComponent<VisualisationManager>();

        if (VM != null)
        {
            m_coordinatesField.text = VM.GetVisualisationUnits();
            m_julianDateText.text = VM.GetJulianDate_String();
            m_rawJulianDatetext.text = VM.GetJulianDate_Raw().ToString();



            m_useTimeStepToggle.SetIsOnWithoutNotify(VM.GetTimeStatus_TimeStep());
            m_realTimeToggle.SetIsOnWithoutNotify(VM.GetTimeStatus_RealTime());

            m_julianDateSlider.SetValueWithoutNotify(VM.GetTimeProgress());
        }

    }

    public void TimeStepToggle_ValueChange()
    {
        // The time step toggle has been pressed
        Debug.Log("Time Step Toggle Changed");
        
        // Get the value of the toggle
        bool useTimeStep = m_useTimeStepToggle.isOn;

        VisualisationManager VM = VisualisationManager.GetComponent<VisualisationManager>();

        if (useTimeStep)
        {
            Debug.Log("TimeStatus: Use time step");
            VM.SetTimeStatus_UseTimeStep();
        }
        else
        {
            Debug.Log("TimeStatus: Paused");
            VM.SetTimeStatus_Paused();
        }
    }

    public void JulianDate_ValueChange()
    {

        if (m_julianDateSlider != null)
        {
            VisualisationManager VM = VisualisationManager.GetComponent<VisualisationManager>();

            if (VM != null)
            {
                VM.SetJulianDateFromControl(m_julianDateSlider.value);
            }
        }

    }




}
