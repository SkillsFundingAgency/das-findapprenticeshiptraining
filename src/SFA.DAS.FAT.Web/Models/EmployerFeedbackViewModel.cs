using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;
using SFA.DAS.FAT.Domain.Extensions;

namespace SFA.DAS.FAT.Web.Models
{
    public class EmployerFeedbackViewModel
    {
        public int TotalFeedbackResponses { get; set; }
        public int TotalFeedbackRating { get; set; }
        public string TotalFeedbackRatingText { get; set; }
        public string TotalFeedbackRatingTextProviderDetail { get; set; }
        public ProviderRating TotalFeedbackText { get; set; }
        public List<EmployerFeedbackDetailViewModel> FeedbackAttributeSummary { get; set; }

        public EmployerFeedbackViewModel(EmployerFeedback employerFeedback)
        {
            if (employerFeedback == null)
                return;

            TotalFeedbackRating = employerFeedback.TotalFeedbackRating;
            TotalFeedbackResponses = employerFeedback.TotalEmployerResponses;
            TotalFeedbackRatingText = GetFeedbackRatingText(false);
            TotalFeedbackRatingTextProviderDetail = GetFeedbackRatingText(true);
            TotalFeedbackText = (ProviderRating)employerFeedback.TotalFeedbackRating;
            FeedbackAttributeSummary = GenerateAttributeSummary(employerFeedback.FeedbackAttributes);
        }

        private string GetFeedbackRatingText(bool isProviderDetail)
        {
            switch (TotalFeedbackResponses)
            {
                case 0:
                    return !isProviderDetail ? "Not yet reviewed (employer reviews)" : "Not yet reviewed";
                case 1:
                    return !isProviderDetail ? "(1 employer review)" : "(1 review)";
            }

            var returnText = TotalFeedbackResponses > 50 && !isProviderDetail ? "(50+ employer reviews)"
                : $"({TotalFeedbackResponses} employer reviews)";

            return isProviderDetail ? returnText.Replace("employer ", "") : returnText;
        }

        private List<EmployerFeedbackDetailViewModel> GenerateAttributeSummary(List<EmployerFeedbackAttributeDetail> source)
        {
            List<EmployerFeedbackDetailViewModel> AttributeSummary = new List<EmployerFeedbackDetailViewModel>();

            foreach (var entry in source)
            {
                int totalCount = entry.Strength + entry.Weakness;

                AttributeSummary.Add(
                    new EmployerFeedbackDetailViewModel
                    {
                        AttributeName = entry.AttributeName,
                        StrengthCount = entry.Strength,
                        WeaknessCount = entry.Weakness,
                        TotalCount = totalCount,
                        StrengthPerc = Math.Round((double)entry.Strength / totalCount * 100, 0),
                        WeaknessPerc = Math.Round((double)entry.Weakness / totalCount * 100, 0)
                    });
            }

            return AttributeSummary.OrderByDescending(o => o.TotalCount).ToList();
        }

        public class EmployerFeedbackDetailViewModel
        {
            public string AttributeName { get; set; }
            public int StrengthCount { get; set; }
            public int WeaknessCount { get; set; }
            public int TotalCount { get; set; }
            public double StrengthPerc { get; set; }
            public double WeaknessPerc { get; set; }
        }

        public class EmployerFeedBackDetail
        {
            public ProviderRating Rating { get; set; }
            public decimal RatingPercentage { get; set; }
            public int RatingCount { get; set; }
            public string RatingText => GetRatingText();

            private string GetRatingText()
            {
                return RatingCount == 1 ? "1 review" : $"{RatingCount} reviews";
            }
        }
    }
}


