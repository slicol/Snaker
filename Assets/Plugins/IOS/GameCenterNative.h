// -------------------------------------------------------
//  Created by Andrew Witte.
//  Copyright (c) 2013 Reign-Studios. All rights reserved.
// -------------------------------------------------------

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface ReignNativeGameCenter : NSObject <GKLeaderboardViewControllerDelegate, GKAchievementViewControllerDelegate>
{
@public
    bool reportScoreDone, reportScoreSucceeded;
    NSString* reportScoreError;
    
    bool reportAchievementDone, reportAchievementSucceeded;
    NSString* reportAchievementError;
    
    bool requestAchievementDone, requestAchievementSucceeded;
    NSString* requestAchievementError;
    NSMutableString* requestAchievementResponse;
    
    bool userAuthenticated, authenticateDone;
    NSString* authenticatedError, *userID;
    
    bool resetUserAchievementsDone, resetUserAchievementsSucceeded;
}

- (void)SetCallbacks;
- (void)Authenticate;
- (void)ReportScore:(int64_t)score leaderboardID:(NSString*)leaderboardID;
- (void)ReportAchievement:(NSString*)achievementID percentComplete:(double)percentComplete;
- (void)RequestAchievements;
- (void)ShowScoresPage:(NSString*)leaderboardID;
- (void)ShowAchievementsPage;
- (void)ResetUserAchievements;
@end
