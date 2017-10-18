// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>

@interface InAppPurchaseNative : NSObject <SKPaymentTransactionObserver, SKProductsRequestDelegate>
{
@private
    bool productsLoaded;
    NSSet* productIdentifiers;
    NSArray* products;
    
@public
    bool buyProductDone, buyProductSucceeded, restoreProductsDone, testing;
    NSString* buyProductID, *receipt;
    NSMutableArray* restoreProductPruductIDs;
    NSString* sharedSecretKey;
}

- (NSArray*)GetProductInfo;
- (void)BuyProduct:(NSString*)productIdentifier;
- (void)RestoreProducts;
- (void)CreateAppleStore:(char**)productIDs :(int)productIDCount;
@end
