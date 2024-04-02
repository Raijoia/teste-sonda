namespace luma.entities
{
  public class Time
  {
    public string? Encondig { get; set; }
    public long? T0 { get; set; }
    public long? T1 { get; set; }
    public long? T2 { get; set; }
    public long? T3 { get; set; }

    public Time(string? encondig, long? t0, string? t1, string? t2, long? t3)
    {
      Encondig = encondig;
      T0 = t0;
      T3 = t3;
      if (t1 != null && t2 != null) {
        switch (Encondig) 
        {
          case "Iso8601": 
            DateTimeOffset dateTimeOffset1 = DateTimeOffset.Parse(t1);
            DateTime dateTimeUtc1 = dateTimeOffset1.UtcDateTime;
            DateTimeOffset dateTimeOffset2 = DateTimeOffset.Parse(t2);
            DateTime dateTimeUtc2 = dateTimeOffset2.UtcDateTime;
            T1 = dateTimeUtc1.Ticks;
            T2 = dateTimeUtc2.Ticks;
            break;
          case "Ticks":
            T1 = long.Parse(t1);
            T2 = long.Parse(t2);
            break;
          case "TicksBinary":
            byte[] bytes1 = Convert.FromBase64String(t1.ToString());
            byte[] bytes2 = Convert.FromBase64String(t2.ToString());
            T1 = BitConverter.ToInt64(bytes1, 0);
            T2 = BitConverter.ToInt64(bytes2, 0);
            break;
          case "TicksBinaryBigEndian":
            byte[] bytes3 = Convert.FromBase64String(t1.ToString());
            byte[] bytes4 = Convert.FromBase64String(t2.ToString());
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes3);
                Array.Reverse(bytes4);
            }
            T1 = BitConverter.ToInt64(bytes3, 0);
            T2 = BitConverter.ToInt64(bytes4, 0);
            break;
        }
      }
    }
  }
}