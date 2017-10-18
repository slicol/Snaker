// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "StreamNative.h"
#import "UnityTypes.h"

@implementation StreamNative
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

- (void)image:(UIImage*)image finishedSavingImage:(NSError*)error contextInfo:(void*)contextInfo
{
    if (error) ImageSavedSucceeded = false;
    else ImageSavedSucceeded = true;
    ImageSavedDone = true;
}

- (void)SaveImageToPhotoAlbum:(char*)data dataSize:(int)dataSize
{
    NSData* dataObj = [NSData dataWithBytes:(const void*)data length:dataSize];
    UIImage *image = [[UIImage alloc] initWithData:dataObj];
    if(image)
    {
        ImageSavedSucceeded = false;
        ImageSavedDone = false;
        UIImageWriteToSavedPhotosAlbum(image, self, @selector(image:finishedSavingImage:contextInfo:), NULL);
    }
    else
    {
        ImageSavedSucceeded = false;
        ImageSavedDone = true;
    }
}

- (UIImage*)correctImageRotation:(UIImage*)image :(float)width :(float)height
{
    CGSize size;
    size.width = width;
    size.height = height;
    UIGraphicsBeginImageContext( size );
    [image drawInRect:CGRectMake(0, 0, width, height)];
    UIImage* newImage = UIGraphicsGetImageFromCurrentImageContext();
    UIGraphicsEndImageContext();
    
    return newImage;
}

- (void)imagePickerController:(UIImagePickerController*)picker didFinishPickingMediaWithInfo:(NSDictionary*)info
{
    // grab the image
    UIImage *image;
    image = [info objectForKey:UIImagePickerControllerOriginalImage];
    
    // rotate image to fit correct pixel width and height
    if (image.imageOrientation == UIImageOrientationLeft)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    else if (image.imageOrientation == UIImageOrientationRight)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    else if (image.imageOrientation == UIImageOrientationUp)
    {
        // do nothing
    }
    else if (image.imageOrientation == UIImageOrientationDown)
    {
        image = [self correctImageRotation:image :image.size.width :image.size.height];
    }
    
    // process image
    [self performSelector:@selector(processImageFromImagePicker:) withObject:image];
    
    // dimiss the picker
    [self dismissWrappedController];
}

- (void)dismissWrappedController
{
    if (popoverViewController != nil)
    {
        [popoverViewController dismissPopoverAnimated:YES];
        popoverViewController = nil;
    }
    
    UIViewController *vc = UnityGetGLViewController();
    if(vc) [vc dismissViewControllerAnimated:YES completion:nil];
}

void fitInViewIfLarger(float objectWidth, float objectHeight, float viewWidth, float viewHeight, float* newWidth, float* newHeight)
{
    if (objectWidth <= viewWidth && objectHeight <= viewHeight)
    {
        *newWidth = objectWidth;
        *newHeight = objectHeight;
        return;
    }
    
    float objectSlope = objectHeight / objectWidth;
    float viewSlope = viewHeight / viewWidth;
    
    if (objectSlope >= viewSlope)
    {
        *newWidth = (objectWidth/objectHeight) * viewHeight;
        *newHeight = viewHeight;
    }
    else
    {
        *newWidth = viewWidth;
        *newHeight = (objectHeight/objectWidth) * viewWidth;
    }
}

int ShowPhotoPicker_maxWidth, ShowPhotoPicker_maxHeight;
- (void)processImageFromImagePicker:(UIImage*)image
{
    UIImage* newImage = image;
    if (ShowPhotoPicker_maxWidth != 0 && ShowPhotoPicker_maxHeight != 0)
    {
        float newWidth = 0, newHeight = 0;
        fitInViewIfLarger(image.size.width, image.size.height, ShowPhotoPicker_maxWidth, ShowPhotoPicker_maxHeight, &newWidth, &newHeight);
        CGSize newSize = CGSizeMake(newWidth, newHeight);
        
        UIGraphicsBeginImageContextWithOptions(newSize, NO, 0.0);
        [image drawInRect:CGRectMake(0, 0, newSize.width, newSize.height)];
        newImage = UIGraphicsGetImageFromCurrentImageContext();
        UIGraphicsEndImageContext();
    }
    
    NSData *pngData = UIImagePNGRepresentation(image);
    char* tempData = (char*)[pngData bytes];
    imageDataSize = (int)pngData.length;
    imageData = new char[imageDataSize];
    memcpy(imageData, tempData, imageDataSize);
    
    ImageLoadSucceeded = true;
    ImageLoadDone = true;
}

- (void)popoverControllerDidDismissPopover:(UIPopoverController*)popoverController
{
    if (popoverViewController != nil)
    {
        #if !UNITY_5_0_0
        [popoverViewController release];
        #endif
        popoverViewController = nil;
    }
    
    ImageLoadSucceeded = false;
    ImageLoadDone = true;
}

