using Microsoft.Data.SqlClient;
using System.Data;

 
    public class MockCustomerRepository : ICustomerRepository
    {
    DataTable mockDataTable = new DataTable("MockData");
        public MockCustomerRepository()
        {
            mockDataTable.Columns.Add("Id", typeof(string));
            mockDataTable.Columns.Add("Item", typeof(string));
            mockDataTable.Columns.Add("Price", typeof(double));
            mockDataTable.Columns.Add("TypeOfPayment", typeof(string));
            mockDataTable.Columns.Add("Date", typeof(DateOnly));

        for (int i = 1; i <= 5; i++)
        {
            DataRow newRow = mockDataTable.NewRow();
            newRow["Id"] = "A123";
            newRow["Item"] = "Item " + i;
            newRow["Price"] = 2000*i;
            newRow["TypeOfPayment"] = "Credit";
            newRow["Date"] = System.DateOnly.FromDateTime(DateTime.Now.AddDays(i));
            mockDataTable.Rows.Add(newRow);
        }
        for (int i = 1; i <= 5; i++)
        {
            DataRow newRow = mockDataTable.NewRow();
            newRow["Id"] = "B234";
            newRow["Item"] = "Item " + i;
            newRow["Price"] = 2000 * i;
            newRow["TypeOfPayment"] = "Credit";
            newRow["Date"] = System.DateOnly.FromDateTime(DateTime.Now.AddDays(i));
            mockDataTable.Rows.Add(newRow);
        }
        foreach (DataRow dataRow in mockDataTable.Rows)
        {
            foreach (var item in dataRow.ItemArray)
            {
                Console.WriteLine(item);
            }
        }

    }

    public DataTable GetCustomerInfo(string id)
    {
        DataTable resultTable = new DataTable();
        resultTable.Columns.Add("Item", typeof(string));
        resultTable.Columns.Add("Price", typeof(double));
        resultTable.Columns.Add("TypeOfPayment", typeof(string));
        resultTable.Columns.Add("Date", typeof(DateOnly));

        var query = from row in mockDataTable.AsEnumerable()
                    where row.Field<string>("Id") == id
                    select new
                    {
                        Item = row.Field<string>("Item"),
                        Price = row.Field<double>("Price"),
                        TypeOfPayment = row.Field<string>("TypeOfPayment"),
                        Date = row.Field<DateOnly>("Date")
                    };
        foreach (var row in query)
        {
            var newRow = resultTable.NewRow();
            newRow["Item"] = row.Item;
            newRow["Price"] = row.Price;
            newRow["TypeOfPayment"] = row.TypeOfPayment;newRow["Date"] = row.Date;
            resultTable.Rows.Add(newRow);
        }
        return resultTable;
    }

    public DataTable GetCustomerInfo(string id, DateOnly startDate, DateOnly endDate)
    {
        DataTable resultTable = new DataTable();
        resultTable.Columns.Add("Item", typeof(string));
        resultTable.Columns.Add("Price", typeof(double));
        resultTable.Columns.Add("TypeOfPayment", typeof(string));
        resultTable.Columns.Add("Date", typeof(DateOnly));

        var query = from row in mockDataTable.AsEnumerable()
                    where row.Field<string>("Id") == id
                          && row.Field<DateOnly>("Date") >= startDate
                          && row.Field<DateOnly>("Date") <= endDate
                    select new
                    {
                        Item = row.Field<string>("Item"),
                        Price = row.Field<double>("Price"),
                        TypeOfPayment = row.Field<string>("TypeOfPayment"),
                        Date = row.Field<DateOnly>("Date")
                    };

        foreach (var row in query)
        {
            var newRow = resultTable.NewRow();
            newRow["Item"] = row.Item;
            newRow["Price"] = row.Price;
            newRow["TypeOfPayment"] = row.TypeOfPayment;
            newRow["Date"] = row.Date;
            resultTable.Rows.Add(newRow);
        }

        return resultTable;
    }
    public DataTable GetCustomerInfoWithDate(string id, DateOnly startDate, DateOnly endDate)
    {
        var dt = new DataTable();
        using (var conn = new SqlConnection("...")) 
        {
            conn.Open();

            string startDateString = startDate.ToString("yyyy-MM-dd");
            string endDateString = endDate.ToString("yyyy-MM-dd");

            var sql = $"SELECT * FROM Customer WHERE id = '{id}' " 
                + $"AND DateColumn BETWEEN '{startDateString}' AND '{endDateString}'";

            using (var da = new SqlDataAdapter(sql, conn))
            {
                da.Fill(dt);
            }
        }
        return dt;
    }


}

