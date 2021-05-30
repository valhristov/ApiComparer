using System.Linq;

namespace OmsApiComparer.WPF
{
    public class MultiIndustryValue<T>
    {
        public string Name { get; }

        public T BEER { get; }
        public T BICYCLE { get; }
        public T LIGHT { get; }
        public T LP { get; }
        public T MILK { get; }
        public T NCP { get; }
        public T OTP { get; }
        public T PERFUM { get; }
        public T PHARMA { get; }
        public T PHOTO { get; }
        public T SHOES { get; }
        public T TIRES { get; }
        public T TOBACCO { get; }
        public T WATER { get; }
        public T WHEELCHAIRS { get; }

        public MultiIndustryValue(IGrouping<string, (string Industry, T Value)> source)
        {
            Name = source.Key;

            BEER = source.Where(p => p.Industry == "beer").Select(p => p.Value).FirstOrDefault();
            BICYCLE = source.Where(p => p.Industry == "bicycle").Select(p => p.Value).FirstOrDefault();
            LIGHT = source.Where(p => p.Industry == "light").Select(p => p.Value).FirstOrDefault();
            LP = source.Where(p => p.Industry == "lp").Select(p => p.Value).FirstOrDefault();
            MILK = source.Where(p => p.Industry == "milk").Select(p => p.Value).FirstOrDefault();
            NCP = source.Where(p => p.Industry == "ncp").Select(p => p.Value).FirstOrDefault();
            OTP = source.Where(p => p.Industry == "otp").Select(p => p.Value).FirstOrDefault();
            PERFUM = source.Where(p => p.Industry == "perfum").Select(p => p.Value).FirstOrDefault();
            PHARMA = source.Where(p => p.Industry == "pharma").Select(p => p.Value).FirstOrDefault();
            PHOTO = source.Where(p => p.Industry == "photo").Select(p => p.Value).FirstOrDefault();
            SHOES = source.Where(p => p.Industry == "shoes").Select(p => p.Value).FirstOrDefault();
            TIRES = source.Where(p => p.Industry == "tires").Select(p => p.Value).FirstOrDefault();
            TOBACCO = source.Where(p => p.Industry == "tobacco").Select(p => p.Value).FirstOrDefault();
            WATER = source.Where(p => p.Industry == "water").Select(p => p.Value).FirstOrDefault();
            WHEELCHAIRS = source.Where(p => p.Industry == "wheelchairs").Select(p => p.Value).FirstOrDefault();
        }
    }
}
