using MediatR;

namespace SFA.DAS.FAT.Application.Standards.Queries.GetStandards;

public record GetStandardsQuery() : IRequest<GetStandardsQueryResult>;
