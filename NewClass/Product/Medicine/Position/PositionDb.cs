﻿using System.Data;

namespace His_Pos.NewClass.Product.Medicine.Position
{
    public static class PositionDb
    {
        public static DataTable GetData()
        {
            return MainWindow.ServerConnection.ExecuteProc("[Get].[Position]");
        }
    }
}
