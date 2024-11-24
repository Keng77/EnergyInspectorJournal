using Xunit;
using InspectorJournal.Controllers;
using Microsoft.AspNetCore.Mvc;
using InspectorJournal.DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using InspectorJournal.DataLayer.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class InspectionsControllerTests
    {
        private readonly InspectionsDbContext _context;
        private readonly InspectionsController _controller;

        public InspectionsControllerTests()
        {
            // Using InMemory database for testing
            var options = new DbContextOptionsBuilder<InspectionsDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new InspectionsDbContext(options);
            _controller = new InspectionsController(_context);

            _context.Database.EnsureDeleted(); // Ensure the database is deleted before each test
            _context.Database.EnsureCreated(); // Ensure the database is created

            SeedData(); // Add initial data for testing
        }

        // Seed some test data into the InMemory database
        private void SeedData()
        {
            _context.Enterprises.AddRange(
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
                }
            );

            _context.ViolationTypes.AddRange(
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
                }
            );

            _context.Inspectors.AddRange(
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
                }
            );

            _context.Inspections.AddRange(
                new Inspection
                {
                    InspectionId = 1,
                    InspectionDate = DateOnly.FromDateTime(DateTime.Now),
                    ProtocolNumber = "PRT-001",
                    ViolationTypeId = 1,
                    ResponsiblePerson = "John Doe",
                    PenaltyAmount = 1000,
                    PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                    CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
                    PaymentStatus = "Unpaid",
                    CorrectionStatus = "Pending",
                    EnterpriseId = 1,
                    InspectorId = 1

                }
            );

            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateReturnsViewWithSelectLists()
        {
            // Act
            var result = await _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.NotNull(viewResult.Model);

            var viewData = viewResult.ViewData;
            Assert.NotNull(viewData["EnterpriseId"]);
            Assert.NotNull(viewData["ViolationTypeId"]);
            Assert.NotNull(viewData["InspectorId"]);
        }

        [Fact]
        public async Task Create_Post_Returns_Redirect_To_Index_When_ModelState_Is_Valid()
        {
            // Arrange
            var inspection = new Inspection
            {
                InspectionId = 2, // New ID
                InspectionDate = DateOnly.FromDateTime(DateTime.Now),
                ProtocolNumber = "PRT-002",
                ViolationTypeId = 1,
                ResponsiblePerson = "Jane Doe",
                PenaltyAmount = 500,
                PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                PaymentStatus = "Unpaid",
                CorrectionStatus = "Pending",
                EnterpriseId = 1,
                InspectorId = 2
            };

            // Act
            var result = await _controller.Create(inspection);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Check if the inspection was added
            var createdInspection = _context.Inspections.FirstOrDefault(i => i.InspectionId == 2);
            Assert.NotNull(createdInspection);
        }

        [Fact]
        public async Task Edit_Returns_View_With_Inspection_When_Exists()
        {
            // Arrange
            var inspectionId = 1;
            var inspection = _context.Inspections.FirstOrDefault(i => i.InspectionId == inspectionId);

            // Act
            var result = await _controller.Edit(inspectionId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<Inspection>(viewResult.Model);
            Assert.Equal(inspectionId, model.InspectionId);
        }

        [Fact]
        public async Task Create_Post_Returns_View_When_ModelState_Is_Invalid()
        {
            // Arrange
            var inspection = new Inspection
            {
                InspectionId = 2, // New ID
                InspectionDate = DateOnly.FromDateTime(DateTime.Now),
                ProtocolNumber = "PRT-002",
                ViolationTypeId = 1, // Valid ViolationTypeId, but ensure that we test with a missing Enterprise or Inspector
                ResponsiblePerson = "Jane Doe",
                PenaltyAmount = 500,
                PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
                CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                PaymentStatus = "Unpaid",
                CorrectionStatus = "Pending",
                EnterpriseId = 99, // Invalid EnterpriseId that does not exist in the database
                InspectorId = 2
            };

            // Act
            var result = await _controller.Create(inspection);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);  // Ensure the result is a ViewResult
            Assert.NotNull(viewResult.Model);  // Ensure that the model is returned in the view
            var model = Assert.IsType<Inspection>(viewResult.Model);  // Ensure the model is of type Inspection
            Assert.Equal(inspection.ProtocolNumber, model.ProtocolNumber);  // Check that invalid data is retained in the model
            Assert.False(_controller.ModelState.IsValid);  // Assert that the model state is invalid

            // Ensure that the errors related to missing EnterpriseId are added to the ModelState
            var enterpriseError = _controller.ModelState["EnterpriseId"]?.Errors?.FirstOrDefault()?.ErrorMessage;
            Assert.Equal("Предприятие не найдено.", enterpriseError);  // Explicitly check the error message
        }




        [Fact]
        public async Task Edit_Returns_NotFound_When_Inspection_Does_Not_Exist()
        {
            // Arrange
            var inspectionId = 999; // Non-existing ID

            // Act
            var result = await _controller.Edit(inspectionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Edit_Post_Returns_Redirect_To_Index_When_ModelState_Is_Valid()
        {
            // Arrange
            var inspection = _context.Inspections.First();
            inspection.ResponsiblePerson = "Updated Responsible Person"; // Modify an existing field

            // Act
            var result = await _controller.Edit(inspection.InspectionId, inspection);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Check if the inspection was updated
            var updatedInspection = _context.Inspections.FirstOrDefault(i => i.InspectionId == inspection.InspectionId);
            Assert.Equal("Updated Responsible Person", updatedInspection.ResponsiblePerson);
        }

        [Fact]
        public async Task Delete_Returns_NotFound_When_Inspection_Does_Not_Exist()
        {
            // Arrange
            var inspectionId = 999; // Non-existing ID

            // Act
            var result = await _controller.Delete(inspectionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_Post_Returns_Redirect_To_Index_When_Inspection_Is_Deleted()
        {
            // Arrange
            var inspectionId = 1; // Existing ID to delete
            var inspection = _context.Inspections.FirstOrDefault(i => i.InspectionId == inspectionId);

            // Act
            var result = await _controller.DeleteConfirmed(inspectionId);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Check if the inspection was deleted
            var deletedInspection = _context.Inspections.FirstOrDefault(i => i.InspectionId == inspectionId);
            Assert.Null(deletedInspection);
        }

        [Fact]
        public async Task Create_Post_Returns_View_When_Inspector_Is_Missing()
        {
            // Arrange
            var inspection = new Inspection
            {
                InspectionId = 3, // New ID
                InspectionDate = DateOnly.FromDateTime(DateTime.Now),
                ProtocolNumber = "PRT-003",
                ViolationTypeId = 1, // Valid ViolationTypeId
                ResponsiblePerson = "John Doe",
                PenaltyAmount = 1500,
                PaymentDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(10)),
                CorrectionDeadline = DateOnly.FromDateTime(DateTime.Now.AddDays(20)),
                PaymentStatus = "Unpaid",
                CorrectionStatus = "Pending",
                EnterpriseId = 1, // Valid EnterpriseId
                InspectorId = 99 // Invalid InspectorId (does not exist in the database)
            };

            // Act
            var result = await _controller.Create(inspection);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);  // Ensure the result is a ViewResult
            Assert.NotNull(viewResult.Model);  // Ensure that the model is returned in the view
            var model = Assert.IsType<Inspection>(viewResult.Model);  // Ensure the model is of type Inspection
            Assert.Equal(inspection.ProtocolNumber, model.ProtocolNumber);  // Check that invalid data is retained in the model
            Assert.False(_controller.ModelState.IsValid);  // Assert that the model state is invalid

            // Ensure that the errors related to missing InspectorId are added to the ModelState
            var inspectorError = _controller.ModelState["InspectorId"]?.Errors?.FirstOrDefault()?.ErrorMessage;
            Assert.Equal("Инспектор не найден.", inspectorError);  // Explicitly check the error message for missing InspectorId
        }


    }
}
