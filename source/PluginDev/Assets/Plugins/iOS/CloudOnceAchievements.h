//
//  CloudOnceAchievements.h
//  CloudOnceAchievements
//
//  Created by Jan Ivar Z. Carlsen on 20/04/15.
//  Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
