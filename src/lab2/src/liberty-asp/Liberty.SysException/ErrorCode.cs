using System.ComponentModel;

namespace Liberty.SysException;

public enum ErrorCode
{
    #region Common [E0xxx]

    /// <summary>
    /// Error the system
    /// </summary>
    [Description("Error the system")]
    E0100,

    /// <summary>
    /// Error the database
    /// </summary>
    [Description("Error the database")]
    E0101,

    /// <summary>
    /// Error the app settings
    /// </summary>
    [Description("Error the app settings")]
    E0102,

    /// <summary>
    /// Application user key not found
    /// </summary>
    [Description("Application user key not found")]
    E0103,

    /// <summary>
    /// System is currently processing too many concurrent requests
    /// </summary>
    [Description("System is currently processing too many concurrent requests")]
    E0104,

    /// <summary>
    /// The field is required
    /// </summary>
    [Description("The field is required")]
    E0001,

    /// <summary>
    /// The field is too long
    /// </summary>
    [Description("The field is too long")]
    E0002,

    /// <summary>
    /// The field is number greater than 0
    /// </summary>
    [Description("The field is number greater than 0")]
    E0003,

    /// <summary>
    /// This field is a number less than the specified number
    /// </summary>
    [Description("This field is a number less than the specified number")]
    E0004,

    /// <summary>
    /// The field is invalid url
    /// </summary>
    [Description("The field is invalid url")]
    E0005,

    /// <summary>
    /// The field is invalid email format
    /// </summary>
    [Description("The field is invalid email format")]
    E0006,

    /// <summary>
    /// The field is invalid json
    /// </summary>
    [Description("The field is invalid json")]
    E0007,

    /// <summary>
    /// The field is invalid date
    /// </summary>
    [Description("The field is invalid date")]
    E0008,

    /// <summary>
    /// The field is greater than or equal start date
    /// </summary>
    [Description("The field is greater than or equal start date")]
    E0009,

    /// <summary>
    /// The field has a value less than or equal to 0
    /// </summary>
    [Description("The field has a value less than or equal to 0")]
    E0010,

    /// <summary>
    /// The field is number greater than or equal to 0
    /// </summary>
    [Description("The field is number greater than or equal to 0")]
    E0011,

    /// <summary>
    /// The field is invalid time format
    /// </summary>
    [Description("The field is invalid time format")]
    E0012,

    /// <summary>
    /// The field is invalid color foramt
    /// </summary>
    [Description("The field is invalid color foramt")]
    E0013,

    /// <summary>
    /// The field has a value is empty
    /// </summary>
    [Description("The field has a value is empty")]
    E0014,

    /// <summary>
    /// The field is greater than or equal start day
    /// </summary>
    [Description("The field is greater than or equal start day")]
    E0015,

    [Description("The field is invalid when the plan is set to use day.")]
    E0016,

    #endregion

    #region Reservation module flow [E1xxx]

    /// <summary>
    /// The facility not available
    /// </summary>
    [Description("The facility not available")]
    E1001,

    /// <summary>
    /// The body content in the mail template is invalid
    /// </summary>
    [Description("The body content in the mail template is invalid")]
    E1002,

    /// <summary>
    /// The subject content in the mail template is invalid
    /// </summary>
    [Description("The subject content in the mail template is invalid")]
    E1003,

    /// <summary>
    /// The site code is duplicated
    /// </summary>
    [Description("The site code {0} is duplicated")]
    E1004,

    /// <summary>
    /// The app date not found
    /// </summary>
    [Description("The app date not found")]
    E1005,

    /// <summary>
    /// The data of the app date not found
    /// </summary>
    [Description("The data of the app date not found")]
    E1006,

    /// <summary>
    /// The type of the app date not found
    /// </summary>
    [Description("The type of the app date not found")]
    E1007,

    /// <summary>
    /// The area not found
    /// </summary>
    [Description("The area not found")]
    E1008,

    /// <summary>
    /// The category not found
    /// </summary>
    [Description("The category not found")]
    E1009,

    /// <summary>
    /// The consumption tax not found
    /// </summary>
    [Description("The consumption tax not found")]
    E1010,

