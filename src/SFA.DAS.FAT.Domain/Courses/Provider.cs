using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SFA.DAS.FAT.Domain.Courses
{
    public class Provider
    {
        public uint ProviderId { get; set; }
        public string Name { get; set; }
        public string TradingName { get; set; }
        public string MarketingInfo { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Website { get; set; }
        public int? OverallCohort { get; set; }
        public decimal? OverallAchievementRate { get; set; }
        public int? NationalOverallCohort { get; set; }
        public decimal? NationalOverallAchievementRate { get; set; }
        public Guid? ShortlistId { get; set; }
        public ProviderAddress ProviderAddress { get; set; }
        public IEnumerable<DeliveryMode> DeliveryModes { get; set; }
        public EmployerFeedback EmployerFeedback { get; set; }
        public ApprenticeFeedback ApprenticeFeedback { get; set; }
    }

    public class DeliveryMode
    {
        public DeliveryModeType DeliveryModeType { get; set; }
        public decimal DistanceInMiles { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string County { get; set; }
        public bool National { get; set; }
    }

    public enum DeliveryModeType
    {
        Workplace = 0,
        DayRelease = 1,
        BlockRelease = 2,
        NotFound = 3,
        National = 4
    }

    public enum ProviderDeliveryMode
    {
        [Description("At apprentice’s workplace")]
        Workplace = 0,
        [Description("At training provider's location")]
        Provider = 1,
        [Description("Day release")]
        DayRelease = 2,
        [Description("Block release")]
        BlockRelease = 3

    }

    public enum ProviderRating
    {
        [Description("Not yet reviewed")]
        NotYetReviewed = 0,
        [Description("Very poor")]
        VeryPoor = 1,
        [Description("Poor")]
        Poor = 2,
        [Description("Good")]
        Good = 3,
        [Description("Excellent")]
        Excellent = 4
    }

    public enum QarRating
    {
        [Description("No achievement rate")]
        None,
        [Description("Above 70%")]
        Excellent,
        [Description("60% to 70%")]
        Good,
        [Description("50% to 59%")]
        Poor,
        [Description("Less than 50%")]
        VeryPoor
    }

    public enum ProviderOrderBy
    {
        Distance,
        AchievementRate,
        EmployerProviderRating,
        ApprenticeProviderRating
    }

    public class EmployerFeedback
    {
        public int TotalEmployerResponses { get; set; }
        public int TotalFeedbackRating { get; set; }
        public IEnumerable<EmployerFeedbackDetail> FeedbackDetail { get; set; }
        public List<EmployerFeedbackAttributeDetail> FeedbackAttributes { get; set; }
    }

    public class ApprenticeFeedback
    {
        public int TotalApprenticeResponses { get; set; }
        public int TotalFeedbackRating { get; set; }
        public List<ApprenticeFeedbackAttributeDetail> FeedbackAttributes { get; set; }
    }

    public class EmployerFeedbackDetail
    {
        public string FeedbackName { get; set; }
        public int FeedbackCount { get; set; }
    }

    public class EmployerFeedbackAttributeDetail
    {
        public string AttributeName { get; set; }
        public int Strength { get; set; }
        public int Weakness { get; set; }
    }

    public class ApprenticeFeedbackAttributeDetail
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public int Agree { get; set; }
        public int Disagree { get; set; }
    }

    public class ProviderAddress
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public decimal? DistanceInMiles { get; set; }
    }
}
