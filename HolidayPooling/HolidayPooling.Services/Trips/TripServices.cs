using HolidayPooling.Services.Core;
using System;
using System.Collections.Generic;
using HolidayPooling.Models.Core;
using HolidayPooling.DataRepositories.Repository;
using System.Transactions;

namespace HolidayPooling.Services.Trips
{
    public class TripServices : BaseServices, ITripServices
    {


        #region Repository Dependencies

        private readonly ITripRepository _tripRepository;
        private readonly IPotRepository _potRepository;
        private readonly ITripParticipantRepository _participantRepository;
        private readonly IUserTripRepository _userTripRepository;
        private readonly IPotUserRepository _potUserRepository;

        #endregion

        #region .ctor

        public TripServices() 
            : base()
        {
            _tripRepository = new TripRepository();
            _potRepository = new PotRepository();
            _participantRepository = new TripParticipantRepository();
            _userTripRepository = new UserTripRepository();
            _potUserRepository = new PotUserRepository();
        }

        public TripServices(ITripRepository tripRepo, 
            IPotRepository potRepo, 
            ITripParticipantRepository participantRepo,
            IUserTripRepository userTripRepo,
            IPotUserRepository potUserRepo)
            :base()
        {
            _tripRepository = tripRepo;
            _potRepository = potRepo;
            _participantRepository = participantRepo;
            _userTripRepository = userTripRepo;
            _potUserRepository = potUserRepo;
        }

        #endregion

        #region ITripServices