    /// <summary>
    /// The facility not found
    /// </summary>
    [Description("The facility not found")]
    E1011,

    /// <summary>
    /// The fax service not found
    /// </summary>
    [Description("The fax service not found")]
    E1012,

    /// <summary>
    /// The mail template not found
    /// </summary>
    [Description("The mail template {0} not found")]
    E1013,

    /// <summary>
    /// The point rate not found
    /// </summary>
    [Description("The point rate not found")]
    E1014,

    /// <summary>
    /// The site not found
    /// </summary>
    [Description("The site not found")]
    E1015,

    /// <summary>
    /// The system configuration not found
    /// </summary>
    [Description("The system configuration not found")]
    E1016,

    /// <summary>
    /// The field is greater than or equal person min
    /// </summary>
    [Description("The field is greater than or equal person min")]
    E1017,

    /// <summary>
    /// This field must be greater than or equal to display date start when use display date is true
    /// </summary>
    [Description("This field must be greater than or equal to display date start when use display date is true")]
    E1018,

    /// <summary>
    /// This field must be greater than or equal to accept date start when use accept date is true
    /// </summary>
    [Description("This field must be greater than or equal to accept date start when use accept date is true")]
    E1019,

    /// <summary>
    /// This field is greater than check-in start
    /// </summary>
    [Description("This field is greater than check-in start")]
    E1020,

    /// <summary>
    /// The field is greater than capacity min
    /// </summary>
    [Description("The field is greater than capacity min")]
    E1021,

    /// <summary>
    /// The field is less than or equal to 18 age
    /// </summary>
    [Description("The field is less than or equal to 18 age")]
    E1022,

    /// <summary>
    /// The field is greater than or equal to age min
    /// </summary>
    [Description("The field is greater than or equal to age min")]
    E1023,

    /// <summary>
    /// The field has items must be less than or equal to 4
    /// </summary>
    [Description("The field has items must be less than or equal to 4")]
    E1024,

    /// <summary>
    /// The field is greater than or equal to price min
    /// </summary>
    [Description("The field is greater than or equal to price min")]
    E1025,

    /// <summary>
    /// The field is duplicated with price min in spas
    /// </summary>
    [Description("The field is duplicated with price min in spas")]
    E1026,

    /// <summary>
    /// The field is duplicated with price max in spas
    /// </summary>
    [Description("The field is duplicated with price max in spas")]
    E1027,

    /// <summary>
    /// This field must be confirmed as yes
    /// </summary>
    [Description("This field must be confirmed as yes")]
    E1028,

    /// <summary>
    /// The allergen not found
    /// </summary>
    [Description("The allergen not found")]
    E1029,

    /// <summary>
    /// The bed type not found
    /// </summary>
    [Description("The bed type not found")]
    E1030,

    /// <summary>
    /// The calendar not found
    /// </summary>
    [Description("The calendar not found")]
    E1031,

    /// <summary>
    /// The data of the cancellation policy not found
    /// </summary>
    [Description("The data of the cancellation policy not found")]
    E1032,

    /// <summary>
    /// The cancellation policy not found
    /// </summary>
    [Description("The cancellation policy not found")]
    E1033,

    /// <summary>
    /// The media not found
    /// </summary>
    [Description("The media not found")]
    E1034,

    /// <summary>
    /// The question of the facility not found
    /// </summary>
    [Description("The question of the facility not found")]
    E1035,

    /// <summary>
    /// The group name be duplicated in the facility
    /// </summary>
    [Description("The group name {0} be duplicated in the facility {1}")]
    E1036,

    /// <summary>
    /// The meal type not found
    /// </summary>
    [Description("The meal type not found")]
    E1037,

    /// <summary>
    /// The option item not found
    /// </summary>
    [Description("The option item not found")]
    E1038,

    /// <summary>
    /// The person age type not found
    /// </summary>
    [Description("Person age type not found")]
    E1039,

    /// <summary>
    /// The data in spa tax of the person age type not found
    /// </summary>
    [Description("The data in spa tax of the person age type not found")]
    E1040,

    /// <summary>
    /// The plan not found
    /// </summary>
    [Description("The plan not found")]
    E1041,

    /// <summary>
    /// The question not found
    /// </summary>
    [Description("The question not found")]
    E1042,

