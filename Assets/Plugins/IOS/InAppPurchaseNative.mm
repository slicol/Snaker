// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "InAppPurchaseNative.h"
#import "UnityTypes.h"
#import "VerificationController.h"
#import "NSData+Base64.h"

@implementation InAppPurchaseNative
- (id)init
{
    self = [super init];
    // init other data here...
    return self;
}

- (void)dealloc
{
    // dispose...
    [[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
    
    if (productIdentifiers != nil)
    {
        #if !UNITY_5_0_0
        [productIdentifiers release];
        #endif
        productIdentifiers = nil;
    }
    
    if (products != nil)
    {
        #if !UNITY_5_0_0
        [products release];
        #endif
        products = nil;
    }
    
    #if !UNITY_5_0_0
    [super dealloc];
    #endif
}

- (void)CreateAppleStore:(char**)productIDs :(int)productIDCount
{
    // defaults
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    restoreProductsDone = false;
    buyProductDone = false;
    restoreProductPruductIDs = [[NSMutableArray alloc] init];
    
    // request product list
    productsLoaded = false;
    
    NSMutableArray* productArray = [[NSMutableArray alloc] init];
    for (int i = 0; i != productIDCount; ++i)
    {
        [productArray addObject:[[NSString alloc] initWithUTF8String:productIDs[i]]];
    }
    
    productIdentifiers = [[NSSet alloc] initWithArray:productArray];
    SKProductsRequest *request = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    request.delegate = self;
    [request start];
}

- (void)productsRequest:(SKProductsRequest*)request didReceiveResponse:(SKProductsResponse*)response
{
    products = [[NSArray alloc] initWithArray:response.products];
    productsLoaded = true;
}

- (void)BuyProduct:(NSString*)productIdentifier
{
    buyProductSucceeded = false;
    buyProductDone = false;
    
    if (!productsLoaded)
    {
        NSLog(@"Produces not loaded yet.");
        buyProductSucceeded = false;
        buyProductDone = true;
        return;
    }
    
    if (![SKPaymentQueue canMakePayments])
    {
        NSLog(@"canMakePayments Failed.");
        buyProductSucceeded = false;
        buyProductDone = true;
        return;
    }
    
    SKProduct* buyProduct = nil;
    for (int i = 0; i != products.count; ++i)
    {
        SKProduct* product = [products objectAtIndex:i];
        NSString* idd = product.productIdentifier;
        if ([idd isEqualToString:productIdentifier])
        {
            buyProduct = product;
            break;
        }
    }
    
    if (buyProduct == nil)
    {
        buyProductSucceeded = false;
        buyProductDone = true;
        return;
    }
    
    restoreValidateTic = 0;
    restoreValidateCount = 0;
    buyProductID = buyProduct.productIdentifier;
	SKPayment* payment = [SKPayment paymentWithProduct:buyProduct];
	[[SKPaymentQueue defaultQueue] addPayment:payment];
}

- (void)RestoreProducts
{
    [restoreProductPruductIDs removeAllObjects];
    
    if (!productsLoaded)
    {
        NSLog(@"Produces not loaded yet.");
        restoreProductsDone = true;
        return;
    }
    
    restoreValidateTic = 0;
    restoreValidateCount = 0;
    restoreProductsDone = false;
	[[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void)validateReceiptForTransactionBuy:(SKPaymentTransaction *)transaction
{
    if (!productsLoaded) return;
    
    VerificationController* verifier = [VerificationController sharedInstance];
    [verifier verifyPurchase:transaction testing:testing baseKey:sharedSecretKey completionHandler:^(BOOL success)
    {
        if (success)
        {
            NSLog(@"Buy Successfully verified receipt!");
            buyProductSucceeded = true;
            buyProductDone = true;
        }
        else
        {
            NSLog(@"Buy Failed to validate receipt.");
            buyProductSucceeded = false;
            buyProductDone = true;
        }
    }];
}

int restoreValidateCount = 0, restoreValidateTic = 0;
bool restoreDone = true;
- (void)validateReceiptForTransactionRestore:(SKPaymentTransaction *)transaction
{
    if (!productsLoaded) return;
    
    while (!restoreDone) sleep(1);
    restoreDone = false;
    VerificationController* verifier = [VerificationController sharedInstance];
    [verifier verifyPurchase:transaction testing:testing baseKey:sharedSecretKey completionHandler:^(BOOL success)
     {
         if (success)
         {
             NSLog(@"Restore Successfully verified receipt!");
             [restoreProductPruductIDs addObject:transaction.payment.productIdentifier];
         }
         else
         {
             NSLog(@"Restore Failed to validate receipt.");
             // do nothing...
         }
         
         ++restoreValidateTic;
         if (restoreValidateTic == restoreValidateCount) restoreProductsDone = true;
         restoreDone = true;
     }];
}

- (void)paymentQueue:(SKPaymentQueue*)queue updatedTransactions:(NSArray*)transactions
{
    // VALIDATION REF !!!!
    // https://github.com/evands/iap_validation
    
    bool isRestoring = false;
	for(SKPaymentTransaction *transaction in transactions)
	{
		switch(transaction.transactionState)
		{
			case SKPaymentTransactionStatePurchasing:
				NSLog(@"StoreKit: Purchasing");
				return;
                
			case SKPaymentTransactionStateFailed:
                if(transaction.error.code == SKErrorPaymentCancelled)
                {
                    // cancled
                    buyProductSucceeded = false;
                    buyProductDone = true;
                }
                else
                {
                    // failed
                    buyProductSucceeded = false;
                    buyProductDone = true;
                    NSLog(@"%@", [transaction.error localizedDescription]);
                }
				[[SKPaymentQueue defaultQueue] finishTransaction:transaction];
				break;
                
			case SKPaymentTransactionStatePurchased:
                NSLog(@"StoreKit: Validating Purchased");
                receipt = [[NSString alloc] initWithString:transaction.transactionReceipt.base64EncodedString];
                if (false)//[[UIDevice currentDevice] respondsToSelector:NSSelectorFromString(@"identifierForVendor")])
                {
                    // iOS 6+ Validate
                    [self validateReceiptForTransactionBuy:transaction];
                    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                }
                else
                {
                    // iOS 5 Don't Validate
                    buyProductSucceeded = true;
                    buyProductDone = true;
                    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                }
                break;
                
			case SKPaymentTransactionStateRestored:
                isRestoring = true;
                NSLog(@"StoreKit: Validating Restored Item");
                if (false)//[[UIDevice currentDevice] respondsToSelector:NSSelectorFromString(@"identifierForVendor")])
                {
                    // iOS 6+ Validate
                    ++restoreValidateCount;// get count first
                }
                else
                {
                    // iOS 5 Don't Validate
                    [restoreProductPruductIDs addObject:transaction.payment.productIdentifier];
                    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                }
				break;
		}
        
        // iOS 6+ Validate
        if (false)//isRestoring && [[UIDevice currentDevice] respondsToSelector:NSSelectorFromString(@"identifierForVendor")])
        {
            for(SKPaymentTransaction *transaction in transactions)
            {
                switch(transaction.transactionState)
                {
                    case SKPaymentTransactionStateRestored:
                        [self validateReceiptForTransactionRestore:transaction];
                        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
                        break;
                }
            }
        }
	}
}

- (void)paymentQueue:(SKPaymentQueue*)queue restoreCompletedTransactionsFailedWithError:(NSError*)error
{
    NSLog(@"%@", [error localizedDescription]);
    [restoreProductPruductIDs removeAllObjects];
    restoreProductsDone = true;
}

- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue*)queue
{
	restoreProductsDone = true;
}

- (NSArray*)GetProductInfo
{
    if (!productsLoaded) return nil;
    return products;
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
#if UNITY_5_0_0
static NSMutableArray* NativeIAPs = nil;
#endif

extern "C"
{
    InAppPurchaseNative* InitInAppPurchase(bool testing, const char* const sharedSecretKey)
    {
        InAppPurchaseNative* iap = [[InAppPurchaseNative alloc] init];
        iap->testing = testing;
        iap->sharedSecretKey = [[NSString alloc] initWithUTF8String:sharedSecretKey];
        
        #if UNITY_5_0_0
        if (NativeIAPs == nil) NativeIAPs = [[NSMutableArray alloc] init];
        [NativeIAPs addObject:iap];
        #endif
        return iap;
    }
    
    void DisposeInAppPurchase(InAppPurchaseNative* native)
    {
        if (native != nil)
        {
            #if !UNITY_5_0_0
            [native release];
            #else
            [NativeIAPs removeObject:native];
            #endif
            native = nil;
        }
    }
    
    void CreateInAppPurchase(InAppPurchaseNative* native, char** productIDs, int productIDCount)
    {
        [native CreateAppleStore:productIDs :productIDCount];
    }
    
    char** GetInAppPurchaseProductInfo(InAppPurchaseNative* native)
    {
        NSArray* products = [native GetProductInfo];
        if (products == nil) return 0;
        char** productInfo = new char*[(products.count * 2) + 1];
        productInfo[products.count * 2] = 0;
        for (int i = 0, i2 = 0; i != products.count; ++i, i2 += 2)
        {
            SKProduct* product = [products objectAtIndex:i];
            // get IAP ID
            const char* idValue = [product.productIdentifier cStringUsingEncoding:NSUTF8StringEncoding];
            
            // format price
            NSNumberFormatter *numberFormatter = [[NSNumberFormatter alloc] init];
            [numberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
            [numberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
            [numberFormatter setLocale:product.priceLocale];
            NSString *formattedString = [numberFormatter stringFromNumber:product.price];
            const char* formattedStringValue = [formattedString cStringUsingEncoding:NSUTF8StringEncoding];
            
            // set values
            productInfo[i2] = (char*)idValue;
            productInfo[i2+1] = (char*)formattedStringValue;
        }
        
        return productInfo;
    }
    
    void RestoreInAppPurchase(InAppPurchaseNative* native)
    {
        [native RestoreProducts];
    }
    
    void BuyInAppPurchase(InAppPurchaseNative* native, const char* inAppID)
    {
        [native BuyProduct:GetStringParam(inAppID)];
    }
    
    bool CheckRestoreInAppPurchaseStatus(InAppPurchaseNative* native)
    {
        bool done = native->restoreProductsDone;
        if (native->restoreProductsDone) native->restoreProductsDone = false;
        return done;
    }
    
    char** GetRestoreInAppPurchaseIDs(InAppPurchaseNative* native)
    {
        char** productIDs = new char*[native->restoreProductPruductIDs.count+1];
        productIDs[native->restoreProductPruductIDs.count] = 0;
        
        for (int i = 0; i != native->restoreProductPruductIDs.count; ++i)
        {
            const char* productID = [[native->restoreProductPruductIDs objectAtIndex:i] cStringUsingEncoding:NSUTF8StringEncoding];
            productIDs[i] = (char*)productID;
        }
        
        return productIDs;
    }
    
    bool CheckBuyInAppPurchaseStatus(InAppPurchaseNative* native)
    {
        bool done = native->buyProductDone;
        if (native->buyProductDone) native->buyProductDone = false;
        return done;
    }
    
    const char* GetBuyInAppPurchaseStatusID(InAppPurchaseNative* native, bool* succeeded)
    {
        *succeeded = native->buyProductSucceeded;
        return [native->buyProductID cStringUsingEncoding:NSUTF8StringEncoding];
    }
    
    const char* GetBuyInAppPurchaseReceipt(InAppPurchaseNative* native)
    {
        if (native->receipt != nil)
        {
            const char* value = [native->receipt cStringUsingEncoding:NSUTF8StringEncoding];
            #if !UNITY_5_0_0
            [native->receipt release];
            #endif
            return value;
        }
        else
        {
            return 0;
        }
    }
}