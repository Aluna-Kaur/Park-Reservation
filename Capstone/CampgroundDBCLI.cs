using Capstone.DAL;
using Microsoft.Extensions.Configuration;
using Security.BusinessLogic;
using Security.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Capstone
{
    public class CampgroundDBCLI
    {
        private const string Option_Login = "1";
        private const string Option_Register = "2";
        private const string Option_Logout = "2";
        private const string Option_Quit = "q";
        private const string View_Campgrounds = "1";
        private const string Search_Avail_Reserv = "m";
        private const string CamproundDBMenu = "1";
        private const string Return_Option = "r";
        private UserManager _userMgr = null;
        private string _connection = "";
        private IcampgroundDAO _cG = null;
        protected string submenu = "p";


        public CampgroundDBCLI(UserManager userManager, string connectionstring)
        {
            _userMgr = userManager;
            _connection = connectionstring;
            _cG = new CampgroundDAO(connectionstring);
        }

        public void Run()
        {
            MainMenu();
        }

        private void MainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                AsciiTitle();
                if (_userMgr.IsAuthenticated)
                {
                    Console.Clear();
                    AsciiHouse();
                    Console.WriteLine();
                    Console.WriteLine(" (1) Seach Campground Database");
                    Console.WriteLine(" (2) Logout");           
                }
                else
                {
                    Console.WriteLine(" (1) Login");
                    Console.WriteLine(" (2) Register");
                    Console.WriteLine(" (Q) Quit");
                }
                Console.Write(" Please make a choice: ");

                string choice = Console.ReadLine().ToLower();

                Console.WriteLine();

                if (_userMgr.IsAuthenticated)
                {
                    if (choice == Option_Logout)
                    {
                        _userMgr.LogoutUser();
                    }
                    else if (choice == CamproundDBMenu)
                    {
                        CampgroundCLI();
                    }
                    else if (choice == Option_Quit)
                    {
                        exit = true;
                    }
                    else
                    {
                        DisplayInvalidOption();
                        Console.ReadKey();
                    }
                }
                else
                {
                    if (choice == Option_Login)
                    {
                        DisplayLogin();
                    }
                    else if (choice == Option_Register)
                    {
                        DisplayRegister();
                    }
                    else if (choice == Option_Quit)
                    {
                        exit = true;
                    }
                    else
                    {
                        DisplayInvalidOption();
                        Console.ReadKey();
                    }
                }
            }
        }

        private void CampgroundCLI()
        {
            string reserveMenu = "";
            string campMenu = "";
            string parkMenu = "";
      
            bool exit = false;
            try
            {
                while (!exit)
                {
                    Console.Clear();
                    var parkInfo = _cG.GetParks();
                    Console.WriteLine("National Parks");
                    for (int i = 0; i < parkInfo.Count; i++)
                    {
                        Console.WriteLine($"{parkInfo[i].ParkId}) {parkInfo[i].Name} ");
                    }
                    Console.WriteLine("R) Return to Previous Menu");
                    Console.WriteLine("Enter park number to view more detail or choose from available options.");
                    parkMenu = Console.ReadLine().ToLower();
                    for (int i = 0; i < parkInfo.Count; i++)
                    {
                        if (parkMenu == parkInfo[i].ParkId.ToString())
                        {
                            while (submenu == "p")
                            {
                                campMenu = displayParks(parkInfo, i);
                                var campInfo = _cG.GetCampgroundsById(parkInfo[i].ParkId);
                                if (campMenu == View_Campgrounds)
                                {       
                                    reserveMenu = displayCamps(parkInfo, i, campInfo);
                                    submenu = "m";
                                }
                                else if (campMenu == Return_Option)
                                {
                                    submenu = "h";
                                    
                                }
                                if (submenu == Search_Avail_Reserv || campMenu == Search_Avail_Reserv)
                                {
                                    while (submenu == "m")
                                    {
                                        try
                                        {
                                            if (reserveMenu == Search_Avail_Reserv)
                                            {
                                                MakeReservation(parkInfo, i, campInfo);
                                                
                                            }
                                            if (reserveMenu == Return_Option)
                                            {
                                                submenu = "p";
                                            }
                                            else
                                            {
                                                
                                                submenu = "p";
                                                
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine(e.Message);
                                            Console.ReadKey();
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Please use valid input");
                                    Console.ReadKey();
                                }
                            }
                        }
                        else if (parkMenu == Return_Option)
                        {
                            exit = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
        private void MakeReservation(IList<Parks> parkInfo, int i, IList<Campgrounds> campInfo)
        {
            {
                Console.Clear();
                DisplayCamp(_cG.GetCampgroundsById(parkInfo[i].ParkId));
                Console.WriteLine();
                Console.WriteLine("Which campground?");
                string strCampChoice = Console.ReadLine();
                int campChoice = int.Parse(strCampChoice);
                Console.WriteLine("What is the arrival date?");
                string strArriveTime = Console.ReadLine();
                DateTime arriveTime = Convert.ToDateTime(strArriveTime);
                Console.WriteLine("What is the departure date?");
                string strDepartureTime = Console.ReadLine();
                DateTime departTime = Convert.ToDateTime(strDepartureTime);
                var availSites = _cG.GetAvailSite(campChoice, arriveTime, departTime);
                Console.Clear();
                Console.WriteLine("Results matching your Search Criteria");
                Console.WriteLine("Site No.".PadRight(10) + "Max Occup.".PadRight(15) + "Accessible?".PadRight(20) + 
                                   "Max RV Length".PadRight(15) + "Utility".PadRight(15) + "Cost");
                Console.WriteLine();
                var totalCost = (departTime - arriveTime).TotalDays * (double)campInfo[i].DailyFee;
                for (int k = 0; k < availSites.Count; k++)
                {
                    Console.WriteLine(availSites[k].SiteNumber.ToString().PadRight(10) + availSites[k].MaxOccupy.ToString()
                                     .PadRight(15) + availSites[k].Acessible.ToString().PadRight(20) + 
                                     availSites[k].MaxRvLength.ToString().PadRight(15)
                                     + availSites[k].Utilities.ToString().PadRight(15) + totalCost.ToString("C2"));
                }
                Console.WriteLine();
                Console.WriteLine("Which site should be reserved?");
                int siteNumber = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("What name should the reservation be made under?");
                string reserveName = Console.ReadLine();
                Reservation newRes = new Reservation()
                {
                    Name = reserveName,
                    SiteId = siteNumber,
                    FromDate = arriveTime,
                    ToDate = departTime,
                    CreateDate = DateTime.Now
                };
                _cG.TransactionReservationNumber(newRes, _userMgr.User.Id, newRes.ReservationId);
                Console.WriteLine($"The reservation has been made and the confirmation id is " + newRes.ReservationId);
                Console.ReadKey();
            }
        }
        private static string displayCamps(IList<Parks> parkInfo, int i, IList<Campgrounds> campInfo)
        {
            string reserveMenu;
            Console.Clear();
            Console.WriteLine($"{parkInfo[i].Name} Campgrounds");
            Console.WriteLine();
            DisplayCamp(campInfo);
            Console.WriteLine();
            Console.WriteLine("Select a Command");
            Console.WriteLine("M) Search for Reservation");
            Console.WriteLine("R) Return to Previous Screen");
            reserveMenu = Console.ReadLine().ToLower();
            return reserveMenu;
        }

        private static string displayParks(IList<Parks> parkInfo, int i)
        {
            string parkMenu;
            Console.Clear();
            Console.WriteLine("Park Information");
            Console.WriteLine(parkInfo[i].Name);
            Console.WriteLine("Location:".PadRight(20) + parkInfo[i].Location);
            Console.WriteLine("Established:".PadRight(20) + parkInfo[i].EstablishDate);
            int areaNumber = parkInfo[i].Area;
            string areaFormatted = String.Format("{0:n0}", areaNumber);
            Console.WriteLine("Area:".PadRight(20) + areaFormatted + " sq km");
            int visitNumber = parkInfo[i].Visitors;
            string visitFormatted = String.Format("{0:n0}", visitNumber);
            Console.WriteLine("Annual Vistors:".PadRight(20) + visitFormatted);
            Console.WriteLine();
            Console.WriteLine(parkInfo[i].Description);
            Console.WriteLine();
            Console.WriteLine("Select a Command");
            Console.WriteLine("1) View Campground");            
            Console.WriteLine("R) Return to Previous Screen");
            parkMenu = Console.ReadLine();
            return parkMenu;
        }

        private static void DisplayCamp(IList<Campgrounds> campInfo)
        {
            Console.WriteLine("Name".PadLeft(10).PadRight(26) + "Open".PadRight(15) + "Close".PadRight(15) + "Daily Fee");
            Console.WriteLine();
            for (int j = 0; j < campInfo.Count; j++)
            {
                Dictionary<int, string> months = new Dictionary<int, string>();
                months.Add(1, "January");
                months.Add(2, "Febuary");
                months.Add(3, "March");
                months.Add(4, "April");
                months.Add(5, "May");
                months.Add(6, "June");
                months.Add(7, "July");
                months.Add(8, "August");
                months.Add(9, "September");
                months.Add(10, "October");
                months.Add(11, "November");
                months.Add(12, "December");
                Console.WriteLine("#" + campInfo[j].CampGroundId.ToString().PadRight(5) + campInfo[j].Name.PadRight(20) +
                                  months[campInfo[j].OpenFromMM].PadRight(15) + months[campInfo[j].OpenToMM].PadRight(15) +
                                  campInfo[j].DailyFee.ToString("C2"));
            }
        }

        private void DisplayLogin()
        {
            Console.Clear();
            Console.WriteLine("Enter username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            string password = Console.ReadLine();
            try
            {
                _userMgr.LoginUser(username, password);
                Console.WriteLine($"Welcome {_userMgr.User.FirstName} {_userMgr.User.LastName}");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        private void DisplayRegister()
        {
            Console.Clear();

            User user = new User();
            Console.WriteLine("Enter username: ");
            user.Username = Console.ReadLine();
            Console.WriteLine("Enter password: ");
            user.Password = Console.ReadLine();
            Console.WriteLine("Enter password again: ");
            user.ConfirmPassword = Console.ReadLine();
            Console.WriteLine("Enter email: ");
            user.Email = Console.ReadLine();
            Console.WriteLine("Enter first name: ");
            user.FirstName = Console.ReadLine();
            Console.WriteLine("Enter last name: ");
            user.LastName = Console.ReadLine();

            try
            {
                _userMgr.RegisterUser(user);
                Console.WriteLine($"Welcome {_userMgr.User.FirstName} {_userMgr.User.LastName}");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }

        private void DisplayInvalidOption()
        {
            Console.WriteLine(" The option you selected is not a valid option.");
            Console.WriteLine();
        }

        private void AsciiHouse()
        {
            Console.WriteLine(" ^  ^  ^   ^      ___I_      ^  ^   ^  ^  ^   ^  ^ ");
            Console.WriteLine("/|\\/|\\/|\\ /|\\    /\\-_--\\    /|\\/|\\ /|\\/|\\/|\\ /|\\/|\\ ");
            Console.WriteLine("/|\\/|\\/|\\ /|\\   /  \\_-__\\   /|\\/|\\ /|\\/|\\/|\\ /|\\/|\\ ");
            Console.WriteLine("/|\\/|\\/|\\ /|\\   |[]| [] |   /|\\/|\\ /|\\/|\\/|\\ /|\\/|\\ ");
        }

        private void AsciiTitle()
        {
            Console.WriteLine(" _      __ __  ____    ____  ____        ____  ____    ____  _        ___   ");
            Console.WriteLine("| |    |  |  ||    \\  /    ||    \\      /    ||    \\  /    || |      /  _]  ");
            Console.WriteLine("| |    |  |  ||  _  ||  o  ||  D  )    |  o  ||  _  ||   __|| |     /  [_   ");
            Console.WriteLine("| |___ |  |  ||  |  ||     ||    /     |     ||  |  ||  |  || |___ |    _]  ");
            Console.WriteLine("|     ||  :  ||  |  ||  _  ||    \\     |  _  ||  |  ||  |_ ||     ||   [_   ");
            Console.WriteLine("|     ||     ||  |  ||  |  ||  .  \\    |  |  ||  |  ||     ||     ||     |  ");
            Console.WriteLine("|_____| \\__,_||__|__||__|__||__|\\_|    |__|__||__|__||___,_||_____||_____|  ");
            Console.WriteLine();
            Console.WriteLine(" ____   ____  ____   __  _       _____   ___  ____  __ __  ____   __    ___ ");
            Console.WriteLine("|    \\ /    ||    \\ |  |/ ]     / ___/  /  _]|    \\|  |  ||    | /  ]  /  _]");
            Console.WriteLine("|  o  )  o  ||  D  )|  ' /     (   \\_  /  [_ |  D  )  |  | |  | /  /  /  [_ ");
            Console.WriteLine("|   _/|     ||    / |    \\      \\__  ||    _]|    /|  |  | |  |/  /  |    _]");
            Console.WriteLine("|  |  |  _  ||    \\ |     \\     /  \\ ||   [_ |    \\|  :  | |  /   \\_ |   [_ ");
            Console.WriteLine("|  |  |  |  ||  .  \\|  .  |     \\    ||     ||  .  \\\\   /  |  \\     ||     |");
            Console.WriteLine("|__|  |__|__||__|\\_||__|\\_|      \\___||_____||__|\\_| \\_/  |____\\____||_____|");
        }
    }
}
