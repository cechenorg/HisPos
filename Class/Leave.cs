using System.Collections.Generic;

namespace His_Pos.Class
{
    public class Leave
	{
        //0 : 病假 , 1:喪假 , 2:產假, 3:特休, 4:調假, 5:婚假
        public Dictionary<string, int> LeaveDictionary = new Dictionary<string, int>
	    {
	        {"Sick", 0}, {"Funeral", 1},
	        {"Maternity", 2}, {"Annual", 3},
	        {"Adjust", 4},{"Marriage", 5}
        };
        public int[] Leaves = {0,0,0,0,0,0};
    }
}
