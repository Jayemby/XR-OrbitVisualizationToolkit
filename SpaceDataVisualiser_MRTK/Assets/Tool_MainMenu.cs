using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tool_MainMenu : MonoBehaviour
{

    [Header("OLD Interactible UI Elements")]
    [SerializeField] private Toggle m_globalLineToggle;
    [SerializeField] private Toggle m_globalTagToggle;
    [SerializeField] private Slider m_globalLineWidthSlider;
    [SerializeField] private Slider m_globalShapeToleranceSlider;
    [SerializeField] private Slider m_globalScaleSlider;
    [SerializeField] private Toggle m_globalRealTimeToggle;
    [SerializeField] private Slider m_globalJulianDateSlider;
    [SerializeField] private Slider m_timeStepSlider;
    [SerializeField] private Slider m_heightSlider;
    [SerializeField] private Toggle m_useRotationToggle;


    [Header("OLD UI text Elements")]    
    [SerializeField] private TMP_Text m_globalLinewidthtext;
    [SerializeField] private TMP_Text m_globalShapeTolerancetext;
    [SerializeField] private TMP_Text m_globalScaletext;
    [SerializeField] private TMP_Text m_globalJulianDatetext;
    [SerializeField] private TMP_Text m_globalJulianDateTextAlt;
    
    [SerializeField] private TMP_Text m_unitsText;
    [SerializeField] private TMP_Text m_timeStepText;
    [SerializeField] private TMP_Text m_heightSliderValueText;


    [Header("Interactible UI Elements")]
    [SerializeField] private Toggle m_useTimeStepToggle;


    [Header("UI text Elements")]
    [SerializeField] private TMP_Text m_coordinatesField;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitAppButtonPressed()
    {
        Application.Quit();
    }

}