    /// <summary>
    /// The selected quantity exceeds the available quantity when setting for the room group
    /// </summary>
    [Description("Does not satisfy the {0} room's inventory quantity, the maximum quantity is {1}")]
    E1043,

    /// <summary>
    /// The room group not found
    /// </summary>
    [Description("The room group not found")]
    E1044,

    /// <summary>
    /// The plan of type Room Only not found
    /// </summary>
    [Description("The plan of type Room Only not found")]
    E1045,

    /// <summary>
    /// The number of nighs must be equal to the number of distinct AppDate values in GuestsPerRoom
    /// </summary>
    [Description("The number of nighs must be equal to the number of distinct AppDate values in GuestsPerRoom")]
    E1046,

    /// <summary>
    /// The room group not available in the plan
    /// </summary>
    [Description("The room group not available in the plan")]
    E1047,

    /// <summary>
    /// The site not already in the facility
    /// </summary>
    [Description("The site not already in the facility")]
    E1048,

    /// <summary>
    /// The room name is existied
    /// </summary>
    [Description("The room name is existied")]
    E1049,

    /// <summary>
    /// The option item payload must not be greater than 100
    /// </summary>
    [Description("The option item payload must not be greater than {0}")]
    E1050,

    /// <summary>
    /// The fax number incorrect
    /// </summary>
    [Description("The fax number incorrect")]
    E1051,

    /// <summary>
    /// Reservation Status not allowed
    /// </summary>
    [Description("Reservation Status not allowed")]
    E1052,

    /// <summary>
    /// The number of PriceDatas entries exceeds the allowed limit
    /// </summary>
    [Description("The number of PriceDatas entries exceeds the allowed limit")]
    E1053,

    /// <summary>
    /// Not allowed global online payment
    /// </summary>
    [Description("Not allowed global online payment")]
    E1054,

    /// <summary>
    /// Plan minimum price must not be lower than facility minimum price
    /// </summary>
    [Description("Plan minimum price must not be lower than facility minimum price")]
    E1055,

    /// <summary>
    /// Plan daily pricing must not be lower than facility minimum price
    /// </summary>
    [Description("Plan daily pricing must not be lower than facility minimum price")]
    E1056,

    /// <summary>
    /// Plan standard price must not be lower than facility minimum price
    /// </summary>
    [Description("Plan standard price must not be lower than facility minimum price")]
    E1057,

    /// <summary>
    /// This field must be greater than or equal to booking reception start when use booking reception is true
    /// </summary>
    [Description("This field must be greater than or equal to booking reception start when use booking reception is true")]
    E1058,

    /// <summary>
    /// The reception day limit must be in the range of 0 to 60
    /// </summary>
    [Description("The reception day limit must be in the range of 0 to 120")]
    E1059,

    /// <summary>
    /// This reservation cannot be canceled, modified, or set as no-show because it is past the due date
    /// </summary>
    [Description("This reservation cannot be canceled, modified, or set as no-show because it is past the due date")]
    E1060,

    /// <summary>
    /// The question has no content
    /// </summary>
    [Description("The question has no content")]
    E1061,

    /// <summary>
    /// The type data not configured yet
    /// </summary>
    [Description("The type data not configured yet")]
    E1062,

    /// <summary>
    /// For facilities that only accept online bookings, reservation changes cannot be made when payment is made online.
    /// </summary>
    [Description("For facilities that only accept online bookings, reservation changes cannot be made when payment is made online.")]
    E1063,

    /// <summary>
    /// The cancellation policy is currently being used by a plan, so it cannot be disabled.
    ///</summary>
    [Description("The cancellation policy is currently being used by a plan, so it cannot be disabled.")]
    E1064,

    /// <summary>
    /// Has plan using the cancellation policy, cannot delete.
    /// </summary>
    [Description("Has plan using the cancellation policy, cannot delete.")]
    E1065,

    /// <summary>
    /// Cancellation fee is invalid, must be between 0 and the booking payment amount.
    /// </summary>
    [Description("Cancellation fee is invalid, must be between 0 and the booking payment amount.")]
    E1066,

    /// <summary>
    /// Alert Message not found.
    /// </summary>
    [Description("Alert Message not found.")]
    E1067,

