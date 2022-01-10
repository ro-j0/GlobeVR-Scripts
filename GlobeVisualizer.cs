using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


namespace WPM
{
    public class GlobeVisualizer : MonoBehaviour
    {
        // Filepath
        string filepath = "Assets/Scripts/TestGlobeData.json";
        // USA Filepath
        string USAfilepath = "Assets/Scripts/Test.json";
        // The panel prefab used to display the data 
        public GameObject prefab;
        // Controller Position
        public GameObject cont;
        // Style elements for the GUI
        GUIStyle labelStyle;
        // GameObject for the current panel being displayed on 
        // screen
        GameObject currentPanel;
        // Map being used
        WorldMapGlobe map;
        // JSON Data 
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

            // Map check 
            map.OnCountryClick += (int countryIndex, int regionIndex) => Debug.Log("Clicked country " + map.countries[countryIndex].name);

            // Calls read data function with the filepath
            ReadData(filepath);
            ReadDataUSA(USAfilepath);

            Cursor.lockState = CursorLockMode.None;
        }

        // Update is called once per frame
        void Update()
        {   
            // Checks for mouse input to display panel 
            if(map.GetCountry(map.countryLastClicked) != null)
            {
                AddPanel();
            }
            if(map.GetCountry(map.countryLastClicked) == null && currentPanel != null){
                Destroy(currentPanel);
            }
            if(Input.GetKeyDown(KeyCode.I))
            {
                GoToFlatPlane();
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
            Country currentCountry = map.GetCountry(map.countryLastClicked);
            Province currentProvince = map.GetProvince(map.countries[map.provinces[map.provinceLastClicked].countryIndex].name, map.provinces[map.provinceLastClicked].name);
            Debug.Log(currentCountry.name);
            // Tester Logs
            // Debug.Log(map.countries[map.provinces[map.provinceLastClicked].countryIndex].name);
            Debug.Log(map.provinces[map.provinceLastClicked].name);

            // Instantiate Panel
            currentPanel =  Instantiate<GameObject> (prefab);
            currentPanel.transform.SetParent(cont.transform);
            currentPanel.transform.position = cont.transform.position + new Vector3(0, 0.2f, 0.1f);
            //currentPanel.transform.position = new Vector3(-0.5f, 0.5f, 0);
            // Update panel texts
            // if(currentProvince.attrib["COVID Cases"] == null)
            // {
            //     UpdatePanel(currentCountry);
            // }
            // else
            // {
            //     UpdatePanel(currentProvince);
            // }
            UpdatePanel(currentProvince);

            // Fly to the Province 
            map.FlyToProvince(currentProvince.name);

            // Position the canvas over the globe
			// float distaceToGlobeCenter = 1.4f;
			// Vector3 worldPos = map.transform.TransformPoint (currentProvince.localPosition * distaceToGlobeCenter);
		    // currentPanel.transform.position = worldPos;

            // Parent the panel to the globe so it rotates with it
			// currentPanel.transform.SetParent (map.transform, true);
        }
        // Add COVID data to the Province
        void AddData(Province name, long cases, long death, long rec)
        {
            name.attrib["COVID Cases"] = cases;
            name.attrib["COVID Deaths"] = death;
            name.attrib["COVID Recovered Cases"] = rec;
        }  

        // Add COVID data to the country
        void AddData(Country name, long cases, long death, long rec)
        {
            name.attrib["COVID Cases"] = cases;
            name.attrib["COVID Deaths"] = death;
            name.attrib["COVID Recovered Cases"] = rec;
        }  

        // Updates Panel with the information
        void UpdatePanel(Province curProv)
        {
            // Assigns text to the panel
            Text countryName, covidCases, covidDeaths, covidRecovery;
            countryName = currentPanel.transform.Find ("Panel/RowCountry/CountryName").GetComponent<Text> ();
            covidCases = currentPanel.transform.Find ("Panel/RowCOVIDCases/COVIDCases").GetComponent<Text> ();
            covidDeaths = currentPanel.transform.Find ("Panel/RowCOVIDDeaths/COVIDDeaths").GetComponent<Text> ();
            covidRecovery = currentPanel.transform.Find("Panel/RowCOVIDRecovery/COVIDRecovery").GetComponent<Text> ();

            // Writes text to the fields
            countryName.text = curProv.name;
            if(curProv.attrib["COVID Cases"] == null){
                UpdatePanel(map.GetCountry(curProv.countryIndex));
            }
            else{
                covidCases.text = curProv.attrib["COVID Cases"].ToString();
                covidDeaths.text = curProv.attrib["COVID Deaths"].ToString();
                covidRecovery.text = curProv.attrib["COVID Recovered Cases"].ToString();
                }
        }

        void UpdatePanel(Country curCountry)
        {
            Text countryName, covidCases, covidDeaths, covidRecovery;
            countryName = currentPanel.transform.Find ("Panel/RowCountry/CountryName").GetComponent<Text> ();
            covidCases = currentPanel.transform.Find ("Panel/RowCOVIDCases/COVIDCases").GetComponent<Text> ();
            covidDeaths = currentPanel.transform.Find ("Panel/RowCOVIDDeaths/COVIDDeaths").GetComponent<Text> ();
            covidRecovery = currentPanel.transform.Find("Panel/RowCOVIDRecovery/COVIDRecovery").GetComponent<Text> ();

            // Writes text to the fields
            if(curCountry.attrib["COVID Cases"] == null){
                countryName.text = curCountry.name;
                covidCases.text = "NA";
                covidDeaths.text = "NA";
                covidRecovery.text = "NA";
            }
            else{
                countryName.text = curCountry.name;
                covidCases.text = curCountry.attrib["COVID Cases"].ToString();
                covidDeaths.text = curCountry.attrib["COVID Deaths"].ToString();
                covidRecovery.text = curCountry.attrib["COVID Recovered Cases"].ToString();
            }
            
        }

        // Read the pata from the JSON file 
        void ReadData(string path)
        {
            long avg = 0;
            long num = 0;
            long big = 0;
            long small = 0;
            string contents = System.IO.File.ReadAllText(path);
            provinceData = JsonUtility.FromJson<DataParse>(contents);
            foreach (StateData state in provinceData.stateDatas)
            {   
                // Iterates through all the states from the JSON file
                if(state.Province_State != "")
                {
                    Province temp = map.GetProvince(state.Country_Region,state.Province_State);
                    if(temp != null)
                    {
                        AddData(temp,state.Confirmed,state.Deaths,state.Recovered);
                        heatMapGen(temp);
                        avg += state.Confirmed;
                        num++;
                        big = Math.Max(state.Confirmed,big);
                        small = Math.Min(state.Confirmed,small);
                    }  
                }
                else
                {
                    Country temp = map.GetCountry(state.Country_Region);
                    Debug.Log(state.Country_Region);
                    if(temp != null)
                    {
                        AddData(temp,state.Confirmed,state.Deaths, state.Recovered);
                        heatMapGen(temp);
                    }
                }
            }
            Debug.Log("STATE AVG NUM OF CASES");
            avg /= num;
            Debug.Log(avg);
            Debug.Log("MAX");
            Debug.Log(big);
            Debug.Log("MIN");
            Debug.Log(small);
        }

        // Reads Data from the USA JSON 
        void ReadDataUSA(string path)
        {
            long sum = 0;
            long sum1 = 0;
            long sum2 = 0;
            string contents = System.IO.File.ReadAllText(path);
            provinceData = JsonUtility.FromJson<DataParse>(contents);
            Debug.Log("ReadData");
            foreach (StateData state in provinceData.stateDatas)
            {   
                Debug.Log("State " + state.Province_State);
                if(state.Province_State != "")
                {
                    Province temp = map.GetProvince("United States of America",state.Province_State);
                    if(temp != null)
                    {
                        AddData(temp,state.Confirmed,state.Deaths,state.Recovered);
                        sum += state.Confirmed;
                        sum1 += state.Deaths;
                        sum2 += state.Recovered;
                        heatMapGen(temp);
                    }  
                }
            }
            Country C = map.GetCountry("United States of America");
            AddData(C,sum,sum1,sum2);
            heatMapGen(C);
            Debug.Log(sum);
        }

        void heatMapGen(Province C)
        {
            if(C.attrib["COVID Cases"] > 150000){
                Color newColor = new Color(0.75f, 0.0f, 0.15f, 1f);
                map.ToggleProvinceSurface(C,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 90000)
            {
                Color newColor = new Color(0.94f, 0.231f, 0.125f, 1f);
                map.ToggleProvinceSurface(C,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 50000)
            {
                Color newColor = new Color(0.992f, 0.553f, 0.235f, 1f);
                map.ToggleProvinceSurface(C,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 20000){
                Color newColor = new Color(0.996f,0.8f,0.36f, 1f);
                map.ToggleProvinceSurface(C,true,newColor);
            }
            else{
                Color newColor = new Color(1f,1f,0.7f, 1f);
                map.ToggleProvinceSurface(C,true,newColor);
            }
        }
        void heatMapGen(Country C)
        {
            if(C.attrib["COVID Cases"] > 5000000){
                Color newColor = new Color(0.75f, 0.0f, 0.15f, 1f);
                map.ToggleCountrySurface(C.name,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 3000000)
            {
                Color newColor = new Color(0.94f, 0.231f, 0.125f, 0.5f);
                map.ToggleCountrySurface(C.name,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 1000000)
            {
                Color newColor = new Color(0.992f, 0.553f, 0.235f, 1f);
                map.ToggleCountrySurface(C.name,true,newColor);
            }
            else if(C.attrib["COVID Cases"] > 500000){
                Color newColor = new Color(0.996f,0.8f,0.36f, 1f);
                map.ToggleCountrySurface(C.name,true,newColor);
            }
            else{
                Color newColor = new Color(1f,1f,0.7f, 1f);
                map.ToggleCountrySurface(C.name,true,newColor);
            }
        }
        // Changes scene to flat plane to view Data
        void GoToFlatPlane()
        {
            SceneManager.LoadScene(sceneName: "2D_Demo");
        }
    }
}
