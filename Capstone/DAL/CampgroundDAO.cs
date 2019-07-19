using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Capstone.Classes;
using System.Transactions;

namespace Capstone.DAL
{
    public class CampgroundDAO : IcampgroundDAO
    {
        private string connectionString;
        private const string _getLastIdSQL = "SELECT CAST(SCOPE_IDENTITY() as int);";


        // Single Parameter Constructor
        public CampgroundDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        #region Parks
        public IList<Parks> GetParks()
        {
            List<Parks> result = new List<Parks>();
            const string sql = "SELECT * FROM dbo.park;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Parks park = GetParksFromReader(reader);
                    result.Add(park);
                }
            }
            return result;
        }
        private Parks GetParksFromReader(SqlDataReader reader)
        {
            Parks item = new Parks();
            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.Name = Convert.ToString(reader["name"]);
            item.Location = Convert.ToString(reader["location"]);
            item.EstablishDate = Convert.ToDateTime(reader["establish_date"]);
            item.Area = Convert.ToInt32(reader["area"]);
            item.Visitors = Convert.ToInt32(reader["visitors"]);
            item.Description = Convert.ToString(reader["description"]);
            return item;
        }

        #endregion

        #region Campgrounds
        public IList<Campgrounds> GetCampgroundsById(int parkId)
        {
            List<Campgrounds> result = new List<Campgrounds>();
            const string sql = "SELECT * FROM dbo.campground WHERE park_id = @ID;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ID", parkId);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Campgrounds camp = GetCampFromReader(reader);
                    result.Add(camp);
                }
            }
            return result;
        }
        private Campgrounds GetCampFromReader(SqlDataReader reader)
        {
            Campgrounds item = new Campgrounds();
            item.CampGroundId = Convert.ToInt32(reader["campground_id"]);
            item.ParkId = Convert.ToInt32(reader["park_id"]);
            item.Name = Convert.ToString(reader["name"]);
            item.OpenFromMM = Convert.ToInt32(reader["open_from_mm"]);
            item.OpenToMM = Convert.ToInt32(reader["open_to_mm"]);
            item.DailyFee = Convert.ToDecimal(reader["daily_fee"]);
            return item;
        }
        #endregion

        #region Site
        public IList<Site> GetAvailSite(int campId, DateTime from, DateTime to)
        {
            List<Site> result = new List<Site>();
            try
            {
                const string sql = "Select Top 5 * From [site] " +
                                   "Where site_id Not In(Select site.site_id From [site] " +
                                    "Join reservation On reservation.site_id = site.site_id " +
                                    "Where (Not (reservation.to_date < @fromDate Or reservation.from_date > @toDate))) " +
                                                "And campground_id = @CampId " +
                                                "Order By site_number;";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@campId", campId);
                    cmd.Parameters.AddWithValue("@toDate", to);
                    cmd.Parameters.AddWithValue("@fromDate", from);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Site avail = GetSiteFromReader(reader);
                        result.Add(avail);
                    }                    
                }              
            }
            catch (Exception e)
            {
                throw new Exception();
            }
            return result;
        }
        private Site GetSiteFromReader(SqlDataReader reader)
        {
            Site item = new Site();
            item.SiteId = Convert.ToInt32(reader["site_id"]);
            item.CampgroundId = Convert.ToInt32(reader["campground_id"]);
            item.SiteNumber = Convert.ToInt32(reader["site_number"]);
            item.MaxOccupy = Convert.ToInt32(reader["max_occupancy"]);
            item.MaxRvLength = Convert.ToInt32(reader["max_rv_length"]);
            item.Utilities = Convert.ToBoolean(reader["utilities"]);
            item.Acessible = Convert.ToBoolean(reader["accessible"]);
            return item;
        }
        #endregion

        #region Reservation

        public int insertReservation(Reservation newRes)
        {
            const string sql = "INSERT reservation (site_id, name, from_date, to_date, create_date) " +
                        "VALUES (@siteId, @name, @fromDate, @toDate, @createDate);";
          

            // Connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Parameretize query
                SqlCommand cmd = new SqlCommand(sql + _getLastIdSQL, conn);
                cmd.Parameters.AddWithValue("@siteId", newRes.SiteId);
                cmd.Parameters.AddWithValue("@name", newRes.Name);
                cmd.Parameters.AddWithValue("@fromDate", newRes.FromDate);
                cmd.Parameters.AddWithValue("@toDate", newRes.ToDate);
                cmd.Parameters.AddWithValue("@createDate", DateTime.Now);

                // Execute SQL command
                newRes.ReservationId = (int)cmd.ExecuteScalar();
            }

            return newRes.ReservationId;
        }

        public void insertUserConnection(int UserId, int ReservationId)
        {
            const string sql = "INSERT User_Reservation (UserId, reservation_id) " +
                        "VALUES (@UserId, @ReserveId);";

            // Connect to the database
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Parameretize query
                SqlCommand cmd = new SqlCommand(sql + _getLastIdSQL, conn);
                cmd.Parameters.AddWithValue("@UserId", UserId);
                cmd.Parameters.AddWithValue("@ReserveId", ReservationId);
            }
            
        }

        public void TransactionReservationNumber(Reservation newRes, int UserId, int ReservationId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                newRes.ReservationId = insertReservation(newRes);
                insertUserConnection(UserId, newRes.ReservationId);
                scope.Complete();
            }
        }



        private User GetUserIdReader(SqlDataReader userReader)
        {
            User item = new User();
            item.ReservationId = Convert.ToInt32(userReader["ReservationId"]);
            item.UserId = Convert.ToInt32(userReader["UserId"]);
            
            return item;
        }

        private Reservation GetResFromReader(SqlDataReader resReader)
        {
            Reservation item = new Reservation();
            item.ReservationId = Convert.ToInt32(resReader["reservation_id"]);
            item.SiteId = Convert.ToInt32(resReader["site_id"]);
            item.Name = Convert.ToString(resReader["name"]);
            item.FromDate = Convert.ToDateTime(resReader["from_date"]);
            item.ToDate = Convert.ToDateTime(resReader["to_date"]);
            item.CreateDate = Convert.ToDateTime(resReader["create_date"]);
            return item;
        }



        #endregion  
    }





}
