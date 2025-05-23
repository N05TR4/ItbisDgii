using FluentValidation.Results;

namespace ItbisDgii.Application.Exceptions
{
    public class ValidationsExceptions : Exception
    {


        public ValidationsExceptions() : base("Se han producido uno o más errores de validación")
        {
            Errors = new List<string>();
        }

        public List<string> Errors { get; }

        public ValidationsExceptions(IEnumerable<ValidationFailure> failures) : this()
        {
            foreach (var failure in failures)
            {
                Errors.Add(failure.ErrorMessage);
            }

        }
    }
}
