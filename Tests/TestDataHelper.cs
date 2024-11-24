using InspectorJournal.DataLayer.Models;

namespace Tests
{
    internal class TestDataHelper
    {
        public static List<Enterprise> GetFakeEnterprisesList()
        {
            return
            [
                new Enterprise
                {
                    EnterpriseId = 1,
                    Name = "Enterprise One",
                    OwnershipType = "Private",
                    Address = "123 Main Street",
                    DirectorName = "John Doe",
                    DirectorPhone = "123-456-7890"
                },
                new Enterprise
                {
                    EnterpriseId = 2,
                    Name = "Enterprise Two",
                    OwnershipType = "Public",
                    Address = "456 Elm Street",
                    DirectorName = "Jane Smith",
                    DirectorPhone = "987-654-3210"
                },
                new Enterprise
                {
                    EnterpriseId = 3,
                    Name = "Enterprise Three",
                    OwnershipType = "Private",
                    Address = "789 Pine Avenue",
                    DirectorName = "Alice Brown",
                    DirectorPhone = "555-555-5555"
                }
            ];
        }

        public static List<Inspector> GetFakeInspectorsList()
        {
            return
            [
                new Inspector
                {
                    InspectorId = 1,
                    FullName = "Inspector One",
                    Department = "Department1"
                },
                new Inspector
                {
                    InspectorId = 2,
                    FullName = "Inspector Two",
                    Department = "Department2"
                },
                new Inspector
                {
                    InspectorId = 3,
                    FullName = "Inspector Three",
                    Department = "Department3"
                }
            ];
        }

        public static List<ViolationType> GetFakeViolationTypesList()
        {
            return
            [
                new ViolationType
                {
                    ViolationTypeId = 1,
                    Name = "Safety Violation",
                    PenaltyAmount = 1000,
                    CorrectionPeriodDays = 4
                },
                new ViolationType
                {
                    ViolationTypeId = 2,
                    Name = "Environmental Violation",
                    PenaltyAmount = 2000,
                    CorrectionPeriodDays = 8
                },
                new ViolationType
                {
                    ViolationTypeId = 3,
                    Name = "Financial Violation",
                    PenaltyAmount = 3000,
                    CorrectionPeriodDays = 15
                }
            ];
        }

        public static List<Inspection> GetFakeInspectionsList()
        {
            var enterprises = GetFakeEnterprisesList();
            var inspectors = GetFakeInspectorsList();
            var violationTypes = GetFakeViolationTypesList();

            return
            [
                new Inspection
                {
                    InspectionId = 1,
                    InspectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10)),
                    ProtocolNumber = "PRT-001",
                    ViolationTypeId = 1,
                    ResponsiblePerson = "John Doe",
                    PenaltyAmount = 5000,
                    PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                    CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
                    PaymentStatus = "Unpaid",
                    CorrectionStatus = "Pending",
                    EnterpriseId = 1,
                    InspectorId = 1,
                    ViolationType = violationTypes.FirstOrDefault(v => v.ViolationTypeId == 1),
                    Enterprise = enterprises.FirstOrDefault(e => e.EnterpriseId == 1),
                    Inspector = inspectors.FirstOrDefault(i => i.InspectorId == 1)
                },
                new Inspection
                {
                    InspectionId = 2,
                    InspectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
                    ProtocolNumber = "PRT-002",
                    ViolationTypeId = 2,
                    ResponsiblePerson = "Jane Smith",
                    PenaltyAmount = 3000,
                    PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                    CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                    PaymentStatus = "Paid",
                    CorrectionStatus = "Completed",
                    EnterpriseId = 2,
                    InspectorId = 2,
                    ViolationType = violationTypes.FirstOrDefault(v => v.ViolationTypeId == 2),
                    Enterprise = enterprises.FirstOrDefault(e => e.EnterpriseId == 2),
                    Inspector = inspectors.FirstOrDefault(i => i.InspectorId == 2)
                },
                new Inspection
                {
                    InspectionId = 3,
                    InspectionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-20)),
                    ProtocolNumber = "PRT-003",
                    ViolationTypeId = 3,
                    ResponsiblePerson = "Alice Brown",
                    PenaltyAmount = 10000,
                    PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(40)),
                    PaymentStatus = "Unpaid",
                    CorrectionStatus = "Pending",
                    EnterpriseId = 3,
                    InspectorId = 3,
                    ViolationType = violationTypes.FirstOrDefault(v => v.ViolationTypeId == 3),
                    Enterprise = enterprises.FirstOrDefault(e => e.EnterpriseId == 3),
                    Inspector = inspectors.FirstOrDefault(i => i.InspectorId == 3)
                }
            ];
        }
    }
}
