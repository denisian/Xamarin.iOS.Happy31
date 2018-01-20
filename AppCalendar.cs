//
//  SettingsTableViewController.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System;
using System.Threading.Tasks;
using CoreLocation;
using EventKit;
using EventKitUI;
using Foundation;
using UIKit;

namespace Happy31
{
    /// <summary>
    /// Add event to the system calendar
    /// </summary>
    public class AppCalendar
    {
        protected class CreateEventEditViewDelegate : EKEventEditViewDelegate
        {
            // we need to keep a reference to the controller so we can dismiss it
            protected EventKitUI.EKEventEditViewController eventController;

            public CreateEventEditViewDelegate(EventKitUI.EKEventEditViewController eventController)
            {
                // save our controller reference
                this.eventController = eventController;
            }

            // Display notofication after successfull adding to calendar
            public override void Completed(EKEventEditViewController controller, EKEventEditViewAction action)
            {
                eventController.DismissViewController(true, null);

                if (action == EKEventEditViewAction.Saved)
                    AppNotifications.Display(new DisplayedPromptsModel() { Category = "", Task = "Task has been successfully scheduled" });
            }
        }

        protected CreateEventEditViewDelegate eventControllerDelegate;

        public Task<bool> RequestAccessToCalendarAsync()
        {
            var taskSource = new TaskCompletionSource<bool>();

            AppCalendarSingleton.Current.EventStore.RequestAccess(EKEntityType.Event,
                    (bool granted, NSError e) =>
                    {
                        if (!granted)
                            taskSource.SetResult(false);
                        else
                            taskSource.SetResult(true);
                    });

            return taskSource.Task;
        }

        public async Task AddToCalendar(DateTime startTime, DateTime endTime, String subject, String details, Boolean isAllDay, UIViewController viewController)
        {
            var granted = await RequestAccessToCalendarAsync();

            if (granted)
            {
                EKEventEditViewController eventController = new EKEventEditViewController();

                // set the controller's event store - it needs to know where/how to save the event
                eventController.EventStore = AppCalendarSingleton.Current.EventStore;

                // wire up a delegate to handle events from the controller
                eventControllerDelegate = new CreateEventEditViewDelegate(eventController);
                eventController.EditViewDelegate = eventControllerDelegate;

                EKEvent newEvent = EKEvent.FromStore(AppCalendarSingleton.Current.EventStore);
                newEvent.StartDate = (NSDate)DateTime.SpecifyKind(startTime, DateTimeKind.Utc);
                newEvent.EndDate = (NSDate)DateTime.SpecifyKind(endTime, DateTimeKind.Utc);
                newEvent.Title = subject;
                newEvent.Notes = details;
                newEvent.AllDay = isAllDay;
                //newEvent.AddAlarm(ConvertReminder(reminder, startTime));
                //newEvent.Availability = ConvertAppointmentStatus(status);

                eventController.Event = newEvent;

                // show the event controller
                viewController.PresentViewController(eventController, true, null);

                CLLocationManager locationManager = new CLLocationManager();

                // if (CLLocationManager.Status == CLAuthorizationStatus.NotDetermined)
                locationManager.RequestWhenInUseAuthorization();
            }
        }
    }
}
