using MediatR;
using MovieSearch.API.Core.Data.Entities;
using MovieSearch.API.Core.Infrstructure;
using MovieSearch.API.Core.Services.Entities;
using Nest;

namespace MovieSearch.API.Core.Services.Handlers
{
    public class CriteriaHandler : IRequestHandler<CriteriaRequest, CriteriaResponse>
    {
        private readonly IElasticClient elasticClient;

        public CriteriaHandler(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public async Task<CriteriaResponse> Handle(CriteriaRequest request, CancellationToken cancellationToken)
        {
            var response = new CriteriaResponse();

            var aggregateResponse = this.elasticClient.Search<Movie>(s => s
            .Index(Constants.MovieDataIndexName)
            .Size(0)
            .Aggregations(a => a
                .Terms("year_aggregation", t => t
                    .Field(f => f.Year)
                    .Size(10_000))
                .Nested("nested_genres", n => n
                    .Path(p => p.Genres)
                    .Aggregations(na => na
                        .Terms("genre_aggregation", t => t
                            .Field(f => f.Genres.First().Name)
                            .Size(10_000) 
                        )
                    )
                )));

            if (aggregateResponse.IsValid)
            {

                response.Years = aggregateResponse.Aggregations
                    .Terms("year_aggregation")
                    .Buckets
                    .Select(x => int.Parse(x.Key))
                    .ToList();

                response.Genres = aggregateResponse.Aggregations.Nested("nested_genres")
                    .Terms("genre_aggregation")
                    .Buckets
                    .Select(x => x.Key)
                    .ToList();
            }

            return response;
        }
    }
}
