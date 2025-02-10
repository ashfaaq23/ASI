namespace UniversiteDomain.Exceptions.ParcoursExceptions;

public class InvalidAnneeFormationException:Exception
{
    public InvalidAnneeFormationException() : base() { }
    public InvalidAnneeFormationException(string message) : base(message) { }
    public InvalidAnneeFormationException(string message, Exception inner) : base(message, inner) { }
}