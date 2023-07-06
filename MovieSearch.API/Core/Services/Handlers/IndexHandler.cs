using MediatR;
using MovieSearch.API.Core.Data.Entities;
using MovieSearch.API.Core.Infrstructure;
using MovieSearch.API.Core.Infrstructure.Entities;
using MovieSearch.API.Core.Services.Entities;
using MovieSearch.API.Core.Utilities;
using Nest;

namespace MovieSearch.API.Core.Services.Handlers
{
    public class IndexHandler : IRequestHandler<IndexRequest, Response<Entities.IndexResponse>>
    {
        private readonly Nest.IElasticClient elasticClient;
        private readonly ILogger<IndexHandler> logger;

        public IndexHandler(Nest.IElasticClient elasticClient, ILogger<IndexHandler> logger)
        {
            this.elasticClient = elasticClient;
            this.logger = logger;
        }
        public async Task<Response<Entities.IndexResponse>> Handle(IndexRequest request, CancellationToken cancellationToken)
        {
            var response = Response.Create<Entities.IndexResponse>();

            using (var indexStreamReader = new StreamReader(request.DataFile.OpenReadStream()))
            {
                var movies = new List<Movie>();
                while ((await indexStreamReader.ReadLineAsync()) is string movieLine)
                {
                    movies.Add(Movie.Parse(movieLine));
                }

                var indexName = Constants.MovieDataIndexName;

                if (request.DeleteIndex && (await this.elasticClient.Indices.ExistsAsync(Constants.MovieDataIndexName)).Exists)
                {
                    await this.elasticClient.Indices.DeleteAsync(Constants.MovieDataIndexName);
                }

                if (!(await this.elasticClient.Indices.ExistsAsync(indexName)).Exists)
                {
                    this.logger.LogDebug($"Index - {indexName} does not exist, creating index");

                    var createIndexResponse = await this.elasticClient.Indices.CreateAsync(
                                                                indexName,
                                                                i => i.Settings(s => s.NumberOfShards(2)
                                                                                      .NumberOfReplicas(2)

                                                                                      .Analysis(a => a.Tokenizers(t => t.AddCustomNGramTokenizer())

                                                                                                      .TokenFilters(t => t.AddCustomNGramTokenFilter()
                                                                                                                          .AddCustomPhoneticFilter())

                                                                                                      .Analyzers(az => az.AddCustomKeywordLowercaseAnalyzer()
                                                                                                                         .AddCustomNGramAnalyzer()
                                                                                                                         .AddCustomPhoneticAnalyzer())))

                                                                      .Map<Movie>(this.MapMovie));
                    this.logger.LogDebug(createIndexResponse.DebugInformation);

                    this.logger.LogDebug($"Completed creating index - {indexName}");
                }

                var pageIndex = 1;
                var pageSize = 1000;
                var pageCount = (int)Math.Ceiling(movies.Count / (double)pageSize);

                do
                {
                    var offset = (pageIndex - 1) * pageSize;

                    var queryStartTime = DateTime.Now;

                    this.logger.LogDebug($"Querying {pageIndex} page records - {queryStartTime.ToString()}");

                    var currentMovies = movies.Skip(offset).Take(pageSize);
                    
                    var indexStartTime = DateTime.Now;

                    this.logger.LogDebug($"Starting Index - {indexStartTime}");

                    await this.elasticClient.IndexManyAsync(currentMovies, indexName);

                    var indexCompletedTime = DateTime.Now;

                    this.logger.LogDebug($"Completed Index - {indexCompletedTime}");
                    this.logger.LogDebug($"Total time take - {indexCompletedTime.Subtract(indexStartTime).TotalSeconds}");

                    pageIndex += 1;
                }
                while (pageIndex <= pageCount);



                return response;
            }
        }

        public Nest.ITypeMapping MapMovie(Nest.TypeMappingDescriptor<Movie> mapping)
        {
            return mapping.AutoMap().Properties(p => p.Text(t => t.Name(n => n.MovieName)
                                                     .Fielddata()
                                                     .Fields(f => f.Text(ft => ft.Name("phonetic")
                                                                             .Analyzer(ElasticSearchHelper.GetCustomPhoneticAnalyzerName()))))
                                                      .Nested<Genre>(n => n.Name(mv => mv.Genres)
                                                                           .Properties(pp => pp.Text(t => t.Name(g => g.Name).Fielddata()))));
        }
    }
}
