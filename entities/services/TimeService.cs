namespace luma.entities.services
{
  using System.Text.Json;
  using luma.entities;
  using System.Net.Http.Headers;
  internal class TimeService
  {
    public async Task<Time> SincronizarRelogio(string token, string id, string encondig)
    {
      HttpClient httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      long t0 = DateTimeOffset.UtcNow.Ticks;
      var response = await httpClient.PostAsync($"https://luma.lacuna.cc/api/probe/{id}/sync", null);
      long t3 = DateTimeOffset.UtcNow.Ticks;
      
      if (response.IsSuccessStatusCode)
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        var root = jsonDocument.RootElement;
        var t1 = root.GetProperty("t1").GetString();
        var t2 = root.GetProperty("t2").GetString();

        var time = new Time(encondig, t0, t1, t2, t3);

        return time;
      }
      else
      {
        throw new Exception($"Erro ao sincronizar relógio: {response.StatusCode}");
      }
    }

    public async Task<string> VerificarDate(string date, string encondig)
    {
      long data = long.Parse(date);
      byte[] bytes = BitConverter.GetBytes(data);
      switch (encondig)
      {
        case "Iso8601": 
          DateTime dateTime = new DateTime(data, DateTimeKind.Utc);
          return await Task.FromResult(dateTime.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz"));
        case "Ticks":
          return await Task.FromResult(date);
        case "TicksBinary":
          return await Task.FromResult(Convert.ToBase64String(bytes).ToString());
        case "TicksBinaryBigEndian":
          if (BitConverter.IsLittleEndian)
          {
              Array.Reverse(bytes);
          }
          return await Task.FromResult(Convert.ToBase64String(bytes).ToString());
        default:
          return await Task.FromResult("encondig não encontrado");
      }
    }
  }
}