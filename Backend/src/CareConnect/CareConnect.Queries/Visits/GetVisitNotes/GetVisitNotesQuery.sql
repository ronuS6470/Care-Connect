SELECT n.Id, n.VisitId, n.AuthorUserId, u.FirstName + ' ' + u.LastName AS AuthorFullName,
       n.Content, n.CreatedAtUtc
FROM VisitNotes n
INNER JOIN Users u ON u.Id = n.AuthorUserId
WHERE n.VisitId = @VisitId
ORDER BY n.CreatedAtUtc;
