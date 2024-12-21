using System.Diagnostics.CodeAnalysis;

namespace core.domain.entity;

public class MultiMediaModel
{
    public long Id { get; set; }
    public string Url { get; set; }
    public MultiMediaType MediaType { get; set; }
    public string Alt { get; set; }
}

public enum MultiMediaType
{
    IMAGE,VIDEO,GIF,PDF
}

public class MultiMediaEqualityComparer : IEqualityComparer<MultiMediaModel>
{
    public bool Equals(MultiMediaModel? x, MultiMediaModel? y)
    {
        if (x == null && y == null) return true;
        if (x == null || y == null) return false;
        return x.Id == y.Id;
    }

    public int GetHashCode([DisallowNull] MultiMediaModel obj)
    {
        return obj.Id.GetHashCode();
    }
}
