namespace olx_api.Repositories
{
    public interface ILocationRepository
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<IEnumerable<State>> GetStatesByCountryAsync(int countryId);
        Task<IEnumerable<City>> GetCitiesByStateAsync(int stateId);
    }
}