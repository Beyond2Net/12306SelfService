using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class TrainTicket
    {
        protected static TrainTicket _train = null;
        public static TrainTicket GetCache()
        {
            if (_train == null)
            {
                _train = new TrainTicket();
            }
            return _train;
        }
        public enum SeatType
        {

        }
        public static DataView GetTrainTableList()
        {
            DataTable dt = new DataTable("Datas");
            dt.Columns.Add("ID", Type.GetType("System.Int32"));
            dt.Columns[0].AutoIncrement = true;
            dt.Columns[0].AutoIncrementSeed = 1;
            dt.Columns[0].AutoIncrementStep = 1;
            dt.Columns.Add("Product", Type.GetType("System.String"));
            dt.Columns.Add("Version", Type.GetType("System.String"));
            dt.Columns.Add("Description", Type.GetType("System.String"));

            dt.Rows.Add(new object[] { null, "a", "b", "c" });
            dt.Rows.Add(new object[] { null, "a", "b", "c" });
            dt.Rows.Add(new object[] { null, "a", "b", "c" });
            dt.Rows.Add(new object[] { null, "a", "b", "c" });
            dt.Rows.Add(new object[] { null, "a", "b", "c" });

            return dt.DefaultView;
        }
    }
}
