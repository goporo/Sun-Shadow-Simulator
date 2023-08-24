using UnityEngine;

public class SunPositionCalculator : MonoBehaviour
{
    public static (double, double) CalculateSunPosition(double latitude, double longitude, int year, int month, int day, int hour, int minute, int second)
    {



        // Convert date and time to Julian date
        double jd = CalculateJulianDate(year, month, day, hour, minute, second);

        // Calculate solar position
        (double altitude, double azimuth) = CalculateAltitudeAzimuth(latitude, longitude, jd);

        return (altitude, azimuth);
    }

    private static double CalculateJulianDate(int year, int month, int day, int hour, int minute, int second)
    {
        System.DateTime dateTime = new System.DateTime(year, month, day, hour, minute, second);
        System.DateTime dateTime2000 = new System.DateTime(2000, 1, 1, 12, 0, 0);
        double daysSince2000 = (dateTime - dateTime2000).TotalDays;
        double julianDate2000 = 2451545.0;
        double julianDate = julianDate2000 + daysSince2000;

        return julianDate;
    }

    private static (double, double) CalculateAltitudeAzimuth(double latitude, double longitude, double jd)
    {
        double timeZone = 0.0; // You can adjust this based on your local timezone
        double latitudeRad = Mathf.Deg2Rad * (float)latitude;
        double longitudeRad = Mathf.Deg2Rad * (float)longitude;

        double hourAngle = CalculateHourAngle(jd, longitudeRad, timeZone);
        double declination = CalculateSunDeclination(jd);

        double altitudeRad = Mathf.Asin((float)(Mathf.Sin((float)latitudeRad) * Mathf.Sin((float)declination)
            + Mathf.Cos((float)latitudeRad) * Mathf.Cos((float)declination) * Mathf.Cos((float)hourAngle)));

        double azimuthRad = Mathf.Atan2((float)-Mathf.Cos((float)declination) * Mathf.Sin((float)hourAngle),
            (float)Mathf.Sin((float)declination) - Mathf.Sin((float)latitudeRad) * Mathf.Sin((float)altitudeRad));

        double altitude = Mathf.Rad2Deg * (float)altitudeRad;
        double azimuth = Mathf.Rad2Deg * (float)azimuthRad;

        return (altitude, azimuth);
    }

    private static double CalculateHourAngle(double jd, double longitudeRad, double timeZone)
    {
        double t = (jd - 2451545.0) / 36525.0;
        double meanSolarTime = 280.46061837 + 360.98564736629 * (jd - 2451545.0) + t * t * (0.000387933 - t / 38710000.0);
        double meanSiderealTime = meanSolarTime - 360.0 * timeZone;
        double hourAngle = meanSiderealTime - 180.0 - longitudeRad;

        while (hourAngle < -180.0)
        {
            hourAngle += 360.0;
        }

        while (hourAngle > 180.0)
        {
            hourAngle -= 360.0;
        }

        return hourAngle;
    }

    private static double CalculateSunDeclination(double jd)
    {
        double t = (jd - 2451545.0) / 36525.0;
        double epsilon = 23.439292 - t * (46.815 + t * (0.00059 - t * 0.001813));
        double declinationRad = Mathf.Deg2Rad * (float)epsilon;

        return declinationRad;
    }
}