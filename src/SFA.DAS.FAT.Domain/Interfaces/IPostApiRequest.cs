namespace SFA.DAS.FAT.Domain.Interfaces;

public interface IPostApiRequest<TData>
{
    string PostUrl { get; }

    TData Data { get; set; }
}
