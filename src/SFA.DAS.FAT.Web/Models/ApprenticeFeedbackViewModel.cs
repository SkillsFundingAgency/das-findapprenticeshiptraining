using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.FAT.Domain.Courses;

namespace SFA.DAS.FAT.Web.Models
{
    public class ApprenticeFeedbackViewModel
    {
        public int TotalFeedbackResponses { get; set; }
        public int TotalFeedbackRating { get; set; }
        public string TotalFeedbackRatingText { get; set; }
        public string TotalFeedbackRatingTextProviderDetail { get; set; }
        public ProviderRating TotalFeedbackText { get; set; }
        public List<ApprenticeFeedbackDetailViewModel> FeedbackAttributeSummary { get; set; }

        public ApprenticeFeedbackViewModel(ApprenticeFeedback apprenticeFeedback)
        {
            if (apprenticeFeedback == null)
                return;

            TotalFeedbackRating = apprenticeFeedback.TotalFeedbackRating;
            TotalFeedbackResponses = apprenticeFeedback.TotalApprenticeResponses;
            TotalFeedbackRatingText = GetFeedbackRatingText(false);
            TotalFeedbackRatingTextProviderDetail = GetFeedbackRatingText(true);
            TotalFeedbackText = (ProviderRating)apprenticeFeedback.TotalFeedbackRating;
            FeedbackAttributeSummary = GenerateAttributeSummary(apprenticeFeedback.FeedbackAttributes);
        }

        private string GetFeedbackRatingText(bool isProviderDetail)
        {
            switch (TotalFeedbackResponses)
            {
                case 0:
                    return !isProviderDetail ? "Not yet reviewed (apprentice reviews)" : "Not yet reviewed";
                case 1:
                    return !isProviderDetail ? "(1 apprentice review)" : "(1 review)";
            }

            var returnText = TotalFeedbackResponses > 50 && !isProviderDetail ? "(50+ apprentice reviews)"
                : $"({TotalFeedbackResponses} apprentice reviews)";

            return isProviderDetail ? returnText.Replace("apprentice ", "") : returnText;
        }

        private List<ApprenticeFeedbackDetailViewModel> GenerateAttributeSummary(List<ApprenticeFeedbackAttributeDetail> source)
        {
            var attributeSummary = new List<ApprenticeFeedbackDetailViewModel>();

            foreach (var entry in source)
            {
                int totalCount = entry.Agree + entry.Disagree;

                attributeSummary.Add(
                    new ApprenticeFeedbackDetailViewModel
                    {
                        Name = entry.Name,
                        Category = entry.Category,
                        AgreeCount = entry.Agree,
                        DisagreeCount = entry.Disagree,
                        TotalCount = totalCount,
                        AgreePerc = Math.Round((double)entry.Agree / totalCount * 100, 0),
                        DisagreePerc = Math.Round((double)entry.Disagree / totalCount * 100, 0)
                    });
            }

            return attributeSummary.OrderByDescending(o => o.TotalCount).ToList();
        }

        public class ApprenticeFeedbackDetailViewModel
        {
            public string Name { get; set; }
            public string Category { get; set; }
            public int AgreeCount { get; set; }
            public int DisagreeCount { get; set; }
            public int TotalCount { get; set; }
            public double AgreePerc { get; set; }
            public double DisagreePerc { get; set; }
        }
    }
}


