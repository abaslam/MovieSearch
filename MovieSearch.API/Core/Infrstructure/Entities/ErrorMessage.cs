namespace MovieSearch.API.Core.Infrstructure.Entities
{
    public class ErrorMessage
    {
        public ErrorMessage(string propertyName, string message)
        {
            this.PropertyName = propertyName;
            this.Message = message;
        }

        public string PropertyName { get; }
        public string Message { get;  }
    }
}
