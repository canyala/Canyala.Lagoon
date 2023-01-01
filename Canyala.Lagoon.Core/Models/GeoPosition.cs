//-------------------------------------------------------------------------------
//
//  MIT License
//
//  Copyright (c) 2012-2022 Canyala Innovation (Martin Fredriksson)
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in all
//  copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  SOFTWARE.
//
//------------------------------------------------------------------------------- 

using System.Text;

namespace Canyala.Lagoon.Models;

public enum LatitudeDirection { Empty, North, South };
public enum LongitudeDirection { Empty, East, West };

public class GeoPosition
{
    public readonly double Latitude;
    public readonly double Longitude;

    internal GeoPosition(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static GeoPosition FromDegreesDecimal(double latitude, double longitude)
    { return new GeoPosition(latitude, longitude); }

    public static GeoPosition FromDegreesMinutes(double latitude, double longitude)
    {
        var latitudeDegrees = Math.Floor(latitude);
        var latitudeFractionalMinutes = (latitude - latitudeDegrees) / 60;

        var longitudeDegrees = Math.Floor(longitude);
        var longitudeFractionalMinutes = (longitude - longitudeDegrees) / 60;

        return new GeoPosition(latitudeDegrees + latitudeFractionalMinutes, longitudeDegrees + longitudeFractionalMinutes);
    }

    public static GeoPosition Empty = new GeoPosition(Double.NaN, Double.NaN);
    public static GeoPosition[] EmptyArray = new GeoPosition[0];

    public override bool Equals(object? obj)
    {
        if (obj is not GeoPosition geoPosition)
            return false;

        if (Latitude != geoPosition.Latitude)
            return false;

        if (Longitude != geoPosition.Longitude)
            return false;

        return true;
    }

    public override int GetHashCode()
    {
        return Latitude.GetHashCode() ^ Longitude.GetHashCode();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        var latitudeDirection = String.Empty;
        var longitudeDirection = String.Empty;

        if (Latitude < 0)
            latitudeDirection = "S";
        if (Latitude > 0)
            latitudeDirection = "N";
        if (Longitude < 0)
            longitudeDirection = "W";
        if (Longitude > 0)
            longitudeDirection = "E";

        var latitudeDecimal = Math.Abs(Latitude);
        var latitudeDegrees = Math.Floor(latitudeDecimal);
        var latitudeDecimalMinutes = (latitudeDecimal - latitudeDegrees) * 60;
        var latitudeMinutes = Math.Floor(latitudeDecimalMinutes);
        var latitudeSeconds = (latitudeDecimalMinutes - latitudeMinutes) * 60;

        var longitudeDecimal = Math.Abs(Longitude);
        var longitudeDegrees = Math.Floor(longitudeDecimal);
        var longitudeDecimalMinutes = (longitudeDecimal - longitudeDegrees) * 60;
        var longitudeMinutes = Math.Floor(longitudeDecimalMinutes);
        var longitudeSeconds = (longitudeDecimalMinutes - longitudeMinutes) * 60;

        builder.AppendFormat("{0}°{1}'{2}\"{3} {4}°{5}'{6}\"{7}", latitudeDegrees, latitudeMinutes, latitudeSeconds, latitudeDirection
            , longitudeDegrees, longitudeMinutes, longitudeSeconds, longitudeDirection);

        return builder.ToString();
    }

    public static GeoPosition Parse(string text)
    {
        throw new NotImplementedException();
    }
}
