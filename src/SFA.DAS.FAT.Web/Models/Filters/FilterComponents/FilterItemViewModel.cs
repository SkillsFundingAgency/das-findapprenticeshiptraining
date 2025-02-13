namespace SFA.DAS.FAT.Web.Models.Filters.FilterComponents;

public sealed class FilterItemViewModel
{
    public string? Value { get; set; }
    public required string DisplayText { get; set; } = string.Empty;
    public bool Selected { get; set; } = false;
}
