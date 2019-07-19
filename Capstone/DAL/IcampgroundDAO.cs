using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public interface IcampgroundDAO
    {
        IList<Parks> GetParks();

        IList<Campgrounds> GetCampgroundsById(int parkId);

        IList<Site> GetAvailSite(int campId, DateTime to, DateTime from);

        void insertUserConnection(int UserId, int ReservationId);

        int insertReservation(Reservation newRes);

        void TransactionReservationNumber(Reservation newRes, int UserId, int ReservationId);
    }
}
