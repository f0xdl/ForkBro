using ForkBro.Model;

public interface IBookmakerEvent
{
    public int EventID { get; set; }
    public EStatusEvent status { get; set; }
}