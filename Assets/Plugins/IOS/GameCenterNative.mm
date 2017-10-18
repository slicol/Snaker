// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import "GameCenterNative.h"
#import "UnityTypes.h"

@implementation ReignNativeGameCenter
- (id)init
{
    self = [super init];
    // init other data here...
    userAuthenticated = false;
    authenticateDone = false;
    authenticatedError = nil;
    userID = nil;
    
    reportScoreDone = false;
    reportAchievementError = nil;
    reportScoreError = nil;
    return self;
}

- (void)dealloc
{
    // dispose...
    #if !UNITY_5_0_0
    [super dealloc];
    #endif
}

- (void)authenticationChanged
{
    if ([GKLocalPlayer localPlayer].isAuthenticated)
    {
        userID = [[NSString alloc] initWithString:[GKLocalPlayer localPlayer].alias];
        NSLog(@"Authentication changed: player authenticated with user: %@", userID);
        userAuthenticated = true;
    }
    else if (![GKLocalPlayer localPlayer].isAuthenticated)
    {
        NSLog(@"Authentication changed: player not authenticated.");
        userAuthenticated = false;
    }
}

- (void)SetCallbacks
{
    NSNotificationCenter *nc = [NSNotificationCenter defaultCenter];
    [nc addObserver:self selector:@selector(authenticationChanged) name:GKPlayerAuthenticationDidChangeNotificationName object:nil];
}

- (void)Authenticate
{
    #if !UNITY_5_0_0
    if (authenticatedError != nil) [authenticatedError release];
    #endif
    authenticatedError = nil;
    userAuthenticated = false;
    authenticateDone = false;
    NSLog(@"Authenticating local user...");
    if([GKLocalPlayer localPlayer].authenticated == false)
    {
        [[GKLocalPlayer localPlayer] authenticateWithCompletionHandler:^(NSError *error)
         {
             if (error != nil)
             {
                 NSLog(@"GameCenter Error: %@", error.localizedDescription);
                 authenticatedError = [[NSString alloc] initWithString:error.localizedDescription];
                 userAuthenticated = false;
                 authenticateDone = true;
             }
             else
             {
                 userAuthenticated = [GKLocalPlayer localPlayer].authenticated;
                 authenticateDone = true;
             }
         }];
    }
    else
    {
        userAuthenticated = true;
        authenticateDone = true;
    }
}

- (void)ReportScore:(int64_t)score leaderboardID:(NSString*)leaderboardID
{
    #if !UNITY_5_0_0
    GKScore* scoreReporter = [[[GKScore alloc] initWithCategory:leaderboardID] autorelease];
    #else
    GKScore* scoreReporter = [[GKScore alloc] initWithCategory:leaderboardID];
    #endif
    scoreReporter.value = score;
    [scoreReporter reportScoreWithCompletionHandler: ^(NSError *error)
     {
         if (error != nil)
         {
             reportScoreError = [[NSString alloc] initWithString:error.localizedDescription];
             reportScoreSucceeded = false;
         }
         else
         {
             if (reportScoreError != nil)
             {
                 #if !UNITY_5_0_0
                 [reportScoreError dealloc];
                #endif
                 reportScoreError = nil;
             }
             reportScoreSucceeded = true;
         }
         
         reportScoreDone = true;
     }];
}

- (void)ReportAchievement:(NSString*)achievementID percentComplete:(double)percentComplete
{
    #if !UNITY_5_0_0
    GKAchievement* achievement = [[[GKAchievement alloc] initWithIdentifier:achievementID] autorelease];
    #else
    GKAchievement* achievement = [[GKAchievement alloc] initWithIdentifier:achievementID];
    #endif
    achievement.showsCompletionBanner = YES;
    achievement.percentComplete = percentComplete;
    [achievement reportAchievementWithCompletionHandler:^(NSError *error)
     {
         if (error != nil)
         {
             reportAchievementError = [[NSString alloc] initWithString:error.localizedDescription];
             reportAchievementSucceeded = false;
         }
         else
         {
             if (reportAchievementError != nil)
             {
                 #if !UNITY_5_0_0
                 [reportAchievementError dealloc];
                #endif
                 reportAchievementError = nil;
             }
             reportAchievementSucceeded = true;
         }
         
         reportAchievementDone = true;
     }];
}

- (void) RequestAchievements
{
    if (requestAchievementResponse != nil)
    {
        #if !UNITY_5_0_0
        [requestAchievementResponse dealloc];
        #endif
        requestAchievementResponse = nil;
    }
    
    [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error)
     {
         if (error != nil)
         {
             NSLog(@"RequestAchievements Error: %@", error.localizedDescription);
             if (requestAchievementError != nil)
             {
                 #if !UNITY_5_0_0
                 [requestAchievementError dealloc];
                #endif
                 requestAchievementError = nil;
             }
             requestAchievementError = [[NSMutableString alloc] initWithString:error.localizedDescription];
             requestAchievementSucceeded = false;
             requestAchievementDone = true;
         }
         else if (achievements != nil)
         {
             NSLog(@"RequestAchievements Succeeded: %d", achievements.count);
             requestAchievementResponse = [[NSMutableString alloc] initWithString:@""];
             bool staring = true;
             for (GKAchievement* a in achievements)
             {
                 if (!staring) [requestAchievementResponse appendString:@":"];
                 staring = false;
                 [requestAchievementResponse appendString:a.identifier];
                 [requestAchievementResponse appendString:@":"];
                 [requestAchievementResponse appendString:[NSString stringWithFormat:@"%f", a.percentComplete]];
             }
             
             requestAchievementSucceeded = true;
             requestAchievementDone = true;
         }
     }];
}

