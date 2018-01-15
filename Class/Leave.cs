using System.Collections.Generic;

namespace His_Pos.Class
{
    public struct Leave
	{
	    public int[] Leaves { get; set; }

	    public Leave(int sick, int funeral, int maternity, int annual, int adjust, int marriage)
        {
            Leaves = new int[6]{0,0,0,0,0,0};
            Leaves[0] += sick;
            Leaves[1] += funeral;
            Leaves[2] += maternity;
            Leaves[3] += annual;
            Leaves[4] += adjust;
            Leaves[5] += marriage;
        }

	    public void SetLeave(int type,int date)
	    {
	        Leaves[type] += date;
        }
	}
}
