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
    public class EnterprisesControllerTest
    {
        private InspectionsDbContext GetInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<InspectionsDbContext>()
                .UseInMemoryDatabase(databaseName)
                .Options;

            return new InspectionsDbContext(options);
        }


        [Fact]
        public void GetEnterpriseList()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetEnterpriseList)); // Инициализируем InMemory контекст
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList()); // Добавляем тестовые данные
            context.SaveChanges();

            var httpContext = new DefaultHttpContext(); // Создаем контекст HTTP
            httpContext.Session = new TestSession();   // Инициализируем сессию

            var controller = new EnterprisesController(context)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act
            var result = controller.Index(); // Выполняем метод Index

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Проверяем, что результат — это представление
            Assert.NotNull(viewResult);

            var model = Assert.IsAssignableFrom<EnterprisesViewModel>(viewResult.ViewData.Model); // Проверяем модель
            Assert.NotNull(model);
            Assert.Equal(3, model.Enterprises.Count()); // Убедимся, что количество предприятий соответствует ожидаемому
        }



        [Fact]
        public async Task GetEnterprise()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(GetEnterprise));
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList());
            context.SaveChanges();

            var controller = new EnterprisesController(context);

            // Act
            var notFoundResult = await controller.Details(4); // Non-existent ID
            var foundResult = await controller.Details(1);   // Existing ID

            // Assert
            Assert.IsType<NotFoundResult>(notFoundResult); // Assert for non-existent ID
            var viewResult = Assert.IsType<ViewResult>(foundResult);
            var model = Assert.IsAssignableFrom<Enterprise>(viewResult.ViewData.Model);
            Assert.Equal("Enterprise One", model.Name);
        }

        [Fact]
        public async Task Create_ReturnsViewResult_GivenInvalidModel()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsViewResult_GivenInvalidModel));
            var controller = new EnterprisesController(context);
            controller.ModelState.AddModelError("error", "some error"); // Добавление ошибки в модель

            var newEnterprise = new Enterprise
            {
                EnterpriseId = 4,
                Name = "Enterprise Four",
                OwnershipType = "Public",
                Address = "101 New Street",
                DirectorName = "Bob Marley",
                DirectorPhone = "123-987-6543"
            };

            // Act
            var result = await controller.Create(newEnterprise);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result); // Ожидается ViewResult
            Assert.NotNull(viewResult);
            var model = Assert.IsAssignableFrom<Enterprise>(viewResult.ViewData.Model);
            Assert.Equal(newEnterprise.Name, model.Name); // Убедиться, что данные переданы обратно
        }


        [Fact]
        public async Task Create_ReturnsARedirectAndCreate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Create_ReturnsARedirectAndCreate_WhenModelStateIsValid));
            var controller = new EnterprisesController(context);

            var newEnterprise = new Enterprise
            {
                EnterpriseId = 4,
                Name = "Enterprise Four",
                OwnershipType = "Public",
                Address = "101 New Street",
                DirectorName = "Bob Marley",
                DirectorPhone = "123-987-6543"
            };

            // Act
            var result = await controller.Create(newEnterprise);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Validate that the new enterprise is added to the database
            var addedEnterprise = context.Enterprises.FirstOrDefault(e => e.EnterpriseId == 4);
            Assert.NotNull(addedEnterprise);
            Assert.Equal("Enterprise Four", addedEnterprise.Name);
        }
        
    [Fact]
        public async Task Edit_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsNotFound));
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList());
            context.SaveChanges();

            var controller = new EnterprisesController(context);

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
            var enterprises = TestDataHelper.GetFakeEnterprisesList();
            var enterprisesContextMock = new Mock<InspectionsDbContext>();
            enterprisesContextMock.Setup(x => x.Enterprises).ReturnsDbSet(enterprises);

            var controller = new EnterprisesController(enterprisesContextMock.Object);
            controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var invalidEnterprise = new Enterprise
            {
                EnterpriseId = 4, // Пример ID, который может не существовать
                Name = "",        // Ошибка в обязательном поле
                Address = "Test Address"
            };
            var result = await controller.Edit(4, invalidEnterprise);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Enterprise>(viewResult.ViewData.Model);
            Assert.Equal(invalidEnterprise.Name, model.Name);  // Проверка, что модель вернулась с ошибками
        }


        [Fact]
        public async Task Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Edit_ReturnsARedirectAndUpdate_WhenModelStateIsValid));
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList());
            context.SaveChanges();

            var controller = new EnterprisesController(context);

            // Объект обновления
            var updatedEnterprise = context.Enterprises.First(e => e.EnterpriseId == 3);
            updatedEnterprise.Name = "Updated Enterprise";
            updatedEnterprise.Address = "Updated Address";

            // Act
            var result = await controller.Edit(3, updatedEnterprise);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            // Проверяем обновленные данные
            var enterpriseFromDb = context.Enterprises.Find(3);
            Assert.NotNull(enterpriseFromDb);
            Assert.Equal("Updated Enterprise", enterpriseFromDb.Name);
            Assert.Equal("Updated Address", enterpriseFromDb.Address);
        }


        [Fact]
        public async Task Delete_ReturnsNotFound()
        {
            // Arrange
            using var context = GetInMemoryContext(nameof(Delete_ReturnsNotFound));
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList());
            context.SaveChanges();

            var controller = new EnterprisesController(context);

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
            context.Enterprises.AddRange(TestDataHelper.GetFakeEnterprisesList());
            context.SaveChanges();

            var controller = new EnterprisesController(context);

            // Act
            var result = await controller.DeleteConfirmed(3);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Null(redirectToActionResult.ControllerName);
            Assert.Equal("Index", redirectToActionResult.ActionName);

            var deletedEnterprise = context.Enterprises.Find(3);
            Assert.Null(deletedEnterprise);
        }
    }
}
