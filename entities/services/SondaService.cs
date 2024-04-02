namespace luma.entities.services
{
  using Newtonsoft.Json;
  using luma.entities;
  using System.Net.Http.Headers;
  internal class SondaService
  {
    public async Task<Sonda[]> ListarSonda(string token) 
    {
      HttpClient httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      var response = await httpClient.GetAsync("https://luma.lacuna.cc/api/probe");
      response.EnsureSuccessStatusCode();
      var responseBody = await response.Content.ReadAsStringAsync();
      var root = JsonConvert.DeserializeObject<Root>(responseBody);
      var sondas = root?.Probes;

      if (sondas != null && sondas.Length > 0)
      {
        return sondas;
      }
      else
      {
        throw new Exception("Nenhuma sonda encontrada.");
      }
    }
  }
}