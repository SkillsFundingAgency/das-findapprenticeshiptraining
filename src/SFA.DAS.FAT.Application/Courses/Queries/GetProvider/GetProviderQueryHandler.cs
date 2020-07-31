﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.FAT.Domain.Interfaces;
using SFA.DAS.FAT.Domain.Validation;

namespace SFA.DAS.FAT.Application.Courses.Queries.GetProvider
{
    public class GetProviderQueryHandler : IRequestHandler<GetProviderQuery, GetProviderResult>
    {
        private readonly ICourseService _courseService;
        private readonly IValidator<GetProviderQuery> _validator;
        public GetProviderQueryHandler(IValidator<GetProviderQuery> validator, ICourseService courseService)
        {
            _validator = validator;
            _courseService = courseService;
        }

        public async Task<GetProviderResult> Handle(GetProviderQuery query, CancellationToken cancellationToken)
        {

            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }

            var response = await _courseService.GetCourseProviderDetails(query.ProviderId);

            return new GetProviderResult { Provider = response?.CourseProviderDetails };
        }
    }
}
