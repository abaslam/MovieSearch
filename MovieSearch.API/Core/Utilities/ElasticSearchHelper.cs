using Nest;

namespace MovieSearch.API.Core.Utilities
{
    public static class ElasticSearchHelper
    {
        public static AnalyzersDescriptor AddCustomKeywordLowercaseAnalyzer(this AnalyzersDescriptor analyzersDescriptor)
        {
            return analyzersDescriptor.Custom(
                        ElasticSearchHelper.GetCustomKeywordLowercaseAnalyzerName(),
                        cu => cu.Tokenizer("keyword")
                                .Filters("lowercase"));
        }

        public static AnalyzersDescriptor AddCustomKeywordAnalyzer(this AnalyzersDescriptor analyzersDescriptor)
        {
            return analyzersDescriptor.Custom(
                        ElasticSearchHelper.GetCustomKeywordAnalyzerName(),
                        cu => cu.Tokenizer("keyword"));
        }

        public static AnalyzersDescriptor AddCustomNGramAnalyzer(this AnalyzersDescriptor analyzersDescriptor)
        {
            return analyzersDescriptor.Custom(
                    ElasticSearchHelper.GetCustomNGramAnalyzerName(),
                    c => c.Tokenizer(ElasticSearchHelper.GetCustomNGramTokenizerName())
                          .Filters("lowercase", "asciifolding", ElasticSearchHelper.GetCustomNGramFilterName()));
        }

        public static TokenizersDescriptor AddCustomNGramTokenizer(this TokenizersDescriptor tokenizersDescriptor)
        {
            return tokenizersDescriptor.NGram(
                            ElasticSearchHelper.GetCustomNGramTokenizerName(),
                            td => td.MinGram(9)
                                    .MaxGram(10)
                                    .TokenChars(TokenChar.Letter, TokenChar.Digit, TokenChar.Punctuation, TokenChar.Symbol));
        }

        public static TokenFiltersDescriptor AddCustomNGramTokenFilter(this TokenFiltersDescriptor tokenFiltersDescriptor)
        {
            return tokenFiltersDescriptor.NGram(
                ElasticSearchHelper.GetCustomNGramFilterName(),
                ng => ng.MaxGram(10).MinGram(9));
        }

        public static AnalyzersDescriptor AddCustomPhoneticAnalyzer(this AnalyzersDescriptor analyzersDescriptor)
        {
            return analyzersDescriptor.Custom(
                ElasticSearchHelper.GetCustomPhoneticAnalyzerName(),
                c => c.Tokenizer("standard")
                      .Filters("lowercase", ElasticSearchHelper.GetCustomPhoneticFilterName()));
        }

        public static TokenFiltersDescriptor AddCustomPhoneticFilter(this TokenFiltersDescriptor tokenFiltersDescriptor)
        {
            return tokenFiltersDescriptor.Phonetic(
                    ElasticSearchHelper.GetCustomPhoneticFilterName(),
                    p => p.Encoder(PhoneticEncoder.DoubleMetaphone).Replace(true));
        }
       
        public static string GetCustomKeywordLowercaseAnalyzerName()
        {
            return "custom_guid_analyzer";
        }

        public static string GetCustomKeywordAnalyzerName()
        {
            return "custom_keyword_analyzer";
        }

        public static string GetCustomEmailAnalyzerName()
        {
            return "custom_email_analyzer";
        }

        public static string GetCustomNGramTokenizerName()
        {
            return "custom_ngram_tokenizer";
        }

        public static string GetCustomNGramFilterName()
        {
            return "custom_ngram_filter";
        }

        public static string GetCustomNGramAnalyzerName()
        {
            return "custom_ngram_analyzer";
        }

        public static string GetCustomPhoneticFilterName()
        {
            return "custom_phonetic_filter";
        }

        public static string GetCustomPhoneticAnalyzerName()
        {
            return "custom_phonetic_analyzer";
        }
    }
}
