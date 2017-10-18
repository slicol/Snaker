// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "SocialNative.h"
#import "UnityTypes.h"

@implementation SocialNative
- (id)init
{
    self = [super init];
    // init other data here...
    return self;
}

- (void)dealloc
{
    // dispose..
#if !UNITY_5_0_0
    [super dealloc];
#endif
}

- (void)ShareImage:(Byte*)data text:(char*)text dataLength:(int)dataLength isPNG:(bool)isPNG
{
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
    
    NSMutableArray *objectsToShare = [[NSMutableArray alloc] init];
    if (data != nil)
    {
        NSData *nsData = [NSData dataWithBytes:data length:dataLength];
        UIImage *image = [UIImage imageWithData:nsData];
        [objectsToShare addObject:image];
    }
    
    if (text != nil)
    {
        NSString* textObj = GetStringParam(text);
        [objectsToShare addObject:textObj];
    }
    
    UIActivityViewController *controller = [[UIActivityViewController alloc] initWithActivityItems:objectsToShare applicationActivities:nil];
    UIViewController *vc = UnityGetGLViewController();
    if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
    {
        controller.popoverPresentationController.sourceView = vc.view;
        controller.popoverPresentationController.sourceRect = PopoverRect;
    }
    [vc presentViewController:controller animated:YES completion:nil];
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static SocialNative* native = nil;

extern "C"
{
    void InitSocial()
    {
        native = [[SocialNative alloc] init];
    }
    
    void DisposeSocial()
    {
        if (native != nil)
        {
#if !UNITY_5_0_0
            [native release];
#endif
            native = nil;
        }
    }
    
    void Social_ShareImage(Byte* data, char* text, int dataLength, bool isPNG, int x, int y, int width, int height)
    {
        native->PopoverRect = CGRectMake(x, y, width, height);
        [native ShareImage:data text:text dataLength:dataLength isPNG:isPNG];
    }
}