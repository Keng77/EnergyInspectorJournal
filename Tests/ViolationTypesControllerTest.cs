using InspectorJournal.Controllers;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class ViolationTypesControllerTest
    {
        private InspectionsDbContext GetInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<InspectionsDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new InspectionsDbContext(options);
        }

        [Fact]
        public async Task GetViolationTypesList()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetViolationTypesList)); // Initialize InMemory context
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList()); // Add test data
            context.SaveChanges();

            var httpContext = new DefaultHttpContext(); // Create HTTP context
            httpContext.Session = new TestSession();   // Initialize session

            var controller = new ViolationTypesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = await controller.Index(); // Call Index method

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Ensure result is a view
            Assert.NotNull(viewResult);

            var model = Assert.IsAssignableFrom<ViolationTypesViewModel>(viewResult.ViewData.Model); // Check model
            Assert.NotNull(model);
            Assert.Equal(3, model.ViolationTypes.Count()); // Ensure the count matches expected
        }

        [Fact]
        public async Task GetViolationType()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetViolationType));
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList());
            context.SaveChanges();

            var controller = new ViolationTypesController(context);

            // Act
            var notFoundResult = await controller.Details(4); // Non-existent ID
            var foundResult = await controller.Details(1);   // Existing ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult); // Assert for non-existent ID
            var viewResult = Assert.IsType<ViewResult>(foundResult);
            var model = Assert.IsAssignableFrom<ViolationType>(viewResult.ViewData.Model);
            Assert.Equal("Safety Violation", model.Name); // Change expected value to match actual data
        }


        [Fact]
        public async Task Create_ReturnsViewResult_GivenInvalidModel()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsViewResult_GivenInvalidModel));
            var controller = new ViolationTypesController(context);
            controller.ModelState.AddModelError("error", "some error"); // Add model error

            var newViolationType = new ViolationType
            {
                ViolationTypeId = 4,
                Name = "New Violation",
                PenaltyAmount = 100,
                CorrectionPeriodDays = 30
            };

            // Act
            var result = await controller.Create(newViolationType);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Expecting ViewResult
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<ViolationType>(viewResult.ViewData.Model);
            Assert.Equal(newViolationType.Name, model.Name); // Ensure the data is passed back correctly
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsARedirectAndCreate_WhenModelStateIsValid));
            var controller = new ViolationTypesController(context);

            var newViolationType = new ViolationType
            {
                ViolationTypeId = 4,
                Name = "New Violation",
                PenaltyAmount = 100,
                CorrectionPeriodDays = 30
            };

            // Act
            var result = await controller.Create(newViolationType);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Validate that the new violation type is added to the database
            var addedViolationType = context.ViolationTypes.FirstOrDefault(v => v.ViolationTypeId == 4);
            Assert.NotNull(addedViolationType);
            Assert.Equal("New Violation", addedViolationType.Name);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsNotFound));
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList());
            context.SaveChanges();

            var controller = new ViolationTypesController(context);

            // Act
            var notFoundResult = await controller.Edit(4); // Non-existent ID
            var foundResult = await controller.Edit(3);   // Existing ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var violationTypes = TestDataHelper.GetFakeViolationTypesList();
            var violationTypesContextMock = new Mock<InspectionsDbContext>();
            violationTypesContextMock.Setup(x => x.ViolationTypes).ReturnsDbSet(violationTypes);

            var controller = new ViolationTypesController(violationTypesContextMock.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var invalidViolationType = new ViolationType
            {
                ViolationTypeId = 4, // Example ID, may not exist
                Name = "",           // Invalid field
                PenaltyAmount = 100,
                CorrectionPeriodDays = 30
            };
            var result = await controller.Edit(4, invalidViolationType);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<ViolationType>(viewResult.ViewData.Model);
            Assert.Equal(invalidViolationType.Name, model.Name);  // Check that the model returned with errors
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid));
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList());
            context.SaveChanges();

            var controller = new ViolationTypesController(context);

            // Object to update
            var updatedViolationType = context.ViolationTypes.First(v => v.ViolationTypeId == 3);
            updatedViolationType.Name = "Updated Violation";
            updatedViolationType.PenaltyAmount = 150;

            // Act
            var result = await controller.Edit(3, updatedViolationType);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Check updated data
            var violationTypeFromDb = context.ViolationTypes.Find(3);
            Assert.NotNull(violationTypeFromDb);
            Assert.Equal("Updated Violation", violationTypeFromDb.Name);
            Assert.Equal(150, violationTypeFromDb.PenaltyAmount);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Delete_ReturnsNotFound));
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList());
            context.SaveChanges();

            var controller = new ViolationTypesController(context);

            // Act
            var notFoundResult = await controller.Delete(4); // Non-existent ID
            var foundResult = await controller.Delete(3);   // Existing ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Delete_ReturnsARedirectAndDelete));
            context.ViolationTypes.AddRange(TestDataHelper.GetFakeViolationTypesList());
            context.SaveChanges();

            var controller = new ViolationTypesController(context);

            // Act
            var result = await controller.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var deletedViolationType = context.ViolationTypes.Find(3);
            Assert.Null(deletedViolationType);
        }
    }
}
