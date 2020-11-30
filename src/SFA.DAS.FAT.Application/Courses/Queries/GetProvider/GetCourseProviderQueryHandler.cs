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
    public class GetCourseProviderQueryHandler : IRequestHandler<GetCourseProviderQuery, GetCourseProviderResult>
    {
        private readonly ICourseService _courseService;
        private readonly IValidator<GetCourseProviderQuery> _validator;
        public GetCourseProviderQueryHandler(IValidator<GetCourseProviderQuery> validator, ICourseService courseService)
        {
            _validator = validator;
            _courseService = courseService;
        }

        public async Task<GetCourseProviderResult> Handle(GetCourseProviderQuery query, CancellationToken cancellationToken)
        {

            var validationResult = await _validator.ValidateAsync(query);
            if (!validationResult.IsValid())
            {
                throw new ValidationException(validationResult.DataAnnotationResult, null, null);
            }

            var response = await _courseService.GetCourseProviderDetails(query.ProviderId, query.CourseId, query.Location, query.Lat, query.Lon);

            return new GetCourseProviderResult
            {
                Provider = response?.CourseProviderDetails,
                Course = response?.TrainingCourse,
                AdditionalCourses = response?.AdditionalCourses,
                Location = response?.Location?.Name,
                LocationGeoPoint = response?.Location?.LocationPoint?.GeoPoint,
                ProvidersAtLocation = response?.ProvidersCount?.ProvidersAtLocation ?? 0
            };
        }
    }
}
