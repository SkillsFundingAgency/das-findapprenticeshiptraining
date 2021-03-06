﻿using System;
using MediatR;

namespace SFA.DAS.FAT.Application.Shortlist.Commands.CreateShortlistItemForUser
{
    public class CreateShortlistItemForUserCommand : IRequest<Guid>
    {
        public Guid ShortlistUserId { get ; set ; }
        public int Ukprn { get ; set ; }
        public int TrainingCode { get ; set ; }
        public double? Lat { get ; set ; }
        public double? Lon { get ; set ; }
        public string LocationDescription { get ; set ; }
    }
}