namespace MovieSearch.API.Core.Infrstructure.Entities
{
    public class Response
    {
        private List<ErrorMessage> errorMessages = new List<ErrorMessage>();

        public IReadOnlyList<ErrorMessage> ErrorMessages
        {
            get
            {
                return new List<ErrorMessage>(this.errorMessages);
            }

            private set
            {
            }
        }

        public bool IsSuccess
        {
            get
            {
                return !this.errorMessages.Any();
            }

            private set
            {
            }
        }

        public Response Error(string propertyName, string message)
        {
            this.errorMessages.Add(new ErrorMessage(propertyName, message));
            return this;
        }

        public Response Error(string message)
        {
            return this.Error(string.Empty, message);
        }

        public static Response<T> Create<T>()
            where T : new()
        {
            var result = new Response<T>();
            result.Data = new T();
            return result;
        }
    }
}
