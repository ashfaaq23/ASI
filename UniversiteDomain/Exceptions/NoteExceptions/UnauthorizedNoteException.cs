namespace UniversiteDomain.Exceptions.NoteExceptions;

public class UnauthorizedNoteException : Exception
{
    public UnauthorizedNoteException() : base() { }
    public UnauthorizedNoteException(string message) : base(message) { }
    public UnauthorizedNoteException(string message, Exception inner) : base(message, inner) { }
}