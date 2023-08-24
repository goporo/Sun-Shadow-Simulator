using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class SunController : MonoBehaviour
{
    public Transform sunTransform;
    public SunPositionCalculator SunPositionCalculator;
    private double SunAltitude;
    private double SunAzimuth;

    double latitude = 35.652832;
    double longitude = 139.839478;
    public int year = 2023;
    public int month = 8;
    public int day = 24;
    public int hour = 12;
    public int minute = 0;
    public int second = 0;
    public string utc = "9:00";

    void Start()
    {

    }
    void ExecuteSolarComputeExe()
    {
        string arguments = $"-y {year} -m {month} -d {day} -o {longitude} -a {latitude} -u {utc} -t {hour}:{minute}:{second}";

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
            UnityEngine.Debug.Log("Parsed Altitude: " + altitude);
            UnityEngine.Debug.Log("Parsed Azimuth: " + azimuth);

            // Store the values in your variables
            SunAltitude = altitude;
            SunAzimuth = azimuth;
        }
        else
        {
            UnityEngine.Debug.LogError("Parsing error.");
        }
    }



    void Update()
    {
        ExecuteSolarComputeExe();
        MoveSun(); // Call MoveSun to update the sun's rotation

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

        // float unitySunAltitude = 65f + 90f;
        // float unitySunAzimuth = 188f;


        // Calculate sun direction based on angles
        Vector3 sunDirection = CalculateSunDirection(unitySunAltitude, unitySunAzimuth);

        // Set the sun's rotation
        sunTransform.rotation = Quaternion.LookRotation(sunDirection, Vector3.up);
    }
}
