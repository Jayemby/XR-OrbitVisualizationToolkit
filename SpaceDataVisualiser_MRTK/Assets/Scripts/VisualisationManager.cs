using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using QuickType;


public class VisualisationManager : MonoBehaviour
{
    // Data management variables:
    [SerializeField] private GameObject m_orbitManagerPrefab;
    private GameObject[] m_orbitManagers;
    public string m_localFilePath = null;
    private string m_jsonData;
    private OrbitalDataUnity m_orbitalDataUnity;

    // Enables / Disables automatic detection of spacecraft based on orbit radius
    [SerializeField] private bool m_flagAsScFromRadius = true;

    // Visualisation Status:
    enum LoadStatus
    {
        WaitingForFilepath,
        LoadingJson,
        LoadingComplete,
        Ready
    }

    private LoadStatus m_CurrentStatus = LoadStatus.WaitingForFilepath;

    // Global Visualisation Variables:
    [SerializeField] private Vector3 m_visualisationOffset = new Vector3(0,0,0);
    [SerializeField] private Vector3 m_visualisationScale = new Vector3(1,1,1);
    
    // Time Management Variables:
    [SerializeField] private double m_julianDate;

    enum TimeStatus
    {
        SingleUpdate,
        Paused,
        UseTimeStep,
        UseRealTime
    }

    [SerializeField] private TimeStatus m_currentTimeStatus = TimeStatus.Paused;

    [SerializeField] private double m_timeStep = 0;

    private float m_updateFrequency = 0.05f;
    private List<double> m_rawTimes;
    [SerializeField] private List<double> m_allTimes;

    [SerializeField] private List<GameObject> m_UIRecievers;

    // Data scale values:
    public int m_scaleValue;
    //Scale Values
    private int m_scaleStartValue;
    private int m_scaleMinValue;
    private int m_scaleMaxValue;
    //KM Scale Values
    private int m_KMStartValue = 10000;
    private int m_KMMinValue = 1000;
    private int m_KMMaxValue = 30000;
    //AU Scale Values
    private int m_AUStartValue = 1;
    private int m_AUMinValue = 1;
    private int m_AUMaxValue = 100;

