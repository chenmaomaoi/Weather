using Iot.Device.Bmxx80.ReadResult;

namespace Weather.Services
{
    public class DataDto
    {
        public SHT30Result SHT30 { get; set; }

        public Bmp280ReadResult BMP280 { get; set; }

        public class SHT30Result
        {
            public UnitsNet.Temperature Temperature { get; set; }

            public UnitsNet.RelativeHumidity RelativeHumidity { get; set; }
        }
    }
}
