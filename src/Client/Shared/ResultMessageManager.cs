using BlazorHero.CleanArchitecture.Shared.Wrapper;

using MudBlazor;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorHero.CleanArchitecture.Client.Shared
{
    public static class ResultMessageManager
    {
        public static void ManageResult(this IResult result, ISnackbar snackBar, Func<Task> succeededAction = null, Func<Task> failedAction = null)
        {
            try
            {
                if (result.Succeeded)
                {
                    if (result.Messages.Any())
                    {
                        snackBar.Add(result.Messages[0], Severity.Success);
                    }
                    if(succeededAction is not null)
                    Task.Run(succeededAction).AndForget(TaskOption.Safe);
                }
                else
                {
                    foreach (var message in result.Messages)
                    {
                        snackBar.Add(message, Severity.Error);                        
                    }
                    if (failedAction is not null)
                        Task.Run(failedAction);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }

    }
}