        public void CreateTrip(Trip trip, int userId)
        {
            Errors.Clear();
            using(var scope = new TransactionScope())
            {
                try
                {

                    // 1 - Create Trip
                    _tripRepository.SaveTrip(trip);

                    if(_tripRepository.HasErrors)
                    {
                        MergeErrors(_tripRepository);
                        return;
                    }


                    // 2 - Create a pot
                    //TODO : Handle both pot mode when creating from trip
                    var pot = new Pot(-1, trip.Id, trip.Organizer, PotMode.Lead, 0.0, trip.Price, "Pot " + trip.TripName,
                                    trip.StartDate, trip.EndDate, trip.ValidityDate, trip.Description, false, string.Empty, null);
                    _potRepository.SavePot(pot);

                    if(_potRepository.HasErrors)
                    {
                        Errors.Add("Unable to create a pot associated to the trip");
                        MergeErrors(_potRepository);
                        return;
                    }

                    // 3 - add trip in organizer's trip
                    var userTrip = CreateUserTrip(trip, userId, true);
                    _userTripRepository.SaveUserTrip(userTrip);

                    if(_userTripRepository.HasErrors)
                    {
                        Errors.Add("Unable to add trip in user information");
                        MergeErrors(_userTripRepository);
                        return;
                    }

                    // 4 - add organizer as a participant
                    var tripParticipant = CreateParticipant(trip, trip.Organizer);
                    _participantRepository.SaveTripParticipant(tripParticipant);

                    if(_participantRepository.HasErrors)
                    {
                        Errors.Add("Unable to add organizer as a participant");
                        MergeErrors(_participantRepository);
                        return;
                    }

                    

                    // 5 - add organizer as a member of the pot
                    var potUser = CreatePotUser(trip, pot, userId);
                    _potUserRepository.SavePotUser(potUser);

                    if(_potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to ad organizer to the pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    trip.TripPot = pot;
                    trip.AddParticipant(tripParticipant);
                    pot.AddParticipant(potUser);

                    scope.Complete();
                }
                catch(Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public void DeleteTrip(Trip trip)
        {
            Errors.Clear();

            using(var scope = new TransactionScope())
            {
                try
                {
                    // 1 - Delete trip participants
                    foreach(var participant in trip.Participants)
                    {
                        _participantRepository.DeleteTripParticipant(participant);

                        if(_participantRepository.HasErrors)
                        {
                            Errors.Add("Unable to delete participants");
                            MergeErrors(_participantRepository);
                            return;
                        }
                    }

                    // 2 - Delete User trips
                    foreach(var userTrip in _userTripRepository.GetUserTripsByTrip(trip.TripName))
                    {
                        _userTripRepository.DeleteUserTrip(userTrip);
                        if(_userTripRepository.HasErrors)
                        {
                            Errors.Add("Unable to delete user's trip");
                            MergeErrors(_userTripRepository);
                            return;
                        }
                    }

                    // 3 - Delete PotUser
                    foreach(var potUser in trip.TripPot.Participants)
                    {
                        _potUserRepository.DeletePotUser(potUser);

                        if(_potUserRepository.HasErrors)
                        {
                            Errors.Add("Unable to delete pot from user information");
                            MergeErrors(_potUserRepository);
                            return;
                        }
                    }

                    // 4 - Delete pot
                    _potRepository.DeletePot(trip.TripPot);
                    if(_potRepository.HasErrors)
                    {
                        Errors.Add("Unable to delete pot");
                        MergeErrors(_potRepository);
                        return;
                    }

                    // 5 - Delete Trip
                    _tripRepository.DeleteTrip(trip);
                    if(_tripRepository.HasErrors)
                    {
                        MergeErrors(_tripRepository);
                        return;
                    }

                    scope.Complete();
                }
                catch(Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public IEnumerable<TripParticipant> GetParticipants(int tripId)
        {
            Errors.Clear();

            try
            {

                var list = _participantRepository.GetTripParticipants(tripId);
                if(_participantRepository.HasErrors)
                {
                    MergeErrors(_participantRepository);
                    return new List<TripParticipant>(0);
                }

                return list;

            }
            catch(Exception ex)
            {
                HandleException(ex);
                return new List<TripParticipant>(0);
            }
        }

        public void Participate(Trip trip, int userId, string userPseudo)
        {
            Errors.Clear();

            using(var scope = new TransactionScope())
            {
                try
                {
                    // 1 - Create trip participant
                    var participant = CreateParticipant(trip, userPseudo);
                    _participantRepository.SaveTripParticipant(participant);
                    if(_participantRepository.HasErrors)
                    {
                        Errors.Add("Unable to add user to trip participant");
                        MergeErrors(_participantRepository);
                        return;
                    }

                    // 2 - Create user trip
                    var userTrip = CreateUserTrip(trip, userId, false);
                    _userTripRepository.SaveUserTrip(userTrip);

                    if(_userTripRepository.HasErrors)
                    {
                        Errors.Add("Unable to add trip to user information");
                        MergeErrors(_userTripRepository);
                        return;
                    }

                    // 3 - Create pot user
                    var potUser = CreatePotUser(trip, trip.TripPot, userId);
                    _potUserRepository.SavePotUser(potUser);
                    if(_potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to add user to trip's pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    trip.AddParticipant(participant);
                    trip.TripPot.AddParticipant(potUser);
                    scope.Complete();

                }
                catch(Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public void Quit(Trip trip, int userId, string userPseudo)
        {
            //TODO : Handle Repayment
            //TODO : Handle Fees for us
            //TODO : No need to go in DB for PotUser and TripParticipant
            //TODO : Notification
            Errors.Clear();

            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1 - Delete trip participant
                    var participant = _participantRepository.GetTripParticipant(trip.Id, userPseudo);
                    if(participant == null || _participantRepository.HasErrors)
                    {
                        Errors.Add("Unable to find participant");
                        MergeErrors(_participantRepository);
                        return;
                    }

                    _participantRepository.DeleteTripParticipant(participant);
                    if(_participantRepository.HasErrors)
                    {
                        Errors.Add("Unable to delete participant");
                        MergeErrors(_participantRepository);
                        return;

                    }

                    // 2 - Delete user trip information
                    var userTrip = _userTripRepository.GetUserTrip(userId, trip.TripName);
                    if(userTrip == null || _userTripRepository.HasErrors)
                    {
                        Errors.Add("Unable to find user's trip");
                        MergeErrors(_userTripRepository);
                        return;
                    }

                    _userTripRepository.DeleteUserTrip(userTrip);
                    if(_userTripRepository.HasErrors)
                    {
                        Errors.Add("Unable to delete user's trip");
                        MergeErrors(_userTripRepository);
                        return;
                    }

                    // 3 - Delete pot user information
                    var potUser = _potUserRepository.GetPotUser(trip.TripPot.Id, userId);
                    if(potUser == null || _potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to find user in the trip's pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    _potUserRepository.DeletePotUser(potUser);
                    if(_potUserRepository.HasErrors)
                    {
                        Errors.Add("Unable to delete user from pot");
                        MergeErrors(_potUserRepository);
                        return;
                    }

                    // 4 - Update pot target amount if needed
                    if(potUser.Amount > 0)
                    {
                        trip.TripPot.CurrentAmount -= potUser.Amount;
                        _potRepository.UpdatePot(trip.TripPot);
                        if(_potRepository.HasErrors)
                        {
                            trip.TripPot.CurrentAmount += potUser.Amount;
                            Errors.Add("Unable to update trip's pot");
                            MergeErrors(_potRepository);
                            return;
                        }
                    }

                    trip.DeleteParticipant(participant);
                    trip.TripPot.DeleteParticipant(potUser);

                    scope.Complete();

                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public void UpdateAllowedNumberOfPeople(Trip trip, int newNumberOfPeople)
        {
            //TODO : Notification to user's for the change
            //TODO : Repayment for users is newNumberOfPeople is increasing
            //TODO : Update has payed flag if newNumberOfPeople is decreasing
            Errors.Clear();
            var oldNumberOfPeople = trip.NumberMaxOfPeople;
            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1 - Update User pot amotunt
                    foreach(var potUser in _potUserRepository.GetPotUsers(trip.TripPot.Id))
                    {
                        potUser.TargetAmount = trip.Price / newNumberOfPeople;
                        _potUserRepository.UpdatePotUser(potUser);

                        if(_potUserRepository.HasErrors)
                        {
                            Errors.Add("Unable to update pot user informations");
                            MergeErrors(_potUserRepository);
                            return;
                        }


                    }

                    // 2 - update trip
                    trip.NumberMaxOfPeople = newNumberOfPeople;
                    _tripRepository.UpdateTrip(trip);

                    if(_tripRepository.HasErrors)
                    {
                        trip.NumberMaxOfPeople = oldNumberOfPeople;
                        Errors.Add("Unable to update trip");
                        MergeErrors(_tripRepository);
                        return;
                        
                    }

                    scope.Complete();
                }
                catch(Exception ex)
                {
                    trip.NumberMaxOfPeople = oldNumberOfPeople;
                    HandleException(ex);
                }
            }
        }

        public void UpdatePrice(Trip trip, double newAmount)
        {
            //TODO : Notification to user's for the change
            //TODO : Repayment for users if newAmount is decreasing
            //TODO : Update has payed flag if newAmount is increasing
            Errors.Clear();

            using (var scope = new TransactionScope())
            {
                try
                {
                    // 1 - Update User pot amotunt
                    foreach (var potUser in _potUserRepository.GetPotUsers(trip.TripPot.Id))
                    {
                        potUser.TargetAmount = newAmount / trip.NumberMaxOfPeople;
                        _potUserRepository.UpdatePotUser(potUser);

                        if (_potUserRepository.HasErrors)
                        {
                            Errors.Add("Unable to update pot user informations");
                            MergeErrors(_potUserRepository);
                            return;
                        }


                    }

                    // 2 - Update User trip
                    foreach(var userTrip in _userTripRepository.GetUserTripsByTrip(trip.TripName))
                    {
                        userTrip.TripAmount = newAmount;
                        _userTripRepository.UpdateUserTrip(userTrip);
                        if(_userTripRepository.HasErrors)
                        {
                            Errors.Add("Unable to update user's trip's amount");
                            MergeErrors(_userTripRepository);
                            return;
                        }
                    }

                    // 3 - Update pot
                    trip.TripPot.TargetAmount = newAmount;
                    _potRepository.UpdatePot(trip.TripPot);
                    if(_potRepository.HasErrors)
                    {
                        trip.TripPot.TargetAmount = trip.Price;
                        Errors.Add("Unable to update pot");
                        MergeErrors(_potRepository);
                    }

                    // 4 - update trip
                    var oldPrice = trip.Price;
                    trip.Price = newAmount;
                    _tripRepository.UpdateTrip(trip);

                    if (_tripRepository.HasErrors)
                    {
                        trip.Price = oldPrice;
                        Errors.Add("Unable to update trip");
                        MergeErrors(_tripRepository);
                        return;

                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            }
        }

        public Trip GetTrip(int tripId)
        {
            Errors.Clear();

            try
            {
                // 1 - retrieving trip
                var trip = _tripRepository.GetTrip(tripId);

                if(trip == null || _tripRepository.HasErrors)
                {
                    Errors.Add("Unable to find trip");
                    MergeErrors(_tripRepository);
                    return null;
                }

                // 2 - retrieve participant
                var participants = _participantRepository.GetTripParticipants(trip.Id);

                if(_participantRepository.HasErrors)
                {
                    Errors.Add("Error while searching for trip's participant");
                    MergeErrors(_participantRepository);
                    return null;
                }

                foreach(var ptp in participants)
                {
                    trip.AddParticipant(ptp);
                }

                // 3 - retrieve pot
                var pot = _potRepository.GetTripPots(trip.Id);
                if(_potRepository.HasErrors)
                {
                    Errors.Add("Error while searching for trip's pot");
                    MergeErrors(_potRepository);
                    return null;
                }

                // 4 - retrieve pot participant
                var potUsers = _potUserRepository.GetPotUsers(pot.Id);
                if(_potUserRepository.HasErrors)
                {
                    Errors.Add("Error while searching for pot's user");
                    MergeErrors(_potUserRepository);
                    return null;
                }

                foreach(var potUser in potUsers)
                {
                    pot.AddParticipant(potUser);
                }

                trip.TripPot = pot;

                return trip;

            }
            catch (Exception ex)
            {
                HandleException(ex);
                return null;
            }
        }

        #endregion

        #region Methods

        private UserTrip CreateUserTrip(Trip trip, int userId, bool hasOrganized)
        {
            return new UserTrip(userId, trip.TripName, false, hasOrganized, 0.0, trip.Price);
        }

        private TripParticipant CreateParticipant(Trip trip, string userPseudo)
        {
            return new TripParticipant(trip.Id, userPseudo, false, 0.0, null);
        }

        private PotUser CreatePotUser(Trip trip, Pot pot, int userId)
        {
            return new PotUser(userId, pot.Id, false, 0.0, trip.Price / trip.NumberMaxOfPeople, false, string.Empty, false);
        }

        #endregion
    }
}
