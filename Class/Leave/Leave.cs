using System.Collections.Generic;
using System.Data;

namespace His_Pos.Class.Leave
{
    public class Leave
	{
	    public Leave(DataRow dataRow)
	    {
	        Id = dataRow["EMPLEVTYP_ID"].ToString();
	        Name = dataRow["EMPLEVTYP_NAME"].ToString();
        }

	    public string Id { get; set; }
	    public string Name { get; set; }
    }
}
