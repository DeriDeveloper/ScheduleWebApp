namespace ScheduleWebApp.Types
{
    internal class CellCoordinates
    {

        public Coord StartCoord { get; set; }
        public Coord EndCoord { get; set; }

        public class Coord
        {
            public int Row { get; set; }
            public int Column { get; set; }
            public string AddressRow { get; set; }
            public string AddressColumn { get; set; }
        }
    }
}