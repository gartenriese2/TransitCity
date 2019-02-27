namespace WpfTestApp
{
    using CitySimulation;

    public class DistrictViewModel
    {
        private IDistrict _district;

        public DistrictViewModel(IDistrict district)
        {
            _district = district;
        }

        public string Name => _district.Name;

        public int ResidentCount => _district.Residents.Count;

        public int JobCount => _district.Jobs.Count;
    }
}
