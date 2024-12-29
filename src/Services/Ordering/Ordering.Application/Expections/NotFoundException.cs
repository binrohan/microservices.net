namespace Ordering.Application.Expections;

public class NotFoundException(string name, object key)
    : ApplicationException($"Entity \"{name}\" ({key}) was not found.")
{

}
