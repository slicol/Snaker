// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface StreamNative : NSObject <UIImagePickerControllerDelegate, UIImagePickerControllerDelegate, UINavigationControllerDelegate, UIPopoverControllerDelegate>
{
@public
    bool ImageSavedDone, ImageSavedSucceeded, ImageLoadDone, ImageLoadSucceeded;
    CGRect PopoverRect;
    char *imageData;
    int imageDataSize;
    
@private
    UIPopoverController* popoverViewController;
}

- (void)SaveImageToPhotoAlbum:(char*)data dataSize:(int)dataSize;
- (void)ShowPhotoPicker:(UIImagePickerControllerSourceType)type maxWidth:(int)maxWidth maxHeight:(int)maxHeight;
@end
