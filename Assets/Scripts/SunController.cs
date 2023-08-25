using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SunController : MonoBehaviour
{
    public Transform sunTransform;
    public TMP_Text AltitudeText;
    public TMP_Text AzimuthText;
    public TMP_Text PillarHeightText;
    public TMP_Text ShadowLengthText;
    private double SunAltitude;
    private double SunAzimuth;

    string latitude = "35.652832";
    string longitude = "139.839478";
    string year = "2023";
    string month = "8";
    string day = "24";
    string hour = "123";
    string minute = "00";
    string second = "00";
    string utc = "9:00";

    public TMP_InputField latInput;
    public TMP_InputField lonInput;

    public TMP_InputField yearInput;
    public TMP_InputField monthInput;
    public TMP_InputField dayInput;
    public TMP_InputField hourInput;
    public TMP_InputField minuteInput;
    public TMP_InputField secondInput;
    public TMP_InputField utcInput;
    bool isSimulating = false;
    float PillarHeight = 2;
    private IEnumerator StartSimulation()
    {
        int startHour = int.Parse(hourInput.text);
        int startMinute = int.Parse(minuteInput.text);

        int targetHour = 23;  // Target hour value (e.g., 23 for end of the day)
        int targetMinute = 59; // Target minute value

        float animationDuration = 8.0f; // Duration of the animation in seconds
        float elapsedTime = 0.0f;

        while (elapsedTime < animationDuration)
        {
            float t = elapsedTime / animationDuration;

            int totalStartMinutes = startHour * 60 + startMinute;
            int totalTargetMinutes = targetHour * 60 + targetMinute;
            int animatedTotalMinutes = Mathf.RoundToInt(Mathf.Lerp(totalStartMinutes, totalTargetMinutes, t));

            int animatedHour = animatedTotalMinutes / 60;
            int animatedMinute = animatedTotalMinutes % 60;

            hourInput.text = animatedHour.ToString("00");
            minuteInput.text = animatedMinute.ToString("00");

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        hourInput.text = targetHour.ToString("00");
        minuteInput.text = targetMinute.ToString("00");

        isSimulating = false;
    }

    public void ToggleSimulation()
    {
        if (isSimulating)
        {
            StopAllCoroutines();
            isSimulating = false;
        }
        else
        {
            hourInput.text = "00";
            minuteInput.text = "00";
            secondInput.text = "00";
            StartCoroutine(StartSimulation());
            isSimulating = true;
        }
    }

    void ExecuteSolarComputeExe()
    {
        string formattedTime = hour + ":" + minute + ":" + second;
        string arguments = $"-y {year} -m {month} -d {day} -o {longitude} -a {latitude} -u {utc} -t {formattedTime}";

        Process process = new Process();
        process.StartInfo.FileName = "./solar-position-calculator";
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        // UnityEngine.Debug.Log(output);

        // Split output into altitude and azimuth
        string[] parts = output.Split(' ');

        if (parts.Length == 2 && double.TryParse(parts[0], out double altitude) && double.TryParse(parts[1], out double azimuth))
        {
            // Store the values in your variables
            SunAltitude = altitude;
            SunAzimuth = azimuth;
        }
        else
        {
            UnityEngine.Debug.LogError("Parsing error.");
        }
    }

    void Start()
    {

    }
    void Update()
    {
        UpdateSolarDataFromUI();
        UpdateUI();
    }
    void UpdateUI()
    {
        AltitudeText.text = SunAltitude.ToString("0.00") + "°";
        AzimuthText.text = SunAzimuth.ToString("0.00") + "°";
        PillarHeightText.text = PillarHeight.ToString("0.00") + "m";

        double shadowLength = Mathf.Max(0, PillarHeight / Mathf.Tan((float)SunAltitude * Mathf.Deg2Rad));
        ShadowLengthText.text = shadowLength.ToString("0.00") + "m";
    }

    public void UpdateSolarDataFromUI()
    {
        latitude = latInput.text;
        longitude = lonInput.text;

        year = yearInput.text;
        month = monthInput.text;
        day = dayInput.text;
        hour = hourInput.text;
        minute = minuteInput.text;
        second = secondInput.text;
        utc = utcInput.text;

        ExecuteSolarComputeExe();
        MoveSun();
    }

    private Vector3 CalculateSunDirection(double altitude, double azimuth)
    {
        float x = Mathf.Cos((float)(azimuth * Mathf.Deg2Rad)) * Mathf.Sin((float)(altitude * Mathf.Deg2Rad));
        float y = Mathf.Cos((float)(altitude * Mathf.Deg2Rad));
        float z = Mathf.Sin((float)(azimuth * Mathf.Deg2Rad)) * Mathf.Sin((float)(altitude * Mathf.Deg2Rad));
        return new Vector3(x, y, z);
    }

    private void MoveSun()
    {
        // UnityEngine.Debug.Log(SunAltitude + " " + SunAzimuth);

        // Convert angles to Unity's coordinate system
        float unitySunAltitude = (float)SunAltitude + 90f;
        float unitySunAzimuth = (float)SunAzimuth;


        // Calculate sun direction based on angles
        Vector3 sunDirection = CalculateSunDirection(unitySunAltitude, unitySunAzimuth);

        // Set the sun's rotation
        sunTransform.rotation = Quaternion.LookRotation(sunDirection, Vector3.up);
    }
}
