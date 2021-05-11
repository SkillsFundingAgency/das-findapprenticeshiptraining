using SFA.DAS.FAT.Domain.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.FAT.Web.Models
{
    public class CoursesViewModel
    {
        private OrderBy _orderBy = OrderBy.None;
        public List<CourseViewModel> Courses { get; set; }
        public string Keyword { get; set; }
        public int Total { get ; set ; }
        public int TotalFiltered { get ; set ; }
        public List<LevelViewModel> Levels { get; set; }
        public List<SectorViewModel> Sectors { get ; set ; }
        public List<string> SelectedSectors { get ; set ; }
        public List<int> SelectedLevels { get ; set ; }
        public OrderBy OrderBy
        {
            get => _orderBy;
            set
            {
                if (value == OrderBy.None && !string.IsNullOrEmpty(Keyword))
                {
                    _orderBy = OrderBy.Relevance;
                }
                else if (value != OrderBy.None && string.IsNullOrEmpty(Keyword))
                {
                    _orderBy = OrderBy.None;
                }
                else
                {
                    _orderBy = value;    
                }
            }
        }

        public bool ShowFilterOptions =>  ClearSectorLinks.Any() || ClearLevelLinks.Any() || !string.IsNullOrEmpty(Keyword);

        public string TotalMessage => GetTotalMessage();
        public Dictionary<string, string> ClearSectorLinks => BuildClearSectorFilterLinks();
        public string ClearKeywordLink => BuildClearKeywordFilterLink();
        public Dictionary<string, string> ClearLevelLinks => BuildClearLevelFilterLinks();
        public string OrderByName => BuildOrderByLink(OrderBy.Name);
        public string OrderByRelevance => BuildOrderByLink(OrderBy.Relevance);
        public int ShortlistItemCount { get ; set ; }
        public string Location { get ; set ; }

        private string GetTotalMessage()
        {
            var totalToUse = string.IsNullOrEmpty(Keyword) 
                             && (SelectedSectors == null || !SelectedSectors.Any()) 
                             && (SelectedLevels == null || !SelectedLevels.Any())
                                    ? Total 
                                    : TotalFiltered;

            return $"{totalToUse} result" + (totalToUse!=1 ? "s": "");
        }
        private string BuildOrderByLink(OrderBy order)
        {
            var buildOrderByNameLink = !string.IsNullOrEmpty(Keyword) ? $"?keyword={Keyword}" : "";

            buildOrderByNameLink += !string.IsNullOrEmpty(order.ToString()) ? $"{GetSeparator(buildOrderByNameLink)}orderby={order}" : "";

            buildOrderByNameLink += BuildSelectedSectorListLink(buildOrderByNameLink);
            
            buildOrderByNameLink += BuildSelectedLevelsListLink(buildOrderByNameLink);
            
            return buildOrderByNameLink;
        }

        private string BuildClearKeywordFilterLink()
        {
            var buildClearKeywordFilterLink = BuildSelectedSectorListLink("");
            buildClearKeywordFilterLink += BuildSelectedLevelsListLink(buildClearKeywordFilterLink);
            return buildClearKeywordFilterLink;
        }

        private Dictionary<string, string> BuildClearSectorFilterLinks ( )
        {
            var clearFilterLinks = new Dictionary<string, string>();
            if (SelectedSectors?.FirstOrDefault() == null)
            {
                return clearFilterLinks;
            }
            
            var levels = BuildSelectedLevelsListLink("appendTo");
            
            foreach (var selectedSector in SelectedSectors)
            {
                var clearFilterString = BuildClearFilterStringForKeywordAndOrderBy();

                clearFilterString += $"{GetSeparator(clearFilterString)}sectors=" + string.Join("&sectors=", SelectedSectors.Where(c => !c.Equals(selectedSector, StringComparison.CurrentCultureIgnoreCase)).Select(HttpUtility.HtmlEncode));
                clearFilterString += levels;
                
                var sector = Sectors.SingleOrDefault(c => c.Route.Equals(selectedSector, StringComparison.CurrentCultureIgnoreCase));
                if (sector != null)
                {
                    clearFilterLinks.Add(sector.Route, clearFilterString);    
                }
                
            }

            return clearFilterLinks;
        }

        private Dictionary<string, string> BuildClearLevelFilterLinks()
        {
            var clearLevelLink = new Dictionary<string,string>();
            if (SelectedLevels == null || !SelectedLevels.Any())
            {
                return clearLevelLink;
            }

            var sectors = BuildSelectedSectorListLink("appendTo");
            
            foreach (var selectedLevel in SelectedLevels)
            {
                var clearFilterString = BuildClearFilterStringForKeywordAndOrderBy();

                clearFilterString += $"{GetSeparator(clearFilterString)}levels=" + string.Join("&levels=", SelectedLevels.Where(c => !c.Equals(selectedLevel)));
                clearFilterString += sectors;
                var level = Levels.SingleOrDefault(c => c.Code.Equals(selectedLevel));
                if(level != null)
                {
                    clearLevelLink.Add(level.Title, clearFilterString);    
                }
            }
            
            return clearLevelLink;
        }

        private string BuildClearFilterStringForKeywordAndOrderBy()
        {
            var clearFilterString = "";
            
            if (!string.IsNullOrEmpty(Keyword))
            {
                clearFilterString = $"?keyword={Keyword}";
            }

            if (OrderBy != OrderBy.None)
            {
                clearFilterString += $"{GetSeparator(clearFilterString)}orderby={OrderBy}";
            }

            return clearFilterString;
        }

        private string BuildSelectedLevelsListLink(string linkToAppendTo)
        {
            return SelectedLevels != null && SelectedLevels.Any() ? $"{GetSeparator(linkToAppendTo)}levels=" + string.Join("&levels=", SelectedLevels) : "";
        }

        private string BuildSelectedSectorListLink(string linkToAppendTo)
        {
            return SelectedSectors != null && SelectedSectors.Any() ? $"{GetSeparator(linkToAppendTo)}sectors=" + string.Join("&sectors=", SelectedSectors.Select(HttpUtility.HtmlEncode)) : "";
        }

        private string GetSeparator(string url)
        {
            return string.IsNullOrEmpty(url) ? "?" : "&";
        }
    }
}
