﻿using ChoETL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoJSONWriterTest
{
    class Program
    {
        public enum EmpType {  FullTime, Contract }

        static void Main(string[] args)
        {
            DataTableTest();
        }

        static void DataTableTest()
        {
            string connectionstring = @"Data Source=(localdb)\v11.0;Initial Catalog=TestDb;Integrated Security=True";
            using (var conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                var comm = new SqlCommand("SELECT * FROM Customers", conn);
                SqlDataAdapter adap = new SqlDataAdapter(comm);

                DataTable dt = new DataTable("Customer");
                adap.Fill(dt);

                using (var parser = new ChoJSONWriter("customers.json").Configure(c => c.UseJSONSerialization = true))
                    parser.Write(dt);
            }
        }

        static void DataReaderTest()
        {
            string connectionstring = @"Data Source=(localdb)\v11.0;Initial Catalog=TestDb;Integrated Security=True";
            using (var conn = new SqlConnection(connectionstring))
            {
                conn.Open();
                var comm = new SqlCommand("SELECT * FROM Customers", conn);
                using (var parser = new ChoJSONWriter("customers.json"))
                    parser.Write(comm.ExecuteReader());
            }
        }


        static void DynamicTest()
        {
            List<ExpandoObject> objs = new List<ExpandoObject>();
            dynamic rec1 = new ExpandoObject();
            rec1.Id = 1;
            rec1.Name = "Mark";
            rec1.Date = DateTime.Now;
            rec1.Active = true;
            rec1.Salary = new ChoCurrency(10.01);
            rec1.EmpType = EmpType.FullTime;
            rec1.Array = new int[] { 1, 2, 4 };
            rec1.Dict = new Dictionary<int, string>() { { 1, "xx" } };
            objs.Add(rec1);

            dynamic rec2 = new ExpandoObject();
            rec2.Id = 2;
            rec2.Name = "Jason";
            rec2.Date = DateTime.Now;
            rec2.Salary = new ChoCurrency(10.01);
            rec2.Active = false;
            rec2.EmpType = EmpType.Contract;
            rec2.Array = new string[] { "11", "12", "14" };
            rec2.Dict = new string[] { "11", "12", "14" };

            objs.Add(rec2);

            using (var w = new ChoJSONWriter("emp.json")
                )
            {
                w.Write(objs);

                //w.Write(ChoEnumerable.AsEnumerable(() =>
                //{
                //    return new { Address = new string[] { "NJ", "NY" }, Name = "Raj", Zip = "08837" };
                //}));
                //w.Write(new { Name = "Raj", Zip = "08837", Address = new { City = "New York", State = "NY" } });
            }

        }
    }
}
