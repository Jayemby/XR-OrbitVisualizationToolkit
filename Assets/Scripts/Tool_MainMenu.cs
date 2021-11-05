using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SFB;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

#if ENABLE_WINMD_SUPPORT
using Windows.Storage.Pickers;
using Windows.Storage;
#endif

public class Tool_MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject m_visualisationManager;

    // [Header("OLD Interactible UI Elements")]
    // [SerializeField] private Toggle m_globalLineToggle;
    // [SerializeField] private Toggle m_globalTagToggle;
    // [SerializeField] private Slider m_globalLineWidthSlider;
    // [SerializeField] private Slider m_globalShapeToleranceSlider;
    // [SerializeField] private Slider m_globalScaleSlider;
    // [SerializeField] private Toggle m_globalRealTimeToggle;


    // [SerializeField] private Slider m_heightSlider;
    // [SerializeField] private Toggle m_useRotationToggle;


    // [Header("OLD UI text Elements")]    
    // [SerializeField] private TMP_Text m_globalLinewidthtext;
    // [SerializeField] private TMP_Text m_globalShapeTolerancetext;
    // [SerializeField] private TMP_Text m_globalScaletext;
    // 
    // [SerializeField] private TMP_Text m_unitsText;
    // 
    // [SerializeField] private TMP_Text m_heightSliderValueText;


    [Header("Interactible UI Elements")]

    [SerializeField] private Toggle m_realTimeToggle;
    [SerializeField] private Slider m_julianDateSlider;
    [SerializeField] private Toggle m_useTimeStepToggle;
    [SerializeField] private Slider m_timeStepSlider;

    [Header("UI text Elements")]
    [SerializeField] private TMP_Text m_coordinatesField;
    [SerializeField] private TMP_Text m_rawJulianDatetext;
    [SerializeField] private TMP_Text m_julianDateText;
    [SerializeField] private TMP_Text m_timeStepText;


    double[] m_timeSteps = { 0.0, 1.0 / (24.0 * 60.0 * 60.0), 10.0 / (24.0 * 60.0 * 60.0), 1.0 / (24.0 * 60.0), 1.0 / 24.0, 1.0, 30.0, 365.26 };
    string[] m_timeLabels = { "paused", "1 sec", "10 sec", "1 min", "1 hour", "1 day", "1 month", "1 year" };

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

        VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();

        if (VM != null)
        {
            m_coordinatesField.text = VM.GetVisualisationUnits();
            m_julianDateText.text = VM.GetJulianDate_String();
            m_rawJulianDatetext.text = VM.GetJulianDate_Raw().ToString();



            m_useTimeStepToggle.SetIsOnWithoutNotify(VM.GetTimeStatus_TimeStep());
            m_realTimeToggle.SetIsOnWithoutNotify(VM.GetTimeStatus_RealTime());

            m_julianDateSlider.SetValueWithoutNotify(VM.GetTimeProgress());




            double timeStep = VM.GetTimeStep();

            int timeStepIndex = GetTimeStepIndexFromTimeStep(timeStep);

            m_timeStepSlider.SetValueWithoutNotify(timeStepIndex);

            m_timeStepText.text = m_timeLabels[timeStepIndex];
        }

    }

    public void TimeStepToggle_ValueChange()
    {
        // The time step toggle has been pressed
        Debug.Log("Time Step Toggle Changed");

        // Get the value of the toggle
        bool useTimeStep = m_useTimeStepToggle.isOn;

        VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();

        if (useTimeStep)
        {
            Debug.Log("TimeStatus: Use time step");
            VM.SetTimeStatus_UseTimeStep();
            VM.SetOffsetVector(VM.GetOffsetVector() + new Vector3(1, 1, 1));
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
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();

            if (VM != null)
            {
                VM.SetJulianDateFromControl(m_julianDateSlider.value);
            }
        }
    }

    public void TimeStep_ValueChange()
    {

        if (m_timeStepSlider != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();

            int timeStepIndex = (int)m_timeStepSlider.value;

            if (timeStepIndex >= m_timeSteps.Length)
            {
                timeStepIndex = m_timeSteps.Length - 1;
            }

            double timeStepValue = m_timeSteps[timeStepIndex];

            // Debug.Log("TimeStep: " + timeStepValue.ToString());

            if (VM != null)
            {
                VM.SetTimeStep(timeStepValue);
            }
        }

    }

    private int GetTimeStepIndexFromTimeStep(double i_timeStep)
    {
        int index = 0;

        // Itterate though m_timeSteps to find the nearest value for the current timestep

        double smallestDifference = m_timeSteps[m_timeSteps.Length - 1];

        for (int i = 0; i < m_timeSteps.Length; i++)
        {
            double difference = 0;

            difference = System.Math.Abs(m_timeSteps[i] - i_timeStep);

            if (difference < smallestDifference)
            {
                smallestDifference = difference;
                index = i;
            }

        }


        return index;
    }

    public async void OpenFileBrowser()
    {
        if (m_visualisationManager != null)
        {
            VisualisationManager VM = m_visualisationManager.GetComponent<VisualisationManager>();
#if ENABLE_WINMD_SUPPORT
        // Windows FileOpenPicker Implementation:

        string FilePath = await LoadFileAsync();
        Debug.Log(FilePath);
        VM.SetFilePath(FilePath);
#else
            // Standalone File Browser Implementation:
            var paths = StandaloneFileBrowser.OpenFilePanel("Title", "", "json", false);
            string FileLocation = new System.Uri(paths[0]).AbsoluteUri;
            VM.SetFilePath(FileLocation);
#endif
        }
    }

    internal static async Task<string> LoadFileAsync()
    {
        {
#if ENABLE_WINMD_SUPPORT
            var pickCompleted = new TaskCompletionSource<string>();

            UnityEngine.WSA.Application.InvokeOnUIThread(
                async () =>
                {
                    Stream stream = null;
                    FileOpenPicker picker = new FileOpenPicker();
                    picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                    picker.FileTypeFilter.Add(".json");
                    picker.FileTypeFilter.Add("*");
                    picker.ViewMode = PickerViewMode.Thumbnail;
                    picker.CommitButtonText = "Select Data";

                    var file = await picker.PickSingleFileAsync();
                    string filePath = null;

                    if (file != null)
                    {
                        filePath = file.Path;
                    }
                    pickCompleted.SetResult(filePath);
                },
                true
            );

            await pickCompleted.Task;

            return (pickCompleted.Task.Result);

#else
            throw new InvalidOperationException(
                "Sorry, no file dialog support for other platforms here");
#endif

        }
    }
}
