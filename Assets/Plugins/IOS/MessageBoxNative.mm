// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "MessageBoxNative.h"
#import "UnityTypes.h"

@implementation MessageBoxNative
- (id)init
{
    self = [super init];
    // init other data here...
    return self;
}

- (void)dealloc
{
    // dispose...
    #if !UNITY_5_0_0
    [super dealloc];
    #endif
}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    okClicked = buttonIndex == 1;
    cancelClicked = buttonIndex == 0;
}

- (void)alertView:(UIAlertView *)alertView didDismissWithButtonIndex:(NSInteger)buttonIndex
{
    // do nothing...
}

- (void)alertView:(UIAlertView *)alertView willDismissWithButtonIndex:(NSInteger)buttonIndex
{
    // do nothing...
}

- (void)alertViewCancel:(UIAlertView *)alertView
{
    // do nothing...
}

- (BOOL)alertViewShouldEnableFirstOtherButton:(UIAlertView *)alertView
{
    return true;
}

- (void)didPresentAlertView:(UIAlertView *)alertView
{
    // do nothing...
}

- (void)willPresentAlertView:(UIAlertView *)alertView
{
    // do nothing...
}

- (void)Show:(NSString*)title message:(NSString*)message okButtonText:(NSString*)okButtonText cancelButtonText:(NSString*)cancelButtonText type:(int)type
{
    UIAlertView* alert = nil;
    if (type == 0)
    {
        alert = [[UIAlertView alloc] initWithTitle:title message:message delegate:self cancelButtonTitle:okButtonText otherButtonTitles:nil];
    }
    else
    {
        alert = [[UIAlertView alloc] initWithTitle:title message:message delegate:self cancelButtonTitle:cancelButtonText otherButtonTitles:okButtonText, nil];
    }
    
    [alert show];
    #if !UNITY_5_0_0
    [alert release];
    #endif
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static MessageBoxNative* native = nil;

extern "C"
{
    void InitMessageBox()
    {
        if (native == nil) native = [[MessageBoxNative alloc] init];
    }
    
    void DisposeMessageBox()
    {
        if (native != nil)
        {
            #if !UNITY_5_0_0
            [native release];
            #endif
            native = nil;
        }
    }
    
    void ShowMessageBox(const char* title, const char* message, const char* okButtonText, const char* cancelButtonText, int type)
    {
        [native Show:GetStringParam(title) message:GetStringParam(message) okButtonText:GetStringParam(okButtonText) cancelButtonText:GetStringParam(cancelButtonText) type:type];
    }
    
    bool MessageBoxOkClicked()
    {
        bool value = native->okClicked;
        native->okClicked = false;
        return value;
    }
    
    bool MessageBoxCancelClicked()
    {
        bool value = native->cancelClicked;
        native->cancelClicked = false;
        return value;
    }
}