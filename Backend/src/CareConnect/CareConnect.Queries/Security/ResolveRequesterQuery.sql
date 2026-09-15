SELECT u.Id AS UserId, u.Role, cl.Id AS ClientId, cg.Id AS CaregiverId
FROM Users u
LEFT JOIN Clients cl ON cl.UserId = u.Id
LEFT JOIN Caregivers cg ON cg.UserId = u.Id
WHERE u.Auth0UserId = @Auth0UserId;
