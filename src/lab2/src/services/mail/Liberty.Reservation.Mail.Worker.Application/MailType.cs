namespace Liberty.Reservation.Mail.Worker.Application;

public enum MailActorTypes
{
    Site,
    Manager,
    Employee,
    User,
    Guest,
    Reminder
}

public enum MailSentStatus
{
    Success = 1,
    Failed = 2
}