    /// <summary>
    /// If you specify an amount, you cannot register numbers below 100.
    /// </summary>
    [Description("If you specify an amount, you cannot register numbers below 100.")]
    E1068,

    /// <summary>
    /// If you specify a percentage, you cannot register a number between 1 and 100.
    /// </summary>
    [Description("If you specify a percentage, you cannot register a number between 1 and 100.")]
    E1069,

    /// <summary>
    /// The facility does not allow an extended stay on changes
    /// </summary>
    [Description("The facility does not allow an extended stay on changes")]
    E1070,

    /// <summary>
    /// The reception start day limit must be in the range of 0 to 99
    /// </summary>
    [Description("The reception start day limit must be in the range of 0 to 99")]
    E1071,

    /// <summary>
    /// Time must be on the hour (hh:00:00)
    /// </summary>
    [Description("Time must be on the hour (hh:00:00)")]
    E1072,

    /// <summary>
    /// The room size must be a positive numeric value, allowing only one digit after the decimal separator.
    /// </summary>
    [Description("The room size must be a positive numeric value, allowing only one digit after the decimal separator.")]
    E1073,

    /// <summary>
    /// The destination prefix name is duplicated
    /// </summary>
    [Description("The destination prefix name {0} is duplicated")]
    E1074,

    /// <summary>
    /// The destination prefix name using two capital letters
    /// </summary>
    [Description("The destination prefix name using two capital letters")]
    E1075,

    /// <summary>
    /// The person age type not found
    /// </summary>
    [Description("The person age type not found")]
    E1076,

    #endregion

    #region Booking flow [E2xxx]

    /// <summary>
    /// The order not found
    /// </summary>
    [Description("The order not found")]
    E2001,

    /// <summary>
    /// The reservation not found
    /// </summary>
    [Description("The reservation {0} not found")]
    E2002,

    /// <summary>
    /// The reservation information error
    /// </summary>
    [Description("The reservation information error")]
    E2003,

    /// <summary>
    /// The reservation service error
    /// </summary>
    [Description("The reservation service error")]
    E2004,

    /// <summary>
    /// The reservation information not found by reservation, app date and room group index
    /// </summary>
    [Description(
        "The reservation information not found at reservationId:{0}, appDateId:{1}, roomGroupIndex:{2}"
    )]
    E2005,

    /// <summary>
    /// The specified plan or room type is not available by facility, plan and room group
    /// </summary>
    [Description(
        "The specified plan or room type is not available with facility id {0}, plan id {1}, room group id {2}"
    )]
    E2006,

    /// <summary>
    /// The specified plan, room type, or location is not available.
    /// </summary>
    [Description("The specified plan {0}, room type {1}, or location {2} is not available.")]
    E2007,

    /// <summary>
    /// There is an error in the number of points remaining
    /// </summary>
    [Description("There is an error in the number 0 of {1} points remaining")]
    E2008,

    /// <summary>
    /// The specified number of points cannot be used
    /// </summary>
    [Description("The specified number {0} of {1} points cannot be used")]
    E2009,

    /// <summary>
    /// The reservation has no payment result
    /// </summary>
    [Description("The reservation has no payment result")]
    E2010,

    /// <summary>
    /// The reservation question not found with reservation id and question id
    /// </summary>
    [Description("The reservation question not found with reservation id {0} and question id {1}")]
    E2011,

    /// <summary>
    /// The price minus of the reservation has error
    /// </summary>
    [Description("The price minus of the reservation has error")]
    E2012,

    /// <summary>
    /// The app date not found when searching by site of the rooms in the plan
    /// </summary>
    [Description("The app date not found when searching by site of the rooms in the plan")]
    E2013,

    /// <summary>
    /// The reservation over remain room number
    /// </summary>
    [Description("The reservation over remain room number")]
    E2014,

    /// <summary>
    /// The reservation over plan group number day sale limit
    /// </summary>
    [Description("The reservation over plan group number day sale limit")]
    E2015,

    /// <summary>
    /// The reservation over plan day sale limit number of rooms data sale limit
    /// </summary>
    [Description("The reservation over plan day sale limit number of rooms data sale limit")]
    E2016,

