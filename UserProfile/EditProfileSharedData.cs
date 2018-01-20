//
//  EditProfileSharedData.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using System.Collections.Generic;

namespace Happy31.UserProfile
{
    /// <summary>
    /// Shared user info among UserProfile and EditProfile
    /// </summary>
    public static class EditProfileSharedData
    {
        public static List<string> EditedUserInfo { get; set; }
    }
}
