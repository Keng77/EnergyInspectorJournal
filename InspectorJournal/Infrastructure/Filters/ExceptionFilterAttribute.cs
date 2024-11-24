using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InspectorJournal.Infrastructure.Filters;

//Фильтр исключений
public class ExceptionFilterAttribute : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var actionName = context.ActionDescriptor.DisplayName; //имя метода действия
        var exceptionStack = context.Exception.StackTrace;
        var exceptionMessage = context.Exception.Message;
        context.Result = new ContentResult
        {
            Content = $"В методе {actionName} возникло исключение: \n {exceptionMessage} \n {exceptionStack}"
        };
        context.ExceptionHandled = true;
    }
}