    /// <summary>
    /// The reservation over display date
    /// </summary>
    [Description("The reservation over display date")]
    E2017,

    /// <summary>
    /// The plan cannot be booked for specified number of nights at number of nights
    /// </summary>
    [Description("The plan cannot be booked for specified number of nights at {0} nights")]
    E2018,

    /// <summary>
    /// The reservation over capacity number of nights
    /// </summary>
    [Description("The reservation over capacity number of nights")]
    E2019,

    /// <summary>
    /// The reservation over accept number of persons
    /// </summary>
    [Description("The reservation over accept number of persons")]
    E2020,

    /// <summary>
    /// The reservation over accept date
    /// </summary>
    [Description("The reservation over accept date")]
    E2021,

    /// <summary>
    /// The reservation out of date
    /// </summary>
    [Description("The reservation out of date")]
    E2022,

    /// <summary>
    /// The reservation exceeds stock quantity
    /// </summary>
    [Description("The reservation exceeds stock quantity with {0} of {1}")]
    E2023,

    /// <summary>
    /// The reservation period in which online payments are possible has exceeded
    /// </summary>
    [Description("The reservation {0} period in which online payments are possible has exceeded")]
    E2024,

    /// <summary>
    /// The reservation not selled in the plan
    /// </summary>
    [Description("The reservation not selled in the plan")]
    E2025,

    /// <summary>
    /// The reservation no remain number of night
    /// </summary>
    [Description("The reservation has no remaining nights, rooms, or options")]
    E2026,

    /// <summary>
    /// The reservation no price setting
    /// </summary>
    [Description("The reservation no price setting")]
    E2027,

    /// <summary>
    /// The reservation no match persons
    /// </summary>
    [Description("The reservation no match persons")]
    E2028,

    /// <summary>
    /// The reservation no person age type
    /// </summary>
    [Description("The reservation no person age type")]
    E2029,

    /// <summary>
    /// The reservation no data
    /// </summary>
    [Description("The reservation no data")]
    E2030,

    /// <summary>
    /// The reservation no adult persons
    /// </summary>
    [Description("The reservation no adult persons")]
    E2031,

    /// <summary>
    /// The reservation invalid
    /// </summary>
    [Description("The reservation invalid with {0}")]
    E2032,

    /// <summary>
    /// The reservation has no adult person age types
    /// </summary>
    [Description("The reservation has no adult person age types")]
    E2033,

    /// <summary>
    /// The reservation cannot be made under the specified conditions
    /// </summary>
    [Description("The reservation cannot be made under the specified conditions with app date {0}")]
    E2034,

    /// <summary>
    /// The cancellation policy of the reservation not found
    /// </summary>
    [Description("The cancellation policy of the reservation not found")]
    E2035,

    /// <summary>
    /// The payment online method is not available
    /// </summary>
    [Description("Payment online method is not available")]
    E2036,

    /// <summary>
    /// The number of nights in the booking is invalid
    /// </summary>
    [Description("The number of nights in the booking is invalid")]
    E2037,

    /// <summary>
    /// The number of rooms in the booking is invalid
    /// </summary>
    [Description("The number of rooms in the booking is invalid")]
    E2038,

    /// <summary>
    /// Booking error: Price for option item {optionItemId} of room {roomId} on date {dateStay}
    /// </summary>
    [Description("Booking error: Price for option item {0} of room {1} on date {2}")]
    E2039,

    /// <summary>
    /// Booking error: Price for option item {optionItemId} of room {roomId} on date {dateStay}
    /// </summary>
    [Description("Booking error: Price for room {0} on date {1}")]
    E2040,

    /// <summary>
    /// This field must be confirmed as yes
    /// </summary>
    [Description("This field must be confirmed as yes")]
    E2041,

    /// <summary>
    /// This field is greater than check-in start
    /// </summary>
    [Description("This field is greater than check-in start")]
    E2042,

    /// <summary>
    /// The field is greater than or equal to price min
    /// </summary>
    [Description("The field is greater than or equal to price min")]
    E2043,

    /// <summary>
    /// The guest's reservation code is incorrect
    /// </summary>
    [Description("The guest's reservation code is incorrect")]
    E2044,

