namespace ForkBro.BookmakerModel.BaseEvents
{
    public enum BetGroup
    {
        None = 0,
        part_1 = 1,
        part_2 = 2,
        part_3 = 3,
        part_4 = 4,
        Over = 5,
        Under = 6,
        Cmd_A = 7,
        Cmd_B = 8,
        Draw = 9 //Ничья
    }
    public enum EventUnit : byte
    {
        None = 0,
        MainTime = 10,
        Set_1 = 1,
        Set_2 = 2,
        Set_3 = 3,
        Set_4 = 4,
        Set_5 = 5
    }
}