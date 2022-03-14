﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ExpertPriceCrawler.Web
{
    public class CrawlJobPost
    {
        [Required]
        public string Url { get; set; } = null!;
        [Required]
        public string EmailAddress { get; set; } = null!;
    }

    public class CrawlJob
    {
        public string Url { get; set; }
        public string EmailAddress { get; set; }
        public DateTime TimeCreated { get; } = DateTime.UtcNow;
        public DateTime TimeCompleted { get; set; }
        public Guid Id { get; } = Guid.NewGuid();
        public string ResultTableHtml { get; set; }

        public CrawlJob(CrawlJobPost input)
        {
            Url = input.Url;
            EmailAddress = input.EmailAddress;
        }
    }
}