    /// <summary>
    /// An error occurred when facility is changed while booking
    /// </summary>
    [Description("An error occurred when facility is changed while booking")]
    E2045,

    /// <summary>
    /// An error occurred when plan is changed while booking
    /// </summary>
    [Description("An error occurred when plan is changed while booking")]
    E2046,

    /// <summary>
    /// An error occurred when site is changed while booking
    /// </summary>
    [Description("An error occurred when site is changed while booking")]
    E2047,

    /// <summary>
    /// An error occurred when room group is changed while booking
    /// </summary>
    [Description("An error occurred when room group is changed while booking")]
    E2048,

    /// <summary>
    /// An error occurred when option item is changed while booking
    /// </summary>
    [Description("An error occurred when option item is changed while booking")]
    E2049,

    /// <summary>
    /// An error occurred when cancellation is changed while booking
    /// </summary>
    [Description("An error occurred when cancellation is changed while booking")]
    E2050,

    /// <summary>
    /// An error occurred when person age type is changed while booking
    /// </summary>
    [Description("An error occurred when person age type is changed while booking")]
    E2051,

    /// <summary>
    /// The user key not found in the columns of jwt
    /// </summary>
    [Description("The user key not found in the columns of jwt")]
    E2052,

    /// <summary>
    /// Number appDateId in nightPeoples equal to numberOfNights
    /// </summary>
    [Description("Number appDateId in nightPeoples equal to numberOfNights")]
    E2053,

    /// <summary>
    /// Number roomIndex in nightPeoples equal to numberOfRooms
    /// </summary>
    [Description("Number roomIndex in nightPeoples equal to numberOfRooms")]
    E2054,

    /// <summary>
    /// roomRepresentatives equal to numberOfRooms
    /// </summary>
    [Description("roomRepresentatives equal to numberOfRooms")]
    E2055,

    /// <summary>
    /// An error occurred when option item is changed while booking
    /// </summary>
    [Description("An error occurred when question is changed while booking")]
    E2056,

    /// <summary>
    /// An error occurred when cancellation is changed while booking
    /// </summary>
    [Description("An error occurred when file is changed while booking")]
    E2057,

    /// <summary>
    /// An error occurred when last updated string request is invalid
    /// </summary>
    [Description("Last updated string is invalid")]
    E2058,

    /// <summary>
    /// An error occurred when option item last updated string is invalid
    /// </summary>
    [Description("Option item last updated string is invalid")]
    E2059,

    /// <summary>
    /// The guest’s secret code is invalid.
    /// </summary>
    [Description("The guest’s secret code is invalid.")]
    E2060,

    /// <summary>
    /// The guest has already confirmed the booking
    /// </summary>
    [Description("The guest has already confirmed the booking")]
    E2061,

    /// <summary>
    /// The plan has been set up
    /// </summary>
    [Description("The plan has been set up")]
    E2062,

    /// <summary>
    /// Guest cannot make a reservation using the online method.
    /// </summary>
    [Description("Guest cannot make reservation using the online method")]
    E2063,

    /// <summary>
    /// The current time is not the appropriate time to mark the booking as no-show.
    /// </summary>
    [Description("The current time is not the time to set the booking as no show.")]
    E2064,

    /// <summary>
    /// The payment on site method is not available.
    /// </summary>
    [Description("Payment on site method is not available")]
    E2065,

    /// <summary>
    /// The reservation was successfully canceled, but the online payment cancellation failed.
    /// </summary>
    [Description("The reservation was successfully canceled, but the online payment cancellation failed.")]
    E2066,

    /// <summary>
    /// Online payment amount change failed.
    /// </summary>
    [Description("Online payment amount change failed.")]
    E2067,

    /// <summary>
    /// Cancellation fee payment cancellation failed.
    /// </summary>
    [Description("Cancellation fee payment cancellation failed.")]
    E2068,

    /// <summary>
    /// Online payment cannot be selected for bookings more than the allowed number of days ahead.
    /// </summary>
    [Description("Online payment cannot be selected for bookings more than {0} days ahead.")]
    E2069,

    /// <summary>
    /// Fax audit log not found.
    /// </summary>
    [Description("Fax Audit Log not found")]
    E2070,

