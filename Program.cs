using luma.entities.services;
namespace luma;

class Program
{
  static async Task Main()
  {
    Console.WriteLine("Digite seu username: ");
    string? username = Console.ReadLine();
    if (username == null)
    {
      Console.WriteLine("username não pode ser nulo.");
      return;
    }

    Console.WriteLine("Digite seu email: ");
    string? email = Console.ReadLine();
    if (email == null)
    {
      Console.WriteLine("Email não pode ser nulo.");
      return;
    }

    try
    {
      var cadastroService = new CadastroService();
      var sondaService = new SondaService();
      var timeService = new TimeService();
      var jobService = new JobService();
      var cadastro = await cadastroService.CadastrarUsuario(username, email);
      if (cadastro.Token != null)
      {
        var sondas = await sondaService.ListarSonda(cadastro.Token);
        Console.WriteLine("Cadastro realizado com sucesso! Bem Vindo " + cadastro.Username);
        Console.WriteLine("Token: " + cadastro.Token);
        Console.WriteLine("Listando sondas...");

        foreach (var sonda in sondas)
        {
          if (sonda.Id != null && sonda.Encoding != null)
          {
            Console.WriteLine("Id: " + sonda.Id);
            Console.WriteLine("Name: " + sonda.Name);
            Console.WriteLine("Encoding: " + sonda.Encoding);
          }
        }
        Console.WriteLine("Fim da listagem de sondas.");

        var job = await jobService.ObterEmprego(cadastro.Token);
        while (job != null)
        {
          var sondaEncontrada = sondas.FirstOrDefault(sonda => sonda.Name == job.SondaName);
          if (sondaEncontrada != null && sondaEncontrada.Id != null && sondaEncontrada.Encoding != null)
          {
            var response = await timeService.SincronizarRelogio(cadastro.Token, sondaEncontrada.Id, sondaEncontrada.Encoding);

            if (response.T0.HasValue && response.T1.HasValue && response.T2.HasValue && response.T3.HasValue && job.Id != null)
            {
              long timeSet = timeOffSet(response.T0.Value, response.T1.Value, response.T2.Value, response.T3.Value);
              int roundTrip = (int)round(response.T0.Value, response.T1.Value, response.T2.Value, response.T3.Value);
              string probeNow = await timeService.VerificarDate(((DateTimeOffset.UtcNow.Ticks + timeSet).ToString()), sondaEncontrada.Encoding);
              string status = await jobService.EnviarEmprego(job.Id, cadastro.Token, probeNow, roundTrip);
              Console.WriteLine(status);
              Console.WriteLine("jobname: " + job.SondaName + " probename: " + sondaEncontrada.Name);
              switch (status)
              {
                case "Fail":
                  status = await jobService.EnviarEmprego(job.Id, cadastro.Token, probeNow, roundTrip);
                  break;
                case "Success":
                  job = await jobService.ObterEmprego(cadastro.Token);
                  break;
                case "Done":
                  job = null;
                  Console.WriteLine("Teste finalizado com sucesso! status: " + status);
                  break;
                default:
                  job = await jobService.ObterEmprego(cadastro.Token);
                  break;
              }
            }
            else
            {
              Console.WriteLine("T1 ou T2 é null");
            }
          }
        }
      }
    }
    catch (HttpRequestException e)
    {
      Console.WriteLine("Message :{0} ", e.Message);
    }
  }

  static public long round(long T0, long T1, long T2, long T3)
  {
    return ((T3 - T0) - (T2 - T1));
  }
  static public long timeOffSet(long T0, long T1, long T2, long T3)
  {
    return ((T1 - T0) + (T2 - T3)) / 2;
  }
}