    private Dictionary<string, scHashes> scStrToHash = new Dictionary<string, scHashes>();
    protected enum scHashes
    {
        SC1, SC2, SC3, SC4
    };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(m_CurrentStatus)
        {
            case LoadStatus.WaitingForFilepath:
                // We are currently waiting for a filepath to load from.

                // Check to see if there is a filepath.
                if (m_localFilePath != null)
                {
                    // A path exists

                    m_CurrentStatus = LoadStatus.LoadingJson;

                    StartCoroutine(LoadJsonData());

                    Debug.Log("Loading from filepath: " + m_localFilePath);
                }
                break;

            case LoadStatus.LoadingJson:
                // We are currently waiting for the LoadJsonData coroutine to finish.

                // Do a check to see if its still running correctly.

                break;

            case LoadStatus.LoadingComplete:
                // The JSON loading is complete we should start the orbit update repeater:
                InvokeRepeating(nameof(OrbitUpdate), 0, m_updateFrequency);
                m_CurrentStatus = LoadStatus.Ready;

                break;

            case LoadStatus.Ready:
                // The visulisation is ready and updating

                break;
        }
    }

    #region Offset Methods
    public Vector3 GetOffsetVector()
    {
        return m_visualisationOffset;
    }

    public void SetOffsetVector(Vector3 i_Offset)
    {
        m_visualisationOffset = i_Offset;
    }

    public void ResetOffsetVector()
    {
        m_visualisationOffset = new Vector3(0, 0, 0);
    }

    #endregion

    #region Time Methods
    public double GetJulianDate_Raw()
    {
        return m_julianDate;
    }

    public string GetJulianDate_String()
    {
        string julianDateString = "";
        
        int Y = 0, M = 0, D = 0, hh = 0, mm = 0, ss = 0;

        YMDhms(m_julianDate, ref Y, ref M, ref D, ref hh, ref mm, ref ss, true);

        julianDateString = $"{Y}/{M:00}/{D:00} {hh:00}:{mm:00}:{ss:00}";

        return julianDateString;
    }

    public static void YMDhms(double JD, ref int Y, ref int M, ref int D, ref int hh, ref int mm, ref int ss,
                            bool offset)
    {
        if (offset)
            JD += 2430000f;
        int J = (int)(JD + 0.5);
        int f = J + 1401 + (((4 * J + 274277) / 146097) * 3) / 4 - 38;
        int e = 4 * f + 3;
        int g = (e % 1461) / 4;
        int h = 5 * g + 2;
        D = (h % 153) / 5 + 1;
        M = ((h / 153 + 2) % 12) + 1;
        Y = e / 1461 - 4716 + (14 - M) / 12;
        double rem = (JD - J) + 0.5;
        hh = (int)(rem * 24);
        rem = rem * 24 - hh;
        mm = (int)(rem * 60);
        rem = rem * 60 - mm;
        ss = (int)(rem * 60);
    }

    public void SetJulianDate(double i_JulianDate)
    {
        // Add checks here
        m_julianDate = i_JulianDate;
    }
    #endregion


    #region Data Management Methods

    private IEnumerator LoadJsonData()
    {
        //using (UnityWebRequest www = new UnityWebRequest(m_localFilePath))
        //{
        //    yield return www;
        //    m_jsonData = www.downloadHandler.text;
        //}

        UnityWebRequest www = new UnityWebRequest(m_localFilePath);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            m_jsonData = www.downloadHandler.text;
        }

        OrbitalData orbitData = OrbitalData.FromJson(m_jsonData);
        m_orbitalDataUnity = new OrbitalDataUnity(orbitData);
        Debug.Log("generating satellites");
        GenerateSatellites();
        Debug.Log("Setting current scale value to " + m_scaleStartValue + "Units = " + m_orbitalDataUnity.Info.Units);

        // CurrentScale = ScaleValue;

        m_CurrentStatus = LoadStatus.LoadingComplete;


        yield return null;
    }

    private void GenerateSatellites()
    {
        //setup Scale values        
        if (m_orbitalDataUnity.Info.Units == "km")
        {
            Debug.Log("Units = Kilometres");
            m_scaleStartValue = m_KMStartValue;
            m_scaleMinValue = m_KMMinValue;
            m_scaleMaxValue = m_KMMaxValue;
        }
        if (m_orbitalDataUnity.Info.Units == "au")
        {
            Debug.Log("Units = Astronomical Units");
            m_scaleStartValue = m_AUStartValue;
            m_scaleMinValue = m_AUMinValue;
            m_scaleMaxValue = m_AUMaxValue;
        }
        m_scaleValue = m_scaleStartValue;

        //setup orbits
        for (int i = 0; i < m_orbitalDataUnity.Orbits.Count; i++)
        {
            // Debug.Log("creating Orbit");
            //instantiate orbit manager prefab as child (which includes all the necessary game objects)
            GameObject orbitchild = Instantiate(m_orbitManagerPrefab, transform.position, Quaternion.identity) as GameObject;
            orbitchild.transform.parent = this.gameObject.transform;

            //Set Orbitmanager Game object name to Orbit name
            orbitchild.name = m_orbitalDataUnity.Orbits[i].Name;
            //cache OrbitManagement script from orbitchild gameobject
            OrbitManagement OM = orbitchild.GetComponent<OrbitManagement>();

            #region Displaytype
            //Draw Line and Satellite, or just satelitte.
            QuickType.Display displaytype = m_orbitalDataUnity.Orbits[i].Display;
            if (displaytype == QuickType.Display.LinePoint) //linepoint
            {
                OM.Line = true;
            }
            if (displaytype == QuickType.Display.Point)
            {
                OM.Line = false;
            }
            #endregion

            #region Radii
            float radii = new float();
            if (m_orbitalDataUnity.Info.Units == "km")
            {
                radii = (float)m_orbitalDataUnity.Orbits[i].Radius / m_scaleValue;
                if (radii < 0.0025f)
                {
                    OM.Radius = 0.0025f;
                }
                else
                {
                    OM.Radius = radii;
                }
            }
            if (m_orbitalDataUnity.Info.Units == "au")
            {
                radii = (float)m_orbitalDataUnity.Orbits[i].Radius;
                if (radii < 0.0125)
                {
                    OM.Radius = 0.0125f;
                }
                else
                {
                    OM.Radius = radii;
                }
            }

            #endregion

            #region Models and Textures
            bool isCelestialBody = false;
            // if name matches premade material, then use matching material as Orbiter
            switch (m_orbitalDataUnity.Orbits[i].Name)
            {
                case "Sun":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Sun");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Mercury":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Mercury");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Venus":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Venus");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Earth":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Earth");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Luna":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Luna");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Mars":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Mars");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Jupiter":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Jupiter");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Saturn":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Saturn");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Uranus":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Uranus");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Neptune":
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/CelestialBodies/Neptune");
                    OM.drawScModel = false; isCelestialBody = true;
                    break;
                case "Sat":
                case "DefaultSC":
                    // GMAT default names, insert more here if necessary
                    OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                    OM.drawScModel = true;
                    break;
                default:
                    // if ((OM.Radius < 300f) || (flagAsScFromRadius == true))    
                    if (((float)m_orbitalDataUnity.Orbits[i].Radius < 300f) || (m_flagAsScFromRadius == true))
                    // optional radius check
                    // *** mind the data type
                    {
                        OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                        OM.drawScModel = true;
                        break;
                    }
                    // must clear prior settings, otherwise they will propagate across
                    // to the next orbit object this function manages 
                    OM.inheritedMaterial = null;
                    OM.drawScModel = false;
                    break;
            }

            // user set spacecraft names matched up here
            if (scStrToHash.ContainsKey(m_orbitalDataUnity.Orbits[i].Name))
            {
                switch (scStrToHash[m_orbitalDataUnity.Orbits[i].Name])
                {
                    case scHashes.SC1:
                        OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                        OM.drawScModel = true;
                        break;
                    case scHashes.SC2:
                        OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                        OM.drawScModel = true;
                        break;
                    case scHashes.SC3:
                        OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                        OM.drawScModel = true;
                        break;
                    case scHashes.SC4:
                        OM.inheritedMaterial = Resources.Load<Material>("Materials/Spacecraft/SatelliteMaterial");
                        OM.drawScModel = true;
                        break;
                    default:
                        OM.inheritedMaterial = null;
                        OM.drawScModel = false;
                        break;
                }
            }

            #endregion

            #region Eph
            //Generate new rawpositions list for each instantiated orbitmanager
            OM.RawPositions = new List<Vector3>();
            //extract positions list from RawEphData
            foreach (RawEphData data in m_orbitalDataUnity.Orbits[i].Eph)
            {
                //convert rawEphData position doubles into floats                
                float xpos = (float)data.xPos;
                float ypos = (float)data.yPos;
                float zpos = (float)data.zPos;
                // convert floats divided by descaler value into Vector3 positions
                Vector3 positions = new Vector3(xpos / m_scaleValue, ypos / m_scaleValue, zpos / m_scaleValue);
                //pass list of raw positions to orbit management script
                OM.RawPositions.Add(positions);
            }
            #endregion

            #region Att
            // Generate new rawattitude list for each instantiated orbitmanager
            OM.RawRotationStates = new List<Quaternion>();

            // Extract from RawAttData
            if (m_orbitalDataUnity.Orbits[i].Att != null)
            // if there is data in Att
            {
                foreach (RawAttData data in m_orbitalDataUnity.Orbits[i].Att)
                {
                    Quaternion rotation;

                    //// convert RawAttData rotation doubles into floats
                    //// no mapping
                    //float X = (float)data.X;
                    //float Y = (float)data.Y;
                    //float Z = (float)data.Z;
                    //float W = (float)data.W;

                    // method for converting quaternions from right to left handed system
                    // https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system
                    //float X = (float)data.Y;    // -(  right = -left  )
                    //float Y = -(float)data.Z;   // -(     up =  up     )
                    //float Z = -(float)data.X;   // -(forward =  forward)
                    //float W;


                    // method by inspection. Refer to report
                    float X = (float)data.X;
                    float Y = (float)data.Z;
                    float Z = (float)data.Y;
                    float W;

                    if (isCelestialBody)
                    {
                        W = -(float)data.W;
                    } // (axis fine, flip rotation dir)
                    else
                    {
                        W = (float)data.W;
                    } // (axis fine, keep rotation dir)



                    // implement scaling here, if needed
                    rotation = new Quaternion(X, Y, Z, W);

                    //pass list of raw positions to orbit management script
                    OM.RawRotationStates.Add(rotation);
                }
                isCelestialBody = false;
                OM.hasAttitude = true;
            }
            else
                OM.hasAttitude = false;
            #endregion

            #region Time
            OM.RawJulianTime = new List<double>();
            OM.RawJulianTime.AddRange(m_orbitalDataUnity.Orbits[i].Time);

            // match length against stored previous array?
            #endregion

            #region Colour
            if (m_orbitalDataUnity.Orbits[i].Color != null)
            {
                Color colour = new Color(); // black as default
                string[] splitTriplet = m_orbitalDataUnity.Orbits[i].Color.Split(',');

                // ensure R,G,B components between 0 and 255
                bool colourValid = true;
                for (int j = 0; j < splitTriplet.Length; j++)
                {
                    if ((int.Parse(splitTriplet[j])) > 255
                        || (int.Parse(splitTriplet[j])) < 0)
                    {
                        colourValid = false;
                    }
                }
                if (colourValid)
                {
                    // alternatively, use pointers and strsep
                    // scaling from 0-255 to 0-1
                    colour.r = (float.Parse(splitTriplet[0])) / 255;
                    colour.g = (float.Parse(splitTriplet[1])) / 255;
                    colour.b = (float.Parse(splitTriplet[2])) / 255;
                    colour.a = 1f;

                    OM.LineColour = colour;
                }
                else
                {
                    // OM.LineColour stays black
                    Debug.LogWarning("RGB triplet component out of range. " +
                                        "Blacl colour carried forward");
                }

            }
            #endregion

            #region Array Length Check
            // this check will have to be skirted if data is thinned out or otherwise changed
            // by DataManager::WriteToJson in plugin
            // consider checking contents of Eph or Att, or setting flags in JSON
            if (!ArrayMismatchCheck(m_orbitalDataUnity.Orbits[i].Eph,
                                m_orbitalDataUnity.Orbits[i].Att,
                                m_orbitalDataUnity.Orbits[i].Time, OM.hasAttitude))
                Debug.LogError("Ephemeris, Attitude or Time array size mismatch");
            #endregion


            //create new list of orbital objects for each instance of OrbitManagement
            OM.orbitalobjects = new List<GameObject>();
        }

        Debug.Log("Orbit Managers Generated");




        // This needs to be removed / changed to reference internal variables instead of a seperate script

        // MainMenuUIManager MM;
        // MM = Pedestal.GetComponent<MainMenuUIManager>();
        // MM.OrbitsCreated = true;
        // MM.CoordinatesText.text = "Coordinates: " + m_orbitalDataUnity.Info.Coordinates;
        // MM.UnitsText.text = "Units: " + m_orbitalDataUnity.Info.Units;
        // MM.NewScaleValue = ScaleValue;

        // Store reference to the orbit manager objects
        m_orbitManagers = GameObject.FindGameObjectsWithTag("OrbitalManager");

        // Update stored time values
        AllTimesUpdate();

    } 

    private bool ArrayMismatchCheck(List<RawEphData> E, List<RawAttData> A, double[] T, bool attitude)
    {
        if (E.Count != T.Length)
            return false;
        if (attitude)
            if ((E.Count != A.Count) || (A.Count != T.Length))
                return false;
        return true;
    }

    #endregion

    private void AllTimesUpdate()
    {
        m_rawTimes = new List<double>();

        foreach (GameObject OrbitManager in m_orbitManagers)
        {
            OrbitManagement OM = OrbitManager.GetComponent<OrbitManagement>();
            m_rawTimes.AddRange(OM.RawJulianTime);
        }

        m_rawTimes.Sort((a, b) => a.CompareTo(b));

        m_allTimes = new List<double>();
        double previousValue = 0;
        foreach (double time in m_rawTimes)
        {
            if (time != previousValue)
            {
                m_allTimes.Add(time);
                previousValue = time;
            }
        }

        // GlobalJulianDateSlider.minValue = 0;
        // GlobalJulianDateSlider.maxValue = m_allTimes.Count - 1;
        // GlobalJulianDateSlider.value = 0;
        // Debug.Log("updated Alltimes");
        m_julianDate = m_allTimes[0];
    }

    //------------------------------------------------------------------------------
    // public void OrbitUpdate()
    //------------------------------------------------------------------------------
    /*
     * Currently invoked by Update() if AllTimes updated (amongst 2 other cases).
     * Advances JulianDate based on options. Calls relevant TM methods.
     */
    //------------------------------------------------------------------------------
    private void OrbitUpdate()
    {

        switch (m_currentTimeStatus)
        {
            case TimeStatus.UseRealTime:
                // The visualisation is using the current real world time

                foreach (GameObject OrbitManager in m_orbitManagers)
                {
                    TimeManipulator TM = OrbitManager.GetComponent<TimeManipulator>();
                    TM.UseRealTime = true;
                    TM.UpdateOrbiterPosition();
                }
                break;

            case TimeStatus.UseTimeStep:
                // The visualisation is using a set timestep

                double newJulianDate = 0;
                // set NewJulian date to current JulianDate + scaled timestep increment 
                newJulianDate = m_julianDate + (m_timeStep / (1 / m_updateFrequency));

                if (newJulianDate > m_allTimes[m_allTimes.Count - 1])
                {
                    // If the visuisation goes past the last time point then reset to the beginning
                    newJulianDate = m_allTimes[0];
                }

                // Distribute JulianDate here
                if (m_julianDate != newJulianDate)
                {
                    //Debug.Log("Updating Orbit with Dataset Values + timestep");
                    m_julianDate = newJulianDate;
                    foreach (GameObject OrbitManager in m_orbitManagers)
                    {
                        TimeManipulator TM = OrbitManager.GetComponent<TimeManipulator>();
                        TM.UseRealTime = false;
                        TM.JulianDate = m_julianDate;
                        TM.UpdateOrbiterPosition();
                    }
                }

                break;
            case TimeStatus.SingleUpdate:
                // The visualisation is doing a single update then will be paused.
                m_currentTimeStatus = TimeStatus.Paused;

                //Debug.Log("Updating Orbit with Dataset Values");
                foreach (GameObject OrbitManager in m_orbitManagers)
                {
                    TimeManipulator TM = OrbitManager.GetComponent<TimeManipulator>();
                    TM.UseRealTime = false;
                    TM.JulianDate = m_julianDate;
                    TM.UpdateOrbiterPosition();
                }
                break;

            case TimeStatus.Paused:
                // The visualisation is paused, no update is needed
                break;
        }
    }




    public void SetTimeStatus_SingleUpdate()
    {
        m_currentTimeStatus = TimeStatus.SingleUpdate;
    }
    public void SetTimeStatus_Paused()
    {
        m_currentTimeStatus = TimeStatus.Paused;
    }

    public void SetTimeStatus_UseTimeStep()
    {
        m_currentTimeStatus = TimeStatus.UseTimeStep;
    }

    public void SetTimeStatus_UseRealTime()
    {
        m_currentTimeStatus = TimeStatus.UseRealTime;
    }

    public string GetVisualisationUnits()
    {
        return m_orbitalDataUnity.Info.Units;
    }

    public void SetJulianDateFromControl(float i_controlValue)
    {
        // Debug.Log("Setting julian date from control");
        // Gets value between 0 and 1

        if (i_controlValue > 1)
        {
            i_controlValue = 1;
        }
        else if (i_controlValue < 0)
        {
            i_controlValue = 0;
        }

        // We need to use this value to get a date from our list to use

        float dateIndexFloat = i_controlValue * m_allTimes.Count;
        // Debug.Log(dateIndexFloat);

        int dateIndexInt = Mathf.RoundToInt(dateIndexFloat);

        // Debug.Log(dateIndexInt);

        m_julianDate = m_allTimes[dateIndexInt];

        if (m_currentTimeStatus == TimeStatus.Paused)
        {
            m_currentTimeStatus = TimeStatus.SingleUpdate;
        }

    }

    public bool GetTimeStatus_TimeStep()
    {
        bool o_TimeStepStatus = false;

        if (m_currentTimeStatus == TimeStatus.UseTimeStep)
        {
            o_TimeStepStatus = true;
        }

        return o_TimeStepStatus;
    }

    public bool GetTimeStatus_RealTime()
    {
        bool o_RealTimeStatus = false;

        if (m_currentTimeStatus == TimeStatus.UseRealTime)
        {
            o_RealTimeStatus = true;
        }

        return o_RealTimeStatus;
    }

    public float GetTimeProgress()
    {
        // Returns a value between 0 and 1 for where the visuisation is in the list of times
        float timeValue = 0;

        double firstTime = m_allTimes[0];

        double lastTime = m_allTimes[m_allTimes.Count - 1];

        double duration = lastTime - firstTime;

        double currentProgress = m_julianDate - firstTime;

        timeValue = (float)(currentProgress / duration);

        Debug.Log(timeValue);

        return timeValue;
    }
}
