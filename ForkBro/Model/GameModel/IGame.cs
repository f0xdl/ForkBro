using ForkBro.Model;

public struct BookmakerEvent
{
    public int EventID;
    public readonly EBookmakers bookmaker;
    public EStatusEvent status;
}