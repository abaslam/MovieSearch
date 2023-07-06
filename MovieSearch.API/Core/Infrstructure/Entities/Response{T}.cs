namespace MovieSearch.API.Core.Infrstructure.Entities
{
    public class Response<T> : Response
    {
        public Response()
        {
        }

        public Response(T data)
        {
            this.Data = data;
        }

        public T Data { get; set; }

        public bool HasData
        {
            get
            {
                return this.Data != null;
            }
        }
    }
}
