//
//  AppCalendarSingleton.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using EventKit;

namespace Happy31
{
    /// <summary>
    /// Singleton to share an instance of the Calendar
    /// </summary>
    public class AppCalendarSingleton
    {
        public static AppCalendarSingleton Current
        {
            get { return current; }
        }
        private static AppCalendarSingleton current;

        public EKEventStore EventStore
        {
            get { return eventStore; }
        } 
        protected EKEventStore eventStore;

        static AppCalendarSingleton()
        {
            current = new AppCalendarSingleton();
        }
        protected AppCalendarSingleton()
        {
            eventStore = new EKEventStore();
        }
    }
}