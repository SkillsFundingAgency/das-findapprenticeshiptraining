select pc.LarsCode, s.Title, p.Ukprn, p.LegalName, count(*) 
from ProviderCourseLocation pcl
inner join ProviderCourse pc on pc.Id = pcl.ProviderCourseId
inner join Standard s on s.LarsCode = pc.LarsCode
inner join Provider p on p.Id = pc.ProviderId
inner join ProviderLocation pl on pl.Id = pcl.ProviderLocationId and pl.LocationType = 0 
where pcl.HasBlockReleaseDeliveryOption = 1
group by pc.LarsCode, s.Title, p.Ukprn, p.LegalName
having count(*) > 2
