// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

//void UnityPause(bool pause);// No longer supported in Unity 4.3+
//void UnitySendMessage(const char * className, const char * methodName, const char * param);

//UIViewController *UnityGetGLViewController();// Use for Unity4.3.4- support.
extern "C" UIViewController *UnityGetGLViewController();// Use for Unity4.5+ support.

// Converts C style string to NSString
#define GetStringParam( _x_ ) ( _x_ != NULL ) ? [NSString stringWithUTF8String:_x_] : [NSString stringWithUTF8String:""]