- (void)leaderboardViewControllerDidFinish:(GKLeaderboardViewController *)viewController
{
    [UnityGetGLViewController() dismissModalViewControllerAnimated: YES];
    #if !UNITY_5_0_0
    [viewController release];
    #endif
}

- (void)ShowScoresPage:(NSString*)leaderboardID
{
    GKLeaderboardViewController* leaderboardController = [[GKLeaderboardViewController alloc] init];
    if (leaderboardController != NULL)
    {
        leaderboardController.category = leaderboardID;
        leaderboardController.timeScope = GKLeaderboardTimeScopeWeek;
        leaderboardController.leaderboardDelegate = self;
        [UnityGetGLViewController() presentModalViewController: leaderboardController animated: YES];
    }
}

- (void)achievementViewControllerDidFinish:(GKAchievementViewController *)viewController;
{
    [UnityGetGLViewController() dismissModalViewControllerAnimated: YES];
    #if !UNITY_5_0_0
    [viewController release];
    #endif
}

- (void)ShowAchievementsPage
{
    GKAchievementViewController *achievements = [[GKAchievementViewController alloc] init];
    if (achievements != NULL)
    {
        achievements.achievementDelegate = self;
        [UnityGetGLViewController() presentModalViewController: achievements animated: YES];
    }
}

- (void)ResetUserAchievements
{
    resetUserAchievementsSucceeded = false;
    resetUserAchievementsDone = false;
    [GKAchievement resetAchievementsWithCompletionHandler:^(NSError *error)
     {
         if (error != nil) NSLog(@"ResetUserAchievements Error: %@", error.localizedDescription);
         resetUserAchievementsSucceeded = error == nil;
         resetUserAchievementsDone = true;
     }];
}
@end

// ----------------------------------
// Unity C Link
// ----------------------------------
static ReignNativeGameCenter* native = nil;

extern "C"
{
    void InitGameCenter()
    {
        if (native == nil)
        {
            native = [[ReignNativeGameCenter alloc] init];
            [native SetCallbacks];
        }
    }
    
    void AuthenticateGameCenter()
    {
        [native Authenticate];
    }
    bool GameCenterCheckAuthenticateDone()
    {
        if (native == nil) return false;
        return native->authenticateDone;
    }
    
    bool GameCenterCheckIsAuthenticated()
    {
        if (native == nil) return false;
        return native->userAuthenticated;
    }
    
    char* GameCenterGetAuthenticatedError()
    {
        if (native->authenticatedError == nil) return 0;
        const char* error = [native->authenticatedError cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)error;
    }
    
    char* GameCenterGetUserID()
    {
        const char* userID = [native->userID cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)userID;
    }
    
    void GameCenterReportScore(int64_t score, const char* leaderboardID)
    {
        NSString* nativeID = [[NSString alloc] initWithUTF8String:leaderboardID];
        [native ReportScore:score leaderboardID:nativeID];
    }
    
    bool GameCenterReportScoreDone()
    {
        if (native == nil) return false;
        bool value = native->reportScoreDone;
        native->reportScoreDone = false;
        return value;
    }
    
    bool GameCenterReportScoreSucceeded()
    {
        if (native == nil) return false;
        return native->reportScoreSucceeded;
    }
    
    char* GameCenterReportScoreError()
    {
        if (native->reportScoreError == nil) return 0;
        const char* error = [native->reportScoreError cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)error;
    }
    
    void GameCenterReportAchievement(const char* achievementID, float percentComplete)
    {
        NSString* nativeID = [[NSString alloc] initWithUTF8String:achievementID];
        [native ReportAchievement:nativeID percentComplete:(double)percentComplete];
    }
    
    bool GameCenterReportAchievementDone()
    {
        if (native == nil) return false;
        bool value = native->reportAchievementDone;
        native->reportAchievementDone = false;
        return value;
    }
    
    bool GameCenterReportAchievementSucceeded()
    {
        if (native == nil) return false;
        return native->reportAchievementSucceeded;
    }
    
    char* GameCenterReportAchievementError()
    {
        if (native->reportAchievementError == nil) return 0;
        const char* error = [native->reportAchievementError cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)error;
    }
    
    void GameCenterRequestAchievements()
    {
        [native RequestAchievements];
    }
    
    bool GameCenterRequestAchievementDone()
    {
        if (native == nil) return false;
        bool value = native->requestAchievementDone;
        native->requestAchievementDone = false;
        return value;
    }
    
    bool GameCenterRequestAchievementSucceeded()
    {
        if (native == nil) return false;
        return native->requestAchievementSucceeded;
    }
    
    char* GameCenterRequestAchievementError()
    {
        if (native->requestAchievementError == nil) return 0;
        const char* error = [native->requestAchievementError cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)error;
    }
    
    char* GameCenterRequestAchievementResponse()
    {
        if (native->requestAchievementResponse == nil) return 0;
        const char* response = [native->requestAchievementResponse cStringUsingEncoding:NSUTF8StringEncoding];
        return (char*)response;
    }
    
    void GameCenterShowScoresPage(const char* leaderboardID)
    {
        NSString* nativeID = [[NSString alloc] initWithUTF8String:leaderboardID];
        [native ShowScoresPage:nativeID];
    }
    
    void GameCeneterShowAchievementsPage()
    {
        [native ShowAchievementsPage];
    }
    
    void GameCenterResetUserAchievements()
    {
        [native ResetUserAchievements];
    }
    
    bool GameCenterResetUserAchievementsDone()
    {
        if (native == nil) return false;
        bool value = native->resetUserAchievementsDone;
        native->resetUserAchievementsDone = false;
        return value;
    }
    
    bool GameCenterResetUserAchievementsSucceeded()
    {
        if (native == nil) return false;
        bool value = native->resetUserAchievementsSucceeded;
        native->resetUserAchievementsSucceeded = false;
        return value;
    }
}