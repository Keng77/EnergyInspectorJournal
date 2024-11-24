using InspectorJournal.Controllers;
using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.ViewModels;
using InspectorJournal.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class InspectorsControllerTest
    {
        private InspectionsDbContext GetInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<InspectionsDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new InspectionsDbContext(options);
        }

        [Fact]
        public void GetInspectorList()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetInspectorList));
            context.Inspectors.AddRange(TestDataHelper.GetFakeInspectorsList()); // Добавляем тестовые данные
            context.SaveChanges();

            var httpContext = new DefaultHttpContext();
            httpContext.Session = new TestSession(); // Инициализация сессии

            var controller = new InspectorsController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = controller.Index(); // Выполнение метода Index

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Проверяем, что результат — это представление
            Assert.NotNull(viewResult);

            var model = Assert.IsAssignableFrom<InspectorsViewModel>(viewResult.ViewData.Model); // Проверяем модель
            Assert.NotNull(model);
            Assert.Equal(3, model.Inspectors.Count()); // Убедимся, что количество инспекторов соответствует ожидаемому
        }

        [Fact]
        public async Task GetInspectorDetails()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetInspectorDetails));
            context.Inspectors.AddRange(TestDataHelper.GetFakeInspectorsList());
            context.SaveChanges();

            var controller = new InspectorsController(context);

            // Act
            var notFoundResult = await controller.Details(4); // Non-existent ID
            var foundResult = await controller.Details(1);   // Existing ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult); // Assert for non-existent ID
            var viewResult = Assert.IsType<ViewResult>(foundResult);
            var model = Assert.IsAssignableFrom<Inspector>(viewResult.ViewData.Model);
            Assert.Equal("Inspector One", model.FullName);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_GivenInvalidModel()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsViewResult_GivenInvalidModel));
            var controller = new InspectorsController(context);
            controller.ModelState.AddModelError("error", "some error"); // Добавление ошибки в модель

            var newInspector = new Inspector
            {
                InspectorId = 4,
                FullName = "Inspector Four",
                Department = "Department Four"
            };

            // Act
            var result = await controller.Create(newInspector);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Ожидается ViewResult
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<Inspector>(viewResult.ViewData.Model);
            Assert.Equal(newInspector.FullName, model.FullName); // Убедиться, что данные переданы обратно
        }

        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsARedirectAndCreate_WhenModelStateIsValid));
            var controller = new InspectorsController(context);

            var newInspector = new Inspector
            {
                InspectorId = 4,
                FullName = "Inspector Four",
                Department = "Department Four"
            };

            // Act
            var result = await controller.Create(newInspector);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Validate that the new inspector is added to the database
            var addedInspector = context.Inspectors.FirstOrDefault(i => i.InspectorId == 4);
            Assert.NotNull(addedInspector);
            Assert.Equal("Inspector Four", addedInspector.FullName);
        }

        [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsNotFound));
            context.Inspectors.AddRange(TestDataHelper.GetFakeInspectorsList());
            context.SaveChanges();

            var controller = new InspectorsController(context);

            // Act
            var notFoundResult = await controller.Edit(4); // Несуществующий ID
            var foundResult = await controller.Edit(3);   // Существующий ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_GivenInvalidModel()
        {
            // Arrange
            var inspectors = TestDataHelper.GetFakeInspectorsList();
            var inspectorsContextMock = new Mock<InspectionsDbContext>();
            inspectorsContextMock.Setup(x => x.Inspectors).ReturnsDbSet(inspectors);

            var controller = new InspectorsController(inspectorsContextMock.Object);
            controller.ModelState.AddModelError("FullName", "FullName is required");

            // Act
            var invalidInspector = new Inspector
            {
                InspectorId = 4, // Пример ID, который может не существовать
                FullName = "",   // Ошибка в обязательном поле
                Department = "Test Department"
            };
            var result = await controller.Edit(4, invalidInspector);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Inspector>(viewResult.ViewData.Model);
            Assert.Equal(invalidInspector.FullName, model.FullName);  // Проверка, что модель вернулась с ошибками
        }

        [Fact]
        public async Task Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid));
            var inspectors = TestDataHelper.GetFakeInspectorsList(); // Получаем список инспекторов
            context.Inspectors.AddRange(inspectors); // Добавляем инспекторов в контекст
            context.SaveChanges(); // Сохраняем изменения в контексте

            var controller = new InspectorsController(context);

            // Объект обновления
            var updatedInspector = context.Inspectors.First(i => i.InspectorId == 3);
            updatedInspector.FullName = "Updated Inspector";
            updatedInspector.Department = "Updated Department";

            // Act
            var result = await controller.Edit(3, updatedInspector); // Обновляем инспектора с ID 3

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Проверяем обновленные данные
            var inspectorFromDb = context.Inspectors.Find(3);
            Assert.NotNull(inspectorFromDb);
            Assert.Equal("Updated Inspector", inspectorFromDb.FullName);
            Assert.Equal("Updated Department", inspectorFromDb.Department);
        }


        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Delete_ReturnsNotFound));
            context.Inspectors.AddRange(TestDataHelper.GetFakeInspectorsList());
            context.SaveChanges();

            var controller = new InspectorsController(context);

            // Act
            var notFoundResult = await controller.Delete(4); // Несуществующий ID
            var foundResult = await controller.Delete(3);   // Существующий ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult);
            Assert.IsType<ViewResult>(foundResult);
        }

        [Fact]
        public async Task Delete_ReturnsARedirectAndDelete()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Delete_ReturnsARedirectAndDelete));
            context.Inspectors.AddRange(TestDataHelper.GetFakeInspectorsList());
            context.SaveChanges();

            var controller = new InspectorsController(context);

            // Act
            var result = await controller.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var deletedInspector = context.Inspectors.Find(3);
            Assert.Null(deletedInspector);
        }
    }
}
