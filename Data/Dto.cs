﻿using System;

namespace Weather.Data
{
    public class Dto
    {
        public DateTime Time { get; set; }

        public SHT30Result SHT30 { get; set; }

        public BMP280Result BMP280 { get; set; }

    }

    public class SHT30Result
    {
        public float Temperature { get; set; }

        public float RelativeHumidity { get; set; }
    }

    public class BMP280Result
    {
        public float Temperature { get; set; }

        public float Pressure { get; set; }

        public float Height { get; set; }
    }
}