    /// <summary>
    /// The specified question requires an answer.
    /// </summary>
    [Description("Question {0} is required, please submit the answer")]
    E2071,

    /// <summary>
    /// The price for children must be either 0 or 100 or greater.
    /// </summary>
    [Description("Price for children must be 0 or greater than 100")]
    E2072,

    /// <summary>
    /// Not enough rooms available to fulfill the reservation.
    /// </summary>
    [Description("Not enough rooms available to fulfill the reservation.")]
    E2073,

    /// <summary>
    /// No valid option is available for this reservation.
    /// </summary>
    [Description("No valid option is available for this reservation.")]
    E2074,

    /// <summary>
    /// No suitable age range exists for the requested reservation.
    /// </summary>
    [Description("No suitable age range exists for the requested reservation.")]
    E2075,

    /// <summary>
    /// Not enough nights available to fulfill the reservation.
    /// </summary>
    [Description("Not enough nights available to fulfill the reservation.")]
    E2076,
    
    [Description("Some dates in the selected range are currently being updated by another user.")]
    E2077,
    
    [Description("Both reception limit and reception day limit are required and must be specified together.")]
    E2078,
    
    #endregion

    #region Payment with GMO

    /// <summary>
    /// An error occurred during the payment search process
    /// </summary>
    [Description("An error occurred during the payment search process with order {0}")]
    E3001,

    /// <summary>
    /// An error occurred during the cancellation process
    /// </summary>
    [Description("An error occurred during the cancellation process with access {0}")]
    E3002,

    /// <summary>
    /// An error occurred while making payment
    /// </summary>
    [Description("An error occurred while making payment with access {0}")]
    E3003,

    /// <summary>
    /// An error occurred while processing your payment
    /// </summary>
    [Description("An error occurred while processing your payment: {0}")]
    E3004,

    /// <summary>
    /// An error occurred while processing your payment
    /// </summary>
    [Description("An error occurred while processing your get payment url: {0} {1}")]
    E3005,

    /// <summary>
    /// Configuration error encountered while processing the request.
    /// </summary>
    [Description("Failed to change the online payment amount, so the reservation change could not be completed.")]
    E3006,

    /// <summary>
    /// The reservation information update failed, so the reservation change could not be completed.
    /// There may be a discrepancy between the payment amount and the reservation amount.
    /// </summary>
    [Description(
        "The reservation information update failed, so the reservation change could not be completed. There may be a discrepancy between the payment amount and the reservation amount."
    )]
    E3007,

    /// <summary>
    /// The reservation information update failed, so the reservation change could not be completed.
    /// </summary>
    [Description("The reservation information update failed, so the reservation change could not be completed.")]
    E3008,

    #endregion

    #region Kakusan

    /// <summary>
    /// Kakusan is invalid request
    /// </summary>
    [Description("Kakusan is invalid request")]
    E4001,

    #endregion

    #region Plan distribution

    /// <summary>
    /// The number of days that can be updated or retrieved at once is limited to 180 days. (Specified days: {0})
    /// </summary>
    [Description("一度に更新および取得できる日数は180日までとなります。(設定された日数：{0})")]
    E5001,

    /// <summary>
    /// The date that can be updated or retrieved must be within 365 days from today. (Lower limit date: {0})
    /// </summary>
    [Description("更新および取得できる日付は本日から365日までとなります。(下限日：{0})")]
    E5002,

    /// <summary>
    /// An invalid message was detected. Element name: {0}, Content: {1}
    /// </summary>
    [Description("不正な電文を検知しました。要素名：{0} 内容：{1}")]
    E5003,

    /// <summary>
    /// The date that can be updated or retrieved must be within 365 days from today. (Upper limit date: {0})
    /// </summary>
    [Description("更新および取得できる日付は本日から365日までとなります。(上限日：{0})")]
    E5004,

    /// <summary>
    /// {0} must be specified within the range 0 to 60.
    /// </summary>
    [Description("{0} は0～60の範囲で指定してください。")]
    E5005,

    /// <summary>
    /// {0} must be registered on or after {1}.
    /// </summary>
    [Description("{0} は {1} 以降を登録してください。")]
    E5006,

    #endregion
}
