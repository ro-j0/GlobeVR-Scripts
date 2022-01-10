using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace WPM
{
    public class Visualizer : MonoBehaviour
    {   
        string filename = "Assets/Scripts/Test.json";
        public GameObject prefab;
		GUIStyle labelStyle;
		GameObject currentPanel;
        WorldMapGlobe map;

        DataParse provinceData = new DataParse(); 

        // Start is called before the first frame update
        void Start()
        {
            // Get a reference to the World Map API
            map = WorldMapGlobe.instance;

            // UI Setup 
		    labelStyle = new GUIStyle ();
		    labelStyle.alignment = TextAnchor.MiddleLeft;
			labelStyle.normal.textColor = Color.white;

            // setup GUI resizer
            //GUIResizer.Init (160000, 100000);

            /* Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city 
            map.OnCountryEnter += (int countryIndex, int regionIndex) => Debug.Log("Entered country " + map.countries[countryIndex].name);
            map.OnCountryExit += (int countryIndex, int r1024egionIndex) => Debug.Log("Exited country " + map.countries[countryIndex].name);
            map.OnCountryPointerDown += (int countryIndex, int regionIndex) => Debug.Log("Pointer down on country " + map.countries[countryIndex].name);
            map.OnCountryClick += (int countryIndex, int regionIndex) => Debug.Log("Clicked country " + map.countries[countryIndex].name);
            map.OnCountryPointerUp += (int countryIndex, int regionIndex) => Debug.Log("Pointer up on country " + map.countries[countryIndex].name);
            */
            ReadData(filename);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButton(0) || Input.GetMouseButton(1)){
                AddPanel();
            }
            
        }

        // Adds a UI panel
        void AddPanel() 
        {   
            // Destroy Previous Panel
            if(currentPanel != null)
            {
                Destroy(currentPanel);
            }

            // Get country and assign it COVID cases and deaths
            Province USA = map.GetProvince(map.countries[map.provinces[map.provinceLastClicked].countryIndex].name, map.provinces[map.provinceLastClicked].name);
            // Debug.Log(map.countries[map.provinces[map.provinceLastClicked].countryIndex].name);
            // Debug.Log(map.provinces[map.provinceLastClicked].name);

            // Instantiate Panel
            currentPanel =  Instantiate<GameObject> (prefab);

            // Update panel texts
            UpdatePanel(USA);

            //map.FlyToProvince(USA.name);

            // Position the canvas over the globe
			float distaceToGlobeCenter = 1.2f;
			Vector3 worldPos = map.transform.TransformPoint (USA.localPosition * distaceToGlobeCenter);
		    currentPanel.transform.position = worldPos;

            // Parent the panel to the globe so it rotates with it
			currentPanel.transform.SetParent (map.transform, true);
            
        }
        /*
        void AddPanelProvince() 
        {   
            // Destroy Previous Panel
            if(currentPanel != null)
            {
                Destroy(currentPanel);
            }

            // Get country and assign it COVID cases and deaths
            Province Ca = map.GetProvince("United States of America","California");
            Debug.Log(
                Ca.attrib.ToString()

            );
            map.FlyToProvince("California");
        }
        */

        //Add Data for Province    
        void AddData(Province name, long cases, long death, long rec)
        {
            name.attrib["COVID Cases"] = cases;
            name.attrib["COVID Deaths"] = death;
            name.attrib["COVID Recovered Cases"] = rec;
        }  

        void AddData(Country name, long cases, long death, long rec)
        {
            name.attrib["COVID Cases"] = cases;
            name.attrib["COVID Deaths"] = death;
            name.attrib["COVID Recovered Cases"] = rec;
        }  

        void UpdatePanel(Province C)
        {
            Text countryName, covidCases, covidDeaths, covidRecovery;
            countryName = currentPanel.transform.Find ("Panel/RowCountry/CountryName").GetComponent<Text> ();
            covidCases = currentPanel.transform.Find ("Panel/RowCOVIDCases/COVIDCases").GetComponent<Text> ();
            covidDeaths = currentPanel.transform.Find ("Panel/RowCOVIDDeaths/COVIDDeaths").GetComponent<Text> ();
            covidRecovery = currentPanel.transform.Find("Panel/RowCOVIDRecovery/COVIDRecovery").GetComponent<Text> ();

            countryName.text = C.name;
            Debug.Log(C.name);
            covidCases.text = C.attrib["COVID Cases"].ToString();
            covidDeaths.text = C.attrib["COVID Deaths"].ToString();
            covidRecovery.text = C.attrib["COVID Recovered Cases"].ToString();
        }

        void ReadData(string path){
            string contents = System.IO.File.ReadAllText(path);
            provinceData = JsonUtility.FromJson<DataParse>(contents);
            foreach (StateData state in provinceData.stateDatas)
            {
                //Debug.Log(state.Province_State);
                //Debug.Log(map.GetProvince(state.Country_Region,state.Province_State));
                if(map.GetProvince(state.Country_Region,state.Province_State) != null)
                    AddData(map.GetProvince(state.Country_Region,state.Province_State),state.Confirmed,state.Deaths,state.Recovered);
            }
        }
    }
}