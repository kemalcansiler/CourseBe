namespace Course.Application.DTOs;

public class FilterOptionDto
{
    public string Label { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public List<FilterBucketDto> Options { get; set; } = new();
}

public class FilterBucketDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}
