using System;

namespace OvertakeAI
{
    public class OvertakeData
    {
        public static Library.Overtake GetData()
        {
            Library.Overtake.SetRandomAsRepeatable(false);
            var overtake = Library.Overtake.GetNextOvertake();
            return overtake;
        }
    }
}