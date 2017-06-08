//
//  CloudOnceAchievements.mm
//  CloudOnceAchievements
//
//  Created by Jan Ivar Z. Carlsen on 20/04/15.
//  Copyright (c) 2016 Jan Ivar Z. Carlsen, Sindri Jóelsson. All rights reserved.
//  Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#import "CloudOnceAchievements.h"

@implementation CloudOnceAchievements
@synthesize achievementsDictionary;

-(id)init
{
    NSLog(@"CloudOnceAchievements init");
    self = [super init];
    achievementsDictionary = [[NSMutableDictionary alloc] init];
    return self;
}

- (void) loadAchievements: (void (*)(BOOL success, int count))callbackBlock
{
    [GKAchievement loadAchievementsWithCompletionHandler:^(NSArray *achievements, NSError *error)
     {
         if (error == nil)
         {
             for (GKAchievement* achievement in achievements)
                 [achievementsDictionary setObject: achievement forKey: achievement.identifier];
             if (callbackBlock != nil) {
                 callbackBlock(YES, (int)achievements.count);
             }
         }
         else
         {
             NSLog(@"Error when loading achievements: %@", error);
             if (callbackBlock != nil) {
                 callbackBlock(NO, 0);
             }
         }
     }];
}

- (GKAchievement*) getAchievementForIdentifier: (NSString*) identifier
{
    GKAchievement *achievement = [achievementsDictionary objectForKey:identifier];
    if (achievement == nil)
    {
        achievement = [[GKAchievement alloc] initWithIdentifier:identifier];
        [achievementsDictionary setObject:achievement forKey:achievement.identifier];
    }
    return achievement;
}

- (void) reportAchievementIdentifier: (NSString*) identifier
                     percentComplete: (float) percent
                            callback: (void (*)(BOOL success))callbackBlock
{
    GKAchievement *achievement = [self getAchievementForIdentifier:identifier];
    if (achievement)
    {
        if(percent >= achievement.percentComplete)
        {
            achievement.percentComplete = percent;
            achievement.showsCompletionBanner = YES;
            [GKAchievement reportAchievements:@[achievement] withCompletionHandler:^(NSError *error)
             {
                 if (error != nil)
                 {
                     NSLog(@"Error when reporting achievement: %@", error);
                     if (callbackBlock != nil) {
                         callbackBlock(NO);
                     }
                 }
                 else
                 {
                     if (callbackBlock != nil) {
                         callbackBlock(YES);
                     }
                 }
             }];
        }
        else
        {
            NSLog(@"Ignoring call to report on achievement, reported progress is lower than previously reported progress.");
            if (callbackBlock != nil) {
                callbackBlock(NO);
            }
        }
    }
}

// Converts C style string to NSString
- (NSString *) createNSString: (const char *) string
{
    if(string)
    {
        return [NSString stringWithUTF8String: string];
    }
    else
    {
        return [NSString stringWithUTF8String:""];
    }
}

@end

static CloudOnceAchievements* achievementsObject = nil;

void EnsureAchievementsObjectIsNotNil()
{
    if (achievementsObject == nil) {
        achievementsObject = [[CloudOnceAchievements alloc] init];
    }
}

extern "C" {
    
    void _LoadAchievements(LoadAchievementsCallback callback)
    {
        NSLog(@"Load avhievements");
        EnsureAchievementsObjectIsNotNil();
        [achievementsObject loadAchievements:callback];
    }
    
    void _UnlockAchievement(const char* achievementId, ReportAchievementCallback callback)
    {
        NSLog(@"Unlock achievement %s", achievementId);
        EnsureAchievementsObjectIsNotNil();
        [achievementsObject reportAchievementIdentifier:[achievementsObject createNSString:achievementId] percentComplete:100.0 callback:callback];
    }
    
    void _RevealAchievement(const char* achievementId, ReportAchievementCallback callback)
    {
        NSLog(@"Reveal achievement %s", achievementId);
        EnsureAchievementsObjectIsNotNil();
        [achievementsObject reportAchievementIdentifier:[achievementsObject createNSString:achievementId] percentComplete:0.0 callback:callback];
    }
    
    void _IncrementAchievement(const char* achievementId, float progress, ReportAchievementCallback callback)
    {
        NSLog(@"Increment achievement %s", achievementId);
        EnsureAchievementsObjectIsNotNil();
        [achievementsObject reportAchievementIdentifier:[achievementsObject createNSString:achievementId] percentComplete:progress callback:callback];
    }
    
}
