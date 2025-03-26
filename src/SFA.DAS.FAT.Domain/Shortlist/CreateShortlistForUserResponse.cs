using System;

namespace SFA.DAS.FAT.Domain.Shortlist;

public record CreateShortlistForUserResponse(Guid ShortlistId, bool IsCreated);
