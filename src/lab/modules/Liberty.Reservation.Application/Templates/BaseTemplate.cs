namespace Liberty.Reservation.Application.Templates;

public interface IIOTemplate
{
    string Subject { get; }
    string Body { get; }

    public void SetSample();
}

public abstract class BaseTemplate : IIOTemplate
{
    public abstract string Subject { get; }
    public abstract string Body { get; }

    public abstract void SetSample();
}
