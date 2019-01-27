using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dialogowe_pkp
{
    class DbConnector {
        private DataSet ds = new DataSet();
        private DataTable dt = new DataTable();

        private List<StationTuple> stationTupleList;
        private List<SeatsQuantity> seatsQuantityList;
        private List<Hour> hourList;

        private string server;
        private string port;
        private string login;
        private string password;
        private string database;

        public List<StationTuple> getStationTuples()
        {
            return stationTupleList;
        }

        public List<SeatsQuantity> GetSeatsQuantities()
        {
            return seatsQuantityList;
        }

            public List<Hour> getHours()
        {
            return hourList;
        }

        public DbConnector()
        {
            this.server = "127.0.0.1";
            this.port = "5432";
            this.login = "postgres";
            this.password = "postgres";
            this.database = "postgres";

            stationTupleList = new List<StationTuple>();
            seatsQuantityList = new List<SeatsQuantity>();
            hourList = new List<Hour>();
        }

        public void saveOrderToDatabase(Order order)
        {
            string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", this.server, this.port, this.login, this.password, this.database);

            NpgsqlConnection conn = new NpgsqlConnection(connstring);
            NpgsqlCommand cmd = null;

            try
            {
                string sql = "INSERT INTO public.Orders (ffrom, tto, hour, seats) VALUES ( @ffrom, @tto, @hour, @seats)";

                conn.Open();
                cmd = new NpgsqlCommand(sql, conn);

                NpgsqlParameter parameter = new NpgsqlParameter("@ffrom", NpgsqlDbType.Varchar);
                parameter.Value = order.From;
                cmd.Parameters.Add(parameter);

                NpgsqlParameter parameter3 = new NpgsqlParameter("@tto", NpgsqlDbType.Varchar);
                parameter3.Value = order.To;
                cmd.Parameters.Add(parameter3);

                NpgsqlParameter parameter1 = new NpgsqlParameter("@hour", NpgsqlDbType.Varchar);
                parameter1.Value = order.Hour;
                cmd.Parameters.Add(parameter1);

                NpgsqlParameter parameter2 = new NpgsqlParameter("@seats", NpgsqlDbType.Integer);
                parameter2.Value = order.Quantity;
                cmd.Parameters.Add(parameter2);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (cmd != null) cmd.Dispose();
                if (conn != null) conn.Close();
            }
        }
        /*public void retrieve()
        {
            try
            {
                // PostgeSQL-style connection string
                string connstring = String.Format("Server={0};Port={1};User Id={2};Password={3};Database={4};", this.server, this.port, this.login, this.password, this.database);
                // Making connection with Npgsql provider
                NpgsqlConnection conn = new NpgsqlConnection(connstring);
                conn.Open();
                // quite complex sql statement
                string sql = "SELECT * FROM public.Movies";
                // data adapter making request from our connection
                NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since it C# DataSet can handle multiple tables, we will select first
                dt = ds.Tables[0];


                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    moviesList.Add(new Movie { Title = dt.Rows[j].ItemArray[1].ToString() });
                }

                sql = "SELECT * FROM public.Hours";
                da = new NpgsqlDataAdapter(sql, conn);
                // i always reset DataSet before i do
                // something with it.... i don't know why :-)
                ds.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                da.Fill(ds);
                // since it C# DataSet can handle multiple tables, we will select first
                dt = ds.Tables[0];

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    hourList.Add(new Hour { Value = dt.Rows[j].ItemArray[1].ToString() });
                }

                // connect grid to DataTable
                //dataGridView1.DataSource = dt;dt.Rows[j].ItemArray[1]
                // since we only showing the result we don't need connection anymore
                conn.Close();
            }
            catch (Exception msg)
            {
                // something went wrong, and you wanna know why
                //MessageBox.Show(msg.ToString());
                throw;
            }
        }*/
    }
}