- (void)imagePickerControllerDidCancel:(UIImagePickerController*)picker
{
    [self dismissWrappedController];
    
    //UnityPause(false);
    ImageLoadSucceeded = false;
    ImageLoadDone = true;
}

- (void)popoverController:(UIPopoverController *)popoverController willRepositionPopoverToRect:(inout CGRect *)rect inView:(inout UIView **)view
{
    // do nothing...
}

- (BOOL)popoverControllerShouldDismissPopover:(UIPopoverController *)popoverController
{
    return YES;
}

- (void)showViewController:(UIViewController*)viewController
{
    /*//UnityPause(true);
    
    // cancel the previous delayed call to dismiss the view controller if it exists
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
    
    // show the picker
    UIViewController *vc = UnityGetGLViewController();
    [vc presentModalViewController:viewController animated:YES];*/
    
    [NSObject cancelPreviousPerformRequestsWithTarget:self];
    UIViewController *vc = UnityGetGLViewController();
    if(UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad)
    {
        viewController.popoverPresentationController.sourceView = vc.view;
        viewController.popoverPresentationController.sourceRect = PopoverRect;
    }
    [vc presentViewController:viewController animated:YES completion:nil];
}

- (void)ShowPhotoPicker:(UIImagePickerControllerSourceType)type maxWidth:(int)maxWidth maxHeight:(int)maxHeight
{
    #if !UNITY_5_0_0
    UIImagePickerController *picker = [[[UIImagePickerController alloc] init] autorelease];
    #else
    UIImagePickerController *picker = [[UIImagePickerController alloc] init];
    #endif
    picker.delegate = self;
    picker.sourceType = type;
    picker.allowsEditing = false;
    
    ShowPhotoPicker_maxWidth = maxWidth;
    ShowPhotoPicker_maxHeight = maxHeight;
    ImageLoadSucceeded = false;
    ImageLoadDone = false;
    
    // We need to display this in a popover on iPad
    /*NSString* version = [[UIDevice currentDevice] systemVersion];
    if([version integerValue] < 8.0 && UI_USER_INTERFACE_IDIOM() == UIUserInterfaceIdiomPad && type != UIImagePickerControllerSourceTypeCamera)
    {
        //Class popoverClass = NSClassFromString(@"UIPopoverController");
         //if(!popoverClass)
         //{
         //ImageLoadSucceeded = false;
         //ImageLoadDone = true;
         //return;
         //}
        
        //popoverViewController = [[popoverClass alloc] initWithContentViewController:picker];
        popoverViewController = [[UIPopoverController alloc] initWithContentViewController:picker];
        [popoverViewController setDelegate:self];
        //picker.modalInPopover = YES;
        
        // Display the popover
        UIWindow* window = [UIApplication sharedApplication].keyWindow;
        [popoverViewController presentPopoverFromRect:PopoverRect inView:window permittedArrowDirections:UIPopoverArrowDirectionAny animated:YES];
    }
    else
    {
        [self showViewController:picker];
    }*/
    
    [self showViewController:picker];
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static StreamNative* native = nil;

extern "C"
{
    void InitStream()
    {
        if (native == nil) native = [[StreamNative alloc] init];
    }
    
    void DisposeStream()
    {
        if (native != nil)
        {
            #if !UNITY_5_0_0
            [native release];
            #endif
            native = nil;
        }
    }
    
    bool CheckImageSavedDoneStatus()
    {
        if (native == nil) return false;
        
        bool done = native->ImageSavedDone;
        native->ImageSavedDone = false;
        return done;
    }
    
    bool CheckImageSavedSucceededStatus()
    {
        return native->ImageSavedSucceeded;
    }
    
    void SaveImageStream(char* data, int dataSize)
    {
        [native SaveImageToPhotoAlbum:data dataSize:dataSize];
    }
    
    bool CheckImageLoadStatus()
    {
        if (native == nil) return false;
        
        bool done = native->ImageLoadDone;
        native->ImageLoadDone = false;
        return done;
    }
    
    bool CheckImageLoadSucceededStatus(char** data, int* dataSize)
    {
        *data = native->imageData;
        *dataSize = native->imageDataSize;
        return native->ImageLoadSucceeded;
    }
    
    void LoadImagePicker(int maxWidth, int maxHeight, int x, int y, int width, int height)
    {
        native->PopoverRect = CGRectMake(x, y, width, height);
        [native ShowPhotoPicker:UIImagePickerControllerSourceTypePhotoLibrary maxWidth:maxWidth maxHeight:maxHeight];
    }
    
    void LoadCameraPicker(int maxWidth, int maxHeight)
    {
        native->PopoverRect = CGRectMake(0, 0, 0, 0);
        [native ShowPhotoPicker:UIImagePickerControllerSourceTypeCamera maxWidth:maxWidth maxHeight:maxHeight];
    }
}