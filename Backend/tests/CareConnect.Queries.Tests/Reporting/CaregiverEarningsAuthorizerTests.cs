using CareConnect.DTOs.Enums;
using CareConnect.Queries.Reporting;
using CareConnect.Queries.Security;
using FluentAssertions;
using Xunit;

namespace CareConnect.Queries.Tests.Reporting;

public class CaregiverEarningsAuthorizerTests
{
    [Fact]
    public void CanView_Admin_CanViewAnyCaregiver()
    {
        var requester = new RequesterContext { UserId = 1, Role = UserRole.Admin, CaregiverId = null, ClientId = null };

        CaregiverEarningsAuthorizer.CanView(requester, targetCaregiverId: 42).Should().BeTrue();
    }

    [Fact]
    public void CanView_CaregiverViewingOwnEarnings_ReturnsTrue()
    {
        var requester = new RequesterContext { UserId = 5, Role = UserRole.Caregiver, CaregiverId = 7, ClientId = null };

        CaregiverEarningsAuthorizer.CanView(requester, targetCaregiverId: 7).Should().BeTrue();
    }

    [Fact]
    public void CanView_CaregiverViewingAnotherCaregiver_ReturnsFalse()
    {
        var requester = new RequesterContext { UserId = 5, Role = UserRole.Caregiver, CaregiverId = 7, ClientId = null };

        CaregiverEarningsAuthorizer.CanView(requester, targetCaregiverId: 8).Should().BeFalse();
    }

    [Fact]
    public void CanView_Client_ReturnsFalse_RegardlessOfTarget()
    {
        var requester = new RequesterContext { UserId = 9, Role = UserRole.Client, CaregiverId = null, ClientId = 3 };

        CaregiverEarningsAuthorizer.CanView(requester, targetCaregiverId: 3).Should().BeFalse();
    }
}
