namespace luma.entities.services
{
  using System.Text;
  using System.Text.Json;
  using luma.entities;
  using System.Net.Http.Headers;
  internal class JobService
  {
    public async Task<Job?> ObterEmprego(string token)
    {
      HttpClient httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      var response = await httpClient.PostAsync($"https://luma.lacuna.cc/api/job/take", null);
      if (response.IsSuccessStatusCode)
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        var root = jsonDocument.RootElement;
        var element = root.GetProperty("job");
        var jobID = element.GetProperty("id").GetString();
        var sondaName = element.GetProperty("probeName").GetString();
        var job = new Job(jobID, sondaName);
        return job;
      }
      else
      {
        return null;
      }
    }

    public async Task<string> EnviarEmprego(string jobId, string token, string probeNow, int roundTrip)
    {
      HttpClient httpClient = new HttpClient();
      httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      var dados = new { probeNow, roundTrip };
      var json = System.Text.Json.JsonSerializer.Serialize(dados);
      var data = new StringContent(json, Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync($"https://luma.lacuna.cc/api/job/{jobId}/check", data);
      if (!response.IsSuccessStatusCode)
      {
        throw new Exception($"Erro ao enviar emprego: {response.StatusCode}");
      }
      else
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(responseBody);
        var root = jsonDocument.RootElement;
        var status = root.GetProperty("code").GetString();
        if (status != null)
        {
          return status;
        }
        else
        {
          throw new Exception($"status code não encontrado: {status}");
        }
      }
    }
  }
}