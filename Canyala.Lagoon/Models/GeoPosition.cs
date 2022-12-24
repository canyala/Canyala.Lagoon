//
// Copyright (c) 2013 Canyala Innovation AB
//
// All rights reserved.
//

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Canyala.Lagoon.Models
{
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

        public override bool Equals(object obj)
        {
            GeoPosition geoPosition = obj as GeoPosition;

            if (geoPosition == null)
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
}
