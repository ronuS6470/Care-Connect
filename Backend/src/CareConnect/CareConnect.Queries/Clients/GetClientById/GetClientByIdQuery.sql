SELECT c.Id, c.UserId, u.FirstName + ' ' + u.LastName AS FullName, u.Email, u.PhoneNumber,
       c.AddressLine1, c.AddressLine2, c.City, c.State, c.PostalCode,
       c.EmergencyContactName, c.EmergencyContactPhone, c.IsActive
FROM Clients c
INNER JOIN Users u ON u.Id = c.UserId
WHERE c.Id = @ClientId;
