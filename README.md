# EnergyInspectorJournal

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-5C2D91?style=for-the-badge&logo=.net&logoColor=white)  
![Entity Framework Core](https://img.shields.io/badge/Entity%20Framework%20Core-512BD4?style=for-the-badge&logo=microsoft&logoColor=white)  
[![Build and Test](https://github.com/Keng77/EnergyInspectorJournal/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Keng77/EnergyInspectorJournal/actions/workflows/dotnet-desktop.yml)

## 📋 Описание проекта
**EnergyInspectorJournal** — это веб-приложение для управления проверками предприятий. Разработанное на **ASP.NET Core**, приложение интегрирует **Entity Framework Core** для работы с базой данных, включая управление нарушениями, инспекторами и другими связанными сущностями. 

Проект использует кэширование запросов и профилирование кэша для оптимизации производительности, а также настраиваемое middleware для контроля данных.

---

## ✨ Основной функционал

- 📝 **Управление проверками предприятий**: Создание, редактирование и просмотр данных по проверкам.  
- 🚀 **Кэширование запросов**: Настраиваемое middleware для управления кэшем.  
- 🔒 **Сессии и безопасность**: Использование сессий для хранения данных пользователя.  
- 🌐 **Маршрутизация MVC**: Полноценная структура MVC для обработки запросов.

---

## 🛠️ Технологии

- **Backend**: ASP.NET Core  
- **ORM**: Entity Framework Core  
- **Database**: Microsoft SQL Server  
- **Кэширование**: Кастомное middleware  
- **Сессии**: Session Management  

---

## 🚀 Использование

### 🌐 Доступ к приложению
Главная страница приложения доступна по адресу:  
**`/Home/Index`**  

На главной странице отображаются:  
- Список проверок предприятий  
- Нарушения  
- Информация об инспекторах и предприятиях  

### ⚙️ Настройка кэширования
Параметры кэширования задаются в файле `Program.cs` в разделе конфигурации middleware.

---

## 🗂️ Структура проекта

- 📁 **Controllers** — обработка запросов и кэширования  
- 📁 **Data** — настройка контекста базы данных (EF Core)  
- 📁 **Middleware** — кастомное middleware для управления кэшем  
- 📁 **ViewModels** — модели представления для отображения данных  
- 📁 **wwwroot** — статические файлы и ресурсы  

---

## 🛠️ Особенности

- **Сессии**: Приложение сохраняет данные пользователя в сессии для обеспечения удобного взаимодействия.  
- **Профилирование кэша**: Контроллеры используют профили для ускорения обработки запросов.  
- **SQL Server**: База данных для хранения данных о проверках, нарушениях, инспекторах и связанных сущностях.  

---

## 🧑‍💻 Разработка и CI/CD

Этот проект использует **GitHub Actions** для автоматического тестирования и сборки.  

[![Build and Test](https://github.com/Keng77/EnergyInspectorJournal/actions/workflows/dotnet-desktop.yml/badge.svg)](https://github.com/Keng77/EnergyInspectorJournal/actions/workflows/dotnet-desktop.yml)

---

## 🖥️ Локальный запуск

```bash
# Клонируйте репозиторий
git clone https://github.com/Keng77/EnergyInspectorJournal.git

# Перейдите в каталог проекта
cd EnergyInspectorJournal

# Настройте базу данных (в appsettings.json):
# Укажите строку подключения к SQL Server.

# Примените миграции
dotnet ef database update

# Запустите приложение
dotnet run

# Теперь приложение доступно по адресу:
# http://localhost:5000


📝 Лицензия
Проект распространяется под лицензией MIT. Подробнее см. в файле LICENSE.

✨ Разработано с 💙 для управления проверками предприятий!
