using System;
using System.Collections.Generic;
using TheWorld.Models;

namespace TheWorld.Interfaces
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
        IEnumerable<Trip> GetAllTripsWithStops();
        void AddTrip(Trip newTrip);
        bool SaveAll();
        Trip GetTripByName(string tripName, string username);
        void AddStop(string tripName, Stop newStop, string username);
        IEnumerable<Trip> GetUserTripsWithStops(string name);
    }
}