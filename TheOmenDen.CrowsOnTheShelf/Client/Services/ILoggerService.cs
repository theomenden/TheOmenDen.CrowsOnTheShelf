namespace TheOmenDen.CrowsOnTheShelf.Client.Services;

public interface ILoggerService
{
    Task Fatal(Exception exception);
}