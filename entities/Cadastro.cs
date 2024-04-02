namespace luma.entities
{
  public class Cadastro
  {
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Token { get; set; }

    public Cadastro(string? username, string? email, string? token)
    {
      Username = username;
      Email = email;
      Token = token;
    }
  }
}