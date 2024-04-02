namespace luma.entities.services
{
  using System.Text;
  using System.Text.Json;
  using luma.entities;
  internal class CadastroService
  {
    public async Task<Cadastro> CadastrarUsuario(string username, string email)
    {
      HttpClient httpClient = new HttpClient();
      var user = new { username, email };
      var json = System.Text.Json.JsonSerializer.Serialize(user);
      var data = new StringContent(json, Encoding.UTF8, "application/json");
      var response = await httpClient.PostAsync("https://luma.lacuna.cc/api/start", data);

      response.EnsureSuccessStatusCode();
      var responseBody = await response.Content.ReadAsStringAsync();
      var jsonDocument = JsonDocument.Parse(responseBody);
      var root = jsonDocument.RootElement;
      var accessToken = root.GetProperty("accessToken").GetString();
      Cadastro cadastroNovo = new Cadastro(username, email, accessToken);
      return cadastroNovo;
    }

    
  }
}