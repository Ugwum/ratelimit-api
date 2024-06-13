namespace RateLimitLib.Configuration
{
    public class RateLimitOptions
    {
        public List<RateLimitRule> RateLimitRules { get; set; }
    }

    public class RateLimitRule
    {
        public string Endpoint { get; set; }
        public TimeSpan Period { get; set; }
        public int Limit { get; set; }
    }
}