//
//  CloudOnceAchievements.h
//  CloudOnceAchievements
//
//  Created by Trollpants Game Studio on 20/04/15.
//  Copyright (c) 2015 Trollpants Game Studio AS. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <GameKit/GameKit.h>

@interface CloudOnceAchievements : NSObject{
    NSMutableDictionary *achievementsDictionary;
}

@property(nonatomic, retain) NSMutableDictionary *achievementsDictionary;

typedef void (*ReportAchievementCallback)(BOOL success);
typedef void (*LoadAchievementsCallback)(BOOL success, int count);

- (void) loadAchievements: (void (*)(BOOL success, int count))callbackBlock;
- (GKAchievement*) getAchievementForIdentifier: (NSString*) identifier;
- (void) reportAchievementIdentifier: (NSString*) identifier
                     percentComplete: (float) percent
                            callback: (void (*)(BOOL success))callbackBlock;
- (NSString *) createNSString: (const char *) string;

@end
