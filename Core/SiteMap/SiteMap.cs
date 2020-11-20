using System;
using System.Collections.Generic;
using AspNetCore.SEOHelper.Sitemap;
using DataLayer.Tools;

namespace Parsia.Core.SiteMap
{
    public static class SiteMap
    {
        public static void GenerateSiteMap(string url, double priority, DateTime date, int frequencyId, string path)
        {
            try
            {
                var list = new List<SitemapNode>();
                var node = new SitemapNode
                {
                    Url = url,
                    Priority = priority,
                    LastModified = date
                };
                switch (frequencyId)
                {
                    case 1:
                        node.Frequency = SitemapFrequency.Always;
                        break;
                    case 2:
                        node.Frequency = SitemapFrequency.Daily;
                        break;
                    case 3:
                        node.Frequency = SitemapFrequency.Hourly;
                        break;
                    case 4:
                        node.Frequency = SitemapFrequency.Weekly;
                        break;
                    case 5:
                        node.Frequency = SitemapFrequency.Monthly;
                        break;
                    case 6:
                        node.Frequency = SitemapFrequency.Yearly;
                        break;
                    case 7:
                        node.Frequency = SitemapFrequency.Never;
                        break;
                    default:
                        node.Frequency = SitemapFrequency.Always;
                        break;
                }

                list.Add(node);
                new SitemapDocument().CreateSitemapXML(list, path);
            }
            catch (Exception ex)
            {
                ExceptionUtil.ExceptionHandler(ex, "SiteMapGenerator.GenerateSiteMap", null);
            }
        }
    }
}