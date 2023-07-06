using MediatR;
using Microsoft.AspNetCore.Http;
using MovieSearch.API.Core.Data.Entities;
using MovieSearch.API.Core.Infrstructure;
using MovieSearch.API.Core.Infrstructure.Entities;
using MovieSearch.API.Core.Services.Entities;
using MovieSearch.API.Core.Utilities;
using Nest;

namespace MovieSearch.API.Core.Services.Handlers
{
    public class SearchMoviesHandler : IRequestHandler<Entities.SearchRequest, Response<SearchResponse>>
    {
        private readonly IElasticClient elasticClient;

        public SearchMoviesHandler(IElasticClient elasticClient)
        {
            this.elasticClient = elasticClient;
        }
        public async Task<Response<SearchResponse>> Handle(Entities.SearchRequest request, CancellationToken cancellationToken)
        {
            var response = Response.Create<SearchResponse>();

            var searchResponse = await this.elasticClient
                                           .SearchAsync<Movie>(x => this.AggregateMovies(x)
                                                                        .Sort(sort => sort.Descending(SortSpecialField.Score)
                                                                                          .Field(f => f.Field(ff => ff.Year).Descending()))
                                                                         .Size(request.PageSize)
                                                                         .Query(q => this.QueryMovies(request, q))
                                                                         .Index(Constants.MovieDataIndexName));

            if (!searchResponse.IsValid)
            {
                response.Error($"Search failed with error: {searchResponse.DebugInformation}");
            }
            else
            {
                response.Data.Years = searchResponse.Aggregations
                                                    .Terms("year_aggregation")
                                                    .Buckets
                                                    .ToDictionary(x => int.Parse(x.Key), x => x.DocCount ?? 0)
                                                    .ToList();

                response.Data.Genres = searchResponse.Aggregations.Nested("nested_genres")
                                                    .Terms("genre_aggregation")
                                                    .Buckets
                                                    .ToDictionary(x => x.Key, x => x.DocCount ?? 0)
                                                    .ToList();

                response.Data.Movies = searchResponse.Documents.ToList();
                response.Data.TotalCount = searchResponse.Total;
            }
            return response;
        }

        private SearchDescriptor<Movie> AggregateMovies(SearchDescriptor<Movie> searchDescriptor)
        {
            return searchDescriptor.Aggregations(x => x.Terms("year_aggregation", y => y.Field(z => z.Year))
                                                       .Nested("nested_genres", n => n.Path(p => p.Genres)
                                                                                      .Aggregations(na => na.Terms("genre_aggregation", t => t
                            .Field(f => f.Genres.First().Name)
                            .Size(10_000)))));
        }

        private QueryContainer QueryMovies(Entities.SearchRequest request, QueryContainerDescriptor<Movie> query)
        {
            var filters = new List<Func<QueryContainerDescriptor<Movie>, QueryContainer>>();

            if (request.Years.Any())
            {
                filters.Add(f => f.Terms(t => t.Field(fd => fd.Year).Terms(request.Years)));
            }

            if (request.Genres.Any())
            {
                filters.Add(f => f.Nested(n => n.Path(p => p.Genres).Query(q => q.Terms(t => t.Field(fd => fd.Genres.First().Name).Terms(request.Genres)))));
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();

                filters.Add(f => f.Match(m => m.Field(mf => mf.MovieName)
                                               .Query(searchTerm)
                                               .Fuzziness(Fuzziness.Auto)
                                               .MaxExpansions(10)));
            }

            return query.Bool(b => b.Filter(filters));
        }
    }
}
