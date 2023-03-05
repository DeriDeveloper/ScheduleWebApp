using LibrarySchedule.Models;

namespace ScheduleWebApp.Types
{
    internal class CellCoordinatesForGroup : Types.CellCoordinates
    {
        internal Group Group { get; set; }
    }
}
