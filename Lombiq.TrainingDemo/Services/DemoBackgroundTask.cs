/*
 * In this demonstration we'll implement a background task that will run in 2 minute intervals five times and will
 * write a message into the error log.
 *
 * In Orchard Core background tasks always run periodically meaning if you want them to stop you need to explicitly
 * disable them from code or from the Background Tasks admin page if the OrchardCore.BackgroundTasks feature is
 * enabled.
 *
 * For scheduling each task Orchard Core uses NCrontab (https://github.com/atifaziz/NCrontab) providing functionality
 * to specify crontab expressions. By default tasks are set to be executed every 5 minutes - and as mentioned they'll
 * run until you disable them. If you want to learn more about crontab expressions you can use this online tool:
 * https://crontab.guru/.
 */

using Microsoft.Extensions.DependencyInjection;
using OrchardCore.BackgroundTasks;
using OrchardCore.BackgroundTasks.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Lombiq.TrainingDemo.Services;

// To set up the default settings for your background task you can use the BackgroundTask attribute. Here you can
// specify the frequency using crontab expressions and setting a description which will be displayed on the Background
// Tasks admin page. Also you can set Enabled to false if you don't want it start right after application start. These
// settings can be updated entirely on the Background Tasks admin page.
[BackgroundTask(Schedule = "*/2 * * * *", Description = "Demo background task that runs every 2 minutes.")]
public sealed class DemoBackgroundTask : IBackgroundTask
{
    // Setting a maximum time this background task will be executed.
    private const int MaxCount = 5;

    // Storing execution times in a private field. Since background tasks are singleton objects this will keep its value
    // while the application runs.
    private int _count;

    // Since background tasks are singletons we'll need this IServiceProvider instance to resolve every non-singleton
    // service. When in doubt, just use this IServiceProvider instance to resolve everything instead of injecting a
    // service via the constructor.
    public async Task DoWorkAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        // This is where the task is implemented. Increment _count and print it to the error log with a message. Notice
        // that there is a GetTaskName() extension method for IBackgroundTask which will return the technical name of
        // the task.

        // Writing a message to the debug output, just so we can see this code running. This will be visible in output
        // window of your IDE when running the app with the debugger attached.
        Debug.WriteLine($"{(++_count).ToTechnicalString()}/{MaxCount}: Hello from {this.GetTaskName()}!");

        if (_count == MaxCount)
        {
            // If it reached the maximum reset it to 0.
            _count = 0;

            // Here is an example of disabling a background task programmatically using BackgroundTaskManager. You can
            // use this method from anywhere in the code base. Here you need to get this service from the service
            // provider since it's a scoped service but background tasks are singletons.
            var backgroundTaskManager = serviceProvider.GetService<BackgroundTaskManager>();

            // The GetDefaultSettings() extension method will return the settings you defined in the BackgroundTask
            // attribute.
            var settings = this.GetDefaultSettings();

            // By setting Enabled to false and using IBackgroundTaskManager to update it the settings will be stored in
            // the database (or updated if it has already been stored) and from now on Orchard Core will ignore this
            // task and it won't be executed.
            settings.Enable = false;
            await backgroundTaskManager.UpdateAsync(settings.Name, settings);
        }
    }
}

// END OF TRAINING SECTION: Background tasks

// NEXT STATION: Events/LoginGreeting.cs
