using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;

namespace InspectorJournal.Infrastructure.Filters
{

    // Фильтр действий для записи в сессию данных из ModelState
    public class SetToSessionAttribute : Attribute, IActionFilter
    {
        private readonly string _name; // имя ключа сессии

        public SetToSessionAttribute(string name)
        {
            _name = name;
        }

        // Выполняется до выполнения метода контроллера
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Здесь можно добавить логику, если нужно
        }

        // Выполняется после выполнения метода контроллера
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var dict = new Dictionary<string, string>();

            // Считывание данных из ModelState и запись в сессию
            if (context.ModelState.Count > 0)
            {
                foreach (var item in context.ModelState)
                {
                    var attemptedValue = item.Value?.AttemptedValue ?? string.Empty;
                    dict.Add(item.Key, attemptedValue);
                }

                context.HttpContext.Session.Set(_name, dict);
            }
        }
    }
}
