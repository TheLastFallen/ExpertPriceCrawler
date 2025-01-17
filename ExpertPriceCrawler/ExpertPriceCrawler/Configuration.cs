﻿using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using PuppeteerSharp;
using Serilog;
using System.Collections.Concurrent;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExpertPriceCrawler
{
    public static class Configuration
    {
        public static MemoryCache MemoryCache = new MemoryCache(new MemoryCacheOptions());
        public static readonly ConfigurationValues Instance = new ConfigurationValues();
        public static ILogger Logger;

        public static IConfigurationRoot Init(string environment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("branches.json", optional: false)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            configuration.Bind("Options", Instance);
            configuration.Bind(Instance);

            Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

            return configuration;
        }
    }

    public class ConfigurationValues
    {

        /// <summary>
        /// CrawlerType. Allowed Values: ApiCrawler, BrowserCrawler
        /// </summary>
        public string CrawlerType { get; set; }
        /// <summary>
        /// Max allowed errors before the App is disabled for some minutes
        /// </summary>
        public int MaxErrorsAllowed = 10;
        /// <summary>
        /// Amount of retries if a request fails.
        /// </summary>
        public int Retries { get; set; }
        /// <summary>
        /// Useragent to use for chromium when requesting the website
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// max amount of parallel requests. Reduce this value if the system is lagging :P
        /// </summary>
        public int MaxParallelRequests { get; set; }

        /// <summary>
        /// Cache Results for some Minutes. Useful for Website so that subsequent requests of the same product won't be crawles again
        /// </summary>
        public int MemoryCacheMinutes { get; set; }

        /// <summary>
        /// Path + filename of file to store crawlhistory to.
        /// </summary>
        public string CompletedJobsJsonFilePath { get; set; } = "completedjobs.json";

        /// <summary>
        /// Options for Puppeteer launcher
        /// </summary>
        public LaunchOptions PuppeteerLaunchOptions { get; set; }

        /// <summary>
        /// Dictionary of expert branches in Form (BranchId, BranchName>)
        /// </summary>
        public Dictionary<string, string> Branches { get; set; }

        /// <summary>
        /// Defines the BaseUrl for Expert site.
        /// </summary>
        public string ExpertBaseUrl { get; set; } = "https://expert.de";

        /// <summary>
        /// Rate Limit for requests by ipaddress and url
        /// </summary>
        public int RateLimitInMinutes { get; set; } = 30;

        public readonly Regex PriceRegex = new Regex(@"[\d\,\.]+", RegexOptions.Compiled);
        public readonly Regex PriceRegexItemProp = new Regex("itemprop=\"price\".*?([\\d\\,\\.]+)", RegexOptions.Compiled);
        public readonly CultureInfo Culture = CultureInfo.InvariantCulture;
        public readonly DateTimeFormatInfo GermanDateFormatInfo = (new CultureInfo("de-DE")).DateTimeFormat;
        public readonly TimeZoneInfo GermanTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

        public const string CartIdPattern = "data-cart-id=\"(.+?)\"";
        public const string ArticleIdPattern = "data-article-id=\"(.+?)\"";
        public const string CsrfTokenPattern = "content=\"(.+?)\".*(csrf-token)";
        public const string WebCodePattern = "Web-Code:\\s*(\\d+)";
        public Regex CartIdRegex = new Regex(CartIdPattern, RegexOptions.Compiled);
        public Regex ArticleIdRegex = new Regex(ArticleIdPattern, RegexOptions.Compiled);
        public Regex CsrfTokenRegex = new Regex(CsrfTokenPattern, RegexOptions.Compiled);
        public Regex WebCodeRegex = new Regex(WebCodePattern, RegexOptions.Compiled);
        public string AddItemUrl = "/_api/shoppingcart/addItem";

        internal ConfigurationValues() { }
    }
}
