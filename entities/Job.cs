namespace luma.entities
{
  public class Job
  {
    public string? Id { get; set; }
    public string? SondaName { get; set; }

    public Job(string? id, string? sondaName)
    {
      Id = id;
      SondaName = sondaName;
    }
  